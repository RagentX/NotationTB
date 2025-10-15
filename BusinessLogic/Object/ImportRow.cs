using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NotationTB.BusinessLogic.Object
{
    public class ImportRow : INotifyPropertyChanged
    {
        private int _rowNumber;
        public int RowNumber
        {
            get => _rowNumber;
            set => SetField(ref _rowNumber, value);
        }

        private string _steelGrade = "";
        public string SteelGrade
        {
            get => _steelGrade;
            set => SetField(ref _steelGrade, value);
        }

        private string _structuralClass = "";
        public string StructuralClass
        {
            get => _structuralClass;
            set => SetField(ref _structuralClass, value);
        }

        private string _materialStandard = "";
        public string MaterialStandard
        {
            get => _materialStandard;
            set => SetField(ref _materialStandard, value);
        }

        private string _productType = "";
        public string ProductType
        {
            get => _productType;
            set => SetField(ref _productType, value);
        }

        private string _productStandard = "";
        public string ProductStandard
        {
            get => _productStandard;
            set => SetField(ref _productStandard, value);
        }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set => SetField(ref _isValid, value);
        }

        private bool _isDuplicate;
        public bool IsDuplicate
        {
            get => _isDuplicate;
            set => SetField(ref _isDuplicate, value);
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set => SetField(ref _error, value);
        }

        private string _warning = "";
        public string Warning
        {
            get => _warning;
            set => SetField(ref _warning, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
