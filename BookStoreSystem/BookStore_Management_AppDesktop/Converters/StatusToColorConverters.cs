using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookStore_Management_AppDesktop.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status?.ToLower() switch
                {
                    "scheduled" => new SolidColorBrush(Color.FromArgb(255, 56, 189, 248)),      // Light Blue
                    "present" => new SolidColorBrush(Color.FromArgb(255, 16, 185, 129)),        // Green
                    "absent" => new SolidColorBrush(Color.FromArgb(255, 239, 68, 68)),          // Red
                    "compensated" => new SolidColorBrush(Color.FromArgb(255, 245, 158, 11)),   // Amber
                    "late" => new SolidColorBrush(Color.FromArgb(255, 249, 115, 22)),          // Orange
                    _ => new SolidColorBrush(Color.FromArgb(255, 100, 116, 139))                // Gray
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ShiftStatusToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status?.ToLower() switch
                {
                    "scheduled" => new SolidColorBrush(Color.FromArgb(50, 56, 189, 248)),
                    "present" => new SolidColorBrush(Color.FromArgb(50, 16, 185, 129)),
                    "absent" => new SolidColorBrush(Color.FromArgb(50, 239, 68, 68)),
                    "compensated" => new SolidColorBrush(Color.FromArgb(50, 245, 158, 11)),
                    "late" => new SolidColorBrush(Color.FromArgb(50, 249, 115, 22)),
                    _ => new SolidColorBrush(Color.FromArgb(50, 100, 116, 139))
                };
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToSoftBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status?.ToLower() switch
                {
                    "scheduled" => new SolidColorBrush(Color.FromArgb(40, 56, 189, 248)),      // Soft Blue
                    "present" => new SolidColorBrush(Color.FromArgb(40, 16, 185, 129)),        // Soft Green
                    "absent" => new SolidColorBrush(Color.FromArgb(40, 239, 68, 68)),          // Soft Red
                    "compensated" => new SolidColorBrush(Color.FromArgb(40, 245, 158, 11)),   // Soft Amber
                    "late" => new SolidColorBrush(Color.FromArgb(40, 249, 115, 22)),          // Soft Orange
                    _ => new SolidColorBrush(Color.FromArgb(40, 100, 116, 139))                // Soft Gray
                };
            }
            return new SolidColorBrush(Color.FromArgb(40, 128, 128, 128));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status?.ToLower() switch
                {
                    "scheduled" => new SolidColorBrush(Color.FromArgb(255, 14, 165, 233)),     // High Contrast Blue
                    "present" => new SolidColorBrush(Color.FromArgb(255, 5, 150, 105)),        // High Contrast Green
                    "absent" => new SolidColorBrush(Color.FromArgb(255, 220, 38, 38)),          // High Contrast Red
                    "compensated" => new SolidColorBrush(Color.FromArgb(255, 217, 119, 6)),    // High Contrast Amber
                    "late" => new SolidColorBrush(Color.FromArgb(255, 234, 88, 12)),          // High Contrast Orange
                    _ => new SolidColorBrush(Color.FromArgb(255, 71, 85, 105))                 // Slate Gray
                };
            }
            return new SolidColorBrush(Color.FromArgb(255, 71, 85, 105));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
