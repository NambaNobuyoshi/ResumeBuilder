using System;
using System.Globalization;
using System.Windows.Data;

namespace ResumeBuilder.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public bool Invert { get; set; } = false; // オプションで反転もできる

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNotNull = value != null;
            return Invert ? !isNotNull : isNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
