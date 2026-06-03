using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookStore_Management_AppDesktop.Converters
    {
        public class BooleanToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool isVisible = value is bool b && b;

                if (parameter is string paramStr && paramStr.Equals("Inverted", StringComparison.OrdinalIgnoreCase))
                {
                    isVisible = !isVisible;
                }

                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is Visibility visibility && visibility  == Visibility.Visible)
                {
                    return true;    
                }
                return false;
            }
        }
    }
