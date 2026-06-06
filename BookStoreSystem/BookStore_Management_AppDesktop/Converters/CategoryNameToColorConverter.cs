using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookStore_Management_AppDesktop.Converters
{
    /// <summary>
    /// Converter that maps a category name to a pair of visually pleasing colors
    /// (soft background + matching foreground). The same category name always
    /// resolves to the same color pair so the UI stays consistent.
    ///
    /// Usage:
    ///   Background: ConverterParameter="Background"
    ///   Foreground: ConverterParameter="Foreground"
    ///   (Default when no parameter is supplied is "Background".)
    /// </summary>
    public class CategoryNameToColorConverter : IValueConverter
    {
        // Soft pastel background + readable foreground pairs (Tailwind-inspired)
        private static readonly (Color Background, Color Foreground)[] ColorPalette =
        {
            (Color.FromArgb(255, 219, 234, 254), Color.FromArgb(255, 30, 64, 175)),    // Blue
            (Color.FromArgb(255, 220, 252, 231), Color.FromArgb(255, 22, 101, 52)),    // Green
            (Color.FromArgb(255, 254, 226, 226), Color.FromArgb(255, 153, 27, 27)),    // Red
            (Color.FromArgb(255, 254, 243, 199), Color.FromArgb(255, 146, 64, 14)),    // Amber
            (Color.FromArgb(255, 243, 232, 255), Color.FromArgb(255, 112, 26, 117)),   // Purple
            (Color.FromArgb(255, 207, 250, 254), Color.FromArgb(255, 12, 74, 110)),    // Cyan
            (Color.FromArgb(255, 255, 237, 213), Color.FromArgb(255, 154, 52, 18)),    // Orange
            (Color.FromArgb(255, 252, 231, 243), Color.FromArgb(255, 157, 23, 77)),    // Pink
            (Color.FromArgb(255, 236, 252, 203), Color.FromArgb(255, 63, 98, 18)),     // Lime
            (Color.FromArgb(255, 224, 231, 255), Color.FromArgb(255, 67, 56, 202)),    // Indigo
            (Color.FromArgb(255, 204, 251, 241), Color.FromArgb(255, 17, 94, 89)),     // Teal
            (Color.FromArgb(255, 229, 231, 235), Color.FromArgb(255, 31, 41, 55)),     // Gray
            (Color.FromArgb(255, 254, 215, 170), Color.FromArgb(255, 124, 45, 18)),    // Peach
            (Color.FromArgb(255, 199, 210, 254), Color.FromArgb(255, 55, 48, 163)),    // Violet
        };

        private static readonly Color DefaultBackground = Color.FromArgb(255, 241, 245, 249); // #F1F5F9
        private static readonly Color DefaultForeground = Color.FromArgb(255, 71, 85, 105);  // #475569

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string categoryName && !string.IsNullOrWhiteSpace(categoryName))
            {
                int index = GetPaletteIndex(categoryName);
                var pair = ColorPalette[index];

                bool wantForeground = parameter is string p
                                      && p.Equals("Foreground", StringComparison.OrdinalIgnoreCase);

                return new SolidColorBrush(wantForeground ? pair.Foreground : pair.Background);
            }

            bool fallbackForeground = parameter is string fp
                                      && fp.Equals("Foreground", StringComparison.OrdinalIgnoreCase);

            return new SolidColorBrush(fallbackForeground ? DefaultForeground : DefaultBackground);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static int GetPaletteIndex(string categoryName)
        {
            // Case-insensitive hash so the same category name (regardless of casing) gets the same color
            int hash = 0;
            unchecked
            {
                foreach (char c in categoryName)
                {
                    hash = (hash * 31) + char.ToLowerInvariant(c);
                }
            }
            return Math.Abs(hash) % ColorPalette.Length;
        }
    }
}
