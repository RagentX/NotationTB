namespace NotationTB.BusinessLogic.Object
{
    public class ImportRow
    {
        public int RowNumber { get; set; }
        public string SteelGrade { get; set; } = "";             // Марка стали
        public string StructuralClass { get; set; } = "";         // Структурный класс (MaterialsType)
        public string MaterialStandard { get; set; } = "";        // Стандарт/ТУ на материалы (MaterialsStandard)
        public string ProductType { get; set; } = "";             // Тип полуфабриката (ProductsType, см. БД)
        public string ProductStandard { get; set; } = "";         // Стандарт полуфабриката (ProductsStandard)

        // Служебные поля предпросмотра / валидации
        public bool IsValid { get; set; }
        public bool IsDuplicate { get; set; }
        public string Error { get; set; } = "";
        public string Warning { get; set; } = "";                // на случай авто-правок
    }
}
