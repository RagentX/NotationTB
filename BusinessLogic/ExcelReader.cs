// ExcelReader.cs
using NotationTB.BusinessLogic.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

public static class ExcelReader
{
    public static List<ImportRow> Read(string path)
    {
        Excel.Application? app = null;
        Excel.Workbook? wb = null;
        Excel.Worksheet? ws = null;
        Excel.Range? used = null;

        try
        {
            app = new Excel.Application
            {
                Visible = false,
                DisplayAlerts = false
            };

            // Открываем книгу только для чтения
            wb = app.Workbooks.Open(
                Filename: path,
                ReadOnly: true,
                Editable: false
            );

            ws = (Excel.Worksheet)wb.Worksheets[1];

            used = ws.UsedRange;
            if (used == null || used.Rows.Count == 0 || used.Columns.Count == 0)
                throw new InvalidOperationException("Лист пуст или не содержит используемого диапазона.");

            int firstRow = used.Row;
            int firstCol = used.Column;
            int lastRow = used.Row + used.Rows.Count - 1;
            int lastCol = used.Column + used.Columns.Count - 1;

            // Читаем строку заголовков (первая использованная строка)
            int headerRow = firstRow;

            // Нормализатор текста ячейки
            static string CellToString(Excel.Range cell)
            {
                // Value2 быстрее и не форматирует даты/валюты по локали
                var v = cell?.Value2;
                return v == null ? string.Empty : Convert.ToString(v)?.Trim() ?? string.Empty;
            }
            static string Normalize(string s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            // Составляем словарь "нормализованное имя → абсолютный индекс столбца"
            var headerByName = new Dictionary<string, int>();
            for (int c = firstCol; c <= lastCol; c++)
            {
                var hdr = Normalize(CellToString(ws.Cells[headerRow, c]));
                if (!string.IsNullOrEmpty(hdr) && !headerByName.ContainsKey(hdr))
                    headerByName[hdr] = c;
            }

            int Col(string name)
            {
                var key = Normalize(name);
                return headerByName.TryGetValue(key, out var col) ? col : -1;
            }

            // Ожидаемые заголовки (любой регистр):
            // Марка стали | Структурный класс | Стандарт или технические условия на материалы | тип полуфабриката | стандарт полуфабриката
            int cSteel = Col("марка стали");
            int cClass = Col("структурный класс");
            int cMatStd = Col("стандарт или технические условия на материалы");
            int cProdTyp = Col("тип полуфабриката");
            int cProdStd = Col("стандарт полуфабриката");

            if (new[] { cSteel, cClass, cMatStd, cProdTyp, cProdStd }.Any(i => i <= 0))
                throw new InvalidOperationException("Не найдены требуемые столбцы в первой строке.");

            var list = new List<ImportRow>();

            // Идём по строкам данных
            for (int r = headerRow + 1; r <= lastRow; r++)
            {
                // Пропускаем полностью пустые строки (по ключевым столбцам)
                bool isEmpty =
                    string.IsNullOrEmpty(CellToString(ws.Cells[r, cSteel])) &&
                    string.IsNullOrEmpty(CellToString(ws.Cells[r, cClass])) &&
                    string.IsNullOrEmpty(CellToString(ws.Cells[r, cMatStd])) &&
                    string.IsNullOrEmpty(CellToString(ws.Cells[r, cProdTyp])) &&
                    string.IsNullOrEmpty(CellToString(ws.Cells[r, cProdStd]));

                if (isEmpty) continue;

                list.Add(new ImportRow
                {
                    RowNumber = r,
                    SteelGrade = CellToString(ws.Cells[r, cSteel]),
                    StructuralClass = CellToString(ws.Cells[r, cClass]),
                    MaterialStandard = CellToString(ws.Cells[r, cMatStd]),
                    ProductType = CellToString(ws.Cells[r, cProdTyp]),
                    ProductStandard = CellToString(ws.Cells[r, cProdStd])
                });
            }

            return list;
        }
        finally
        {
            // Аккуратно освобождаем COM-объекты, начиная с мелких
            void Release(object? o)
            {
                try
                {
                    if (o != null && Marshal.IsComObject(o))
                        Marshal.ReleaseComObject(o);
                }
                catch { /* ignore */ }
            }

            if (wb != null)
            {
                try { wb.Close(SaveChanges: false); } catch { /* ignore */ }
            }
            if (app != null)
            {
                try { app.Quit(); } catch { /* ignore */ }
            }

            Release(used);
            Release(ws);
            Release(wb);
            Release(app);

            // Финальная очистка
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
