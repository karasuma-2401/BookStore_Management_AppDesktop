using System;
using System.Globalization;
using System.Windows.Data;

namespace BookStore_Management_AppDesktop.Converters
{
    public class NumberSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue && parameter is string param)
            {
                if (param == "Positive") return decimalValue > 0;
                if (param == "Negative") return decimalValue < 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}