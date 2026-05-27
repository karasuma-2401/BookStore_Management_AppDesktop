using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookStore_Management_AppDesktop.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                int.TryParse(parameter?.ToString() ?? "0", out int compareValue);
                bool isGreater = intValue > compareValue;

                if (targetType == typeof(Visibility))
                {
                    return isGreater ? Visibility.Visible : Visibility.Collapsed;
                }

                return isGreater;
            }

            if (targetType == typeof(Visibility))
            {
                return Visibility.Collapsed;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}