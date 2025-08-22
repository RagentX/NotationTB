using System.Data;
using System.Data.Common;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using NotationTB.BusinessLogic.Object;
using NotationTB.Data;
using NotationTB.Models;

namespace NotationTB.BusinessLogic
{
    public record LookupIds(
        int MaterialsTypeId,
        int MaterialsStampId,
        int MaterialsStandardId,
        int ProductsTypeId,
        int ProductsStandardId
    );

    public interface IPgRepository
    {
        Task<LookupIds?> ResolveIdsAsync(ImportRow row, CancellationToken ct);
        Task<bool> IsDuplicateCombinationAsync(int materialsStampId, int productsStandardId, int materialsStandardId, CancellationToken ct);
        Task InsertCombinationAsync(int materialsStampId, int productsStandardId, int materialsStandardId, IDbTransaction? tx, CancellationToken ct);
    }

    public class PgRepository : IPgRepository
    {
        private readonly AppDbContext _db;

        public PgRepository(AppDbContext db) => _db = db;

        public async Task<LookupIds?> ResolveIdsAsync(ImportRow row, CancellationToken ct)
        {
            // 1) Структурный класс (MaterialsType) по имени
            var materialsType = await _db.MaterialsTypes
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Name.ToLower().Replace(" ", "") == row.StructuralClass.ToLower().Replace(" ", ""), ct);
            if (materialsType is null) return null; // нет такого структурного класса
            int materialsTypeId = materialsType.Id; // :contentReference[oaicite:5]{index=5}

            // 2) Марка стали (MaterialsStamp) по имени + проверка 1:1 по TypeId
            var stamp = await _db.MaterialsStamps
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Name == row.SteelGrade, ct);
            if (stamp is null) return null; // марки нет
            if (stamp.TypeId != materialsTypeId)
                throw new InvalidOperationException("Марка стали привязана к другому структурному классу."); // :contentReference[oaicite:6]{index=6}

            // 3) Стандарт/ТУ на материал (MaterialsStandard) по имени
            var matStd = await _db.MaterialsStandards
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Name == row.MaterialStandard, ct);
            if (matStd is null) return null; // нет стандарта/ТУ материала :contentReference[oaicite:7]{index=7}

            // 4) Тип полуфабриката (ProductsType) по имени
            var prodType = await _db.ProductsTypes
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Name.ToLower().Replace(" ", "") == row.ProductType.ToLower().Replace(" ", ""), ct);
            if (prodType is null) return null;

            // 5) Стандарт полуфабриката (ProductsStandard) — по (Name, TypeId)!
            var prodStd = await _db.ProductsStandards
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Name == row.ProductStandard && s.TypeId == prodType.Id, ct);
            if (prodStd is null) return null; // поддерживает кейс одно имя на разных типах :contentReference[oaicite:8]{index=8}

            return new LookupIds(
                materialsTypeId,
                stamp.Id,
                matStd.Id,
                prodType.Id,
                prodStd.Id
            );
        }

        public async Task<bool> IsDuplicateCombinationAsync(int materialsStampId, int productsStandardId, int materialsStandardId, CancellationToken ct)
        {
            // Точный дубль по тройке (MaterialId, ProStandardId, MatStandardId) — пропускаем при записи
            return await _db.MaterialsAndProductsCombinations
                .AsNoTracking()
                .AnyAsync(c =>
                    c.MaterialId == materialsStampId &&
                    c.ProStandardId == productsStandardId &&
                    c.MatStandardId == materialsStandardId, ct); // :contentReference[oaicite:9]{index=9}
        }

        public async Task InsertCombinationAsync(int materialsStampId, int productsStandardId, int materialsStandardId, IDbTransaction? tx, CancellationToken ct)
        {
            // Если нам передали внешнюю транзакцию — «прикрепимся» к ней (если совместима)
            if (tx is DbTransaction dbTx)
            {
                // Если контекст ещё не использует транзакцию — подцепим
                var current = _db.Database.CurrentTransaction;
                if (current == null || current.GetDbTransaction() != dbTx)
                {
                    await _db.Database.UseTransactionAsync(dbTx, ct);
                }
            }

            var entity = new MaterialsAndProductsCombination
            {
                MaterialId = materialsStampId,
                ProStandardId = productsStandardId,
                MatStandardId = materialsStandardId
            }; 


            _db.MaterialsAndProductsCombinations.Add(entity);
            await _db.SaveChangesAsync(ct);
        }
    }

}
