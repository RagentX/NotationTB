// Uploader.cs (EF Core)
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NotationTB.BusinessLogic.Object;
using NotationTB.Data;

namespace NotationTB.BusinessLogic
{
    

    public class Uploader
    {
        private readonly IRowValidator _validator;
        private readonly IPgRepository _repo;
        private readonly AppDbContext _db;

        public Uploader(IRowValidator validator, IPgRepository repo, AppDbContext db)
        {
            _validator = validator;
            _repo = repo;
            _db = db;
        }

        /// <summary>
        /// Проверка и подготовка строк к сохранению: нормализация, поиск справочников, пометка дублей.
        /// </summary>
        public async Task ValidateAsync(IList<ImportRow> rows, CancellationToken ct)
        {
            foreach (var r in rows)
            {
                // локальная валидация/нормализация
                var vr = _validator.ValidateAndFix(r);
                r.IsValid = vr.IsValid;
                r.Error = vr.Error;
                r.Warning = vr.Warning;

                if (!r.IsValid)
                    continue;

                try
                {
                    var ids = await _repo.ResolveIdsAsync(r, ct);
                    if (ids is null)
                    {
                        r.IsValid = false;
                        r.Error = "Не найдены справочные значения (марка/класс/стандарты/тип) в БД.";
                        continue;
                    }

                    r.IsDuplicate = await _repo.IsDuplicateCombinationAsync(
                        ids.MaterialsStampId, ids.ProductsStandardId, ids.MaterialsStandardId, ct);
                }
                catch (InvalidOperationException ex)
                {
                    // Жёсткие проверки: 1:1 Марка ↔ Структурный класс и т.п.
                    r.IsValid = false;
                    r.Error = ex.Message;
                }
            }
        }

        /// <summary>
        /// Сохранение в БД валидных и не-дублирующихся строк. Пишем порциями по 200 в отдельных транзакциях.
        /// </summary>
        public async Task<int> SaveAsync(IList<ImportRow> rows, CancellationToken ct)
        {
            int inserted = 0;

            // Берём только актуальные к вставке строки
            var candidates = rows.Where(r => r.IsValid && !r.IsDuplicate).ToList();
            if (candidates.Count == 0)
                return 0;

            foreach (var batch in candidates.Chunk(200))
            {
                // EF Core транзакция на общий DbContext
                await using var tx = await _db.Database.BeginTransactionAsync(ct);
                try
                {
                    foreach (var r in batch)
                    {
                        // На момент записи справочники могли измениться — переопределим Ids
                        var ids = await _repo.ResolveIdsAsync(r, ct);
                        if (ids is null)
                        {
                            r.IsValid = false;
                            r.Error = "Справочники изменились — повторите проверку.";
                            continue;
                        }

                        // Повторная проверка дубля уже "на финише"
                        var dup = await _repo.IsDuplicateCombinationAsync(
                            ids.MaterialsStampId, ids.ProductsStandardId, ids.MaterialsStandardId, ct);
                        if (dup)
                        {
                            r.IsDuplicate = true;
                            r.Error = "Дубль — будет пропущено";
                            continue;
                        }

                        // Вставка. Передаём внутрь текущую транзакцию DbContext'а.
                        await _repo.InsertCombinationAsync(
                            ids.MaterialsStampId, ids.ProductsStandardId, ids.MaterialsStandardId,
                            tx.GetDbTransaction(), ct);

                        inserted++;
                        r.Error = "OK";
                    }

                    await tx.CommitAsync(ct);
                }
                catch
                {
                    await tx.RollbackAsync(ct);
                    throw;
                }
            }

            return inserted;
        }
    }

}
