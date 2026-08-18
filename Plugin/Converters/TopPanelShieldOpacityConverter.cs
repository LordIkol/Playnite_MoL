using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MythosHelper.Converters
{
    public class TopPanelShieldOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double scrollOffset = System.Convert.ToDouble(value);
            double anchor = StickyOffsetConverter.AnchorHeight;
            if (anchor <= 0) anchor = 150.0;
            
            // Starts fading in smoothly as you scroll down
            return Math.Min(1.0, Math.Max(0.0, scrollOffset / anchor));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
