using System.Globalization;
using System.Windows.Data;

namespace BookStore_Management_AppDesktop.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                if (int.TryParse(parameter?.ToString() ?? "0", out int compareValue))
                {
                    return intValue > compareValue;
                }
                return intValue > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
