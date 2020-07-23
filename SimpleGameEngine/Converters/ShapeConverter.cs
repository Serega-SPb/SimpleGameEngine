using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleGameEngine.Converters
{
    public class ShapeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Activator.CreateInstance((Type) value) : null;
        }
    }
}