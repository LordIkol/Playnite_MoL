using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MythosHelper.Converters
{
    /// <summary>
    /// Computes the Y offset needed to make an element "sticky" when it is placed OUTSIDE the ScrollViewer.
    /// This avoids Z-Index and ScrollViewer clipping bugs, especially with HWNDs.
    /// Input: ScrollViewer.VerticalOffset
    /// Returns: Math.Max(0.0, AnchorHeight - VerticalOffset)
    /// </summary>
    public class StickyHeaderOutsideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double scrollOffset = System.Convert.ToDouble(value);
            double anchor = StickyOffsetConverter.AnchorHeight; // same static anchor
            
            var settings = MythosHelperPlugin.Instance?.Settings;
            if (settings != null)
            {
                if (settings.HeaderBehaviorSetting == Settings.HeaderBehavior.TopFixed)
                {
                    return 0.0; // Always at top
                }
                else if (settings.HeaderBehaviorSetting == Settings.HeaderBehavior.BelowBanner)
                {
                    return anchor - scrollOffset; // Never stick, just scroll up
                }
            }

            return Math.Max(60.0, anchor - scrollOffset);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
