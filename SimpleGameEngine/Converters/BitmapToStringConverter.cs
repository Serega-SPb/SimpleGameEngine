using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SimpleGameEngine.Converters
{
    public class BitmapToStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((BitmapImage) value)?.UriSource?.LocalPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value?.ToString();
            if (string.IsNullOrWhiteSpace(strValue))
                return null;
            
            var path = strValue;
            
            if (!Path.IsPathRooted(strValue))
                path = Path.GetFullPath(path);

            return new BitmapImage(new Uri(path));
        }
    }
}