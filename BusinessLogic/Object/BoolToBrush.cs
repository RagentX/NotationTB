// BoolToBrushConverter.cs — простой конвертер для подсветки
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;



public class BoolToBrush : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // valid: зелёный / ошибка: розовый / дубль: жёлтый — выделим в код-бихайнд
        return Brushes.White;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

