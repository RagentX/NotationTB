using NotationTB.BusinessLogic.Object;
using System.Text.RegularExpressions;

namespace NotationTB.BusinessLogic
{
    public interface IRowValidator
    {
        ImportRow ValidateAndFix(ImportRow row);
    }

    public class RowValidator : IRowValidator
    {
        // "[Буквенный код][пробел][цифры и тире]" — только схлопывание лишних пробелов
        // Примеры валидных: "ГОСТ 12345-67", "TU 12-3", "ASTM A-370"
        private static readonly Regex DocPattern =
            new(@"^[\p{L}]+ [0-9][0-9\-\.]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public ImportRow ValidateAndFix(ImportRow row)
        {
            row.Error = "";
            row.Warning = "";
            row.IsValid = false; // по умолчанию

            // 1) Тримим и схлопываем пробелы
            row.SteelGrade = CollapseSpaces(row.SteelGrade);
            row.StructuralClass = CollapseSpaces(row.StructuralClass);
            row.MaterialStandard = CollapseSpaces(row.MaterialStandard);
            row.ProductType = CollapseSpaces(row.ProductType);
            row.ProductStandard = CollapseSpaces(row.ProductStandard);

            // 2) Обязательность
            if (string.IsNullOrWhiteSpace(row.SteelGrade) ||
                string.IsNullOrWhiteSpace(row.StructuralClass) ||
                string.IsNullOrWhiteSpace(row.MaterialStandard) ||
                string.IsNullOrWhiteSpace(row.ProductType) ||
                string.IsNullOrWhiteSpace(row.ProductStandard))
            {
                row.Error = "Не все обязательные поля заполнены.";
                return row;
            }

            // 3) Формат документов (только пробелы «чиним»)
            if (!DocPattern.IsMatch(row.MaterialStandard))
            {
                row.Error = "Поле «Стандарт/ТУ на материалы» не соответствует формату [КОД] [цифры-тире-точки].";
                return row;
            }
            if (!DocPattern.IsMatch(row.ProductStandard))
            {
                row.Error = "Поле «Стандарт полуфабриката» не соответствует формату [КОД] [цифры-тире-точки].";
                return row;
            }

            row.IsValid = true;
            return row;
        }

        private static string CollapseSpaces(string s)
            => Regex.Replace((s ?? string.Empty).Trim(), @"\s+", " ");
    }

}
