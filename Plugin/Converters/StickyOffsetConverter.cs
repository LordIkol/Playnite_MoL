using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MythosHelper.Converters
{
    /// <summary>
    /// Updates the static AnchorHeight used by StickyOffsetConverter.
    /// Bind this to the ActualHeight of the banner.
    /// </summary>
    public class SetAnchorHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value != DependencyProperty.UnsetValue)
            {
                StickyOffsetConverter.AnchorHeight = System.Convert.ToDouble(value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Computes the Y offset needed to make an element "sticky" inside a ScrollViewer.
    /// Uses a static AnchorHeight updated by SetAnchorHeightConverter to avoid Playnite MultiBinding issues.
    /// Input: ScrollViewer.VerticalOffset
    /// Returns: Max(0, VerticalOffset - AnchorHeight)
    /// </summary>
    public class StickyOffsetConverter : IValueConverter
    {
        // Globally shared anchor height, updated by SetAnchorHeightConverter
        public static double AnchorHeight { get; set; } = 0.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double scrollOffset = System.Convert.ToDouble(value);
            
            var settings = MythosHelperPlugin.Instance?.Settings;
            if (settings != null)
            {
                if (settings.HeaderBehaviorSetting == Settings.HeaderBehavior.TopFixed)
                {
                    return scrollOffset - AnchorHeight; // Offset so it locks to Y=0 from the very top
                }
                else if (settings.HeaderBehaviorSetting == Settings.HeaderBehavior.BelowBanner)
                {
                    return 0.0; // Never offset, let it scroll away naturally
                }
            }

            // Default: Sticky
            return Math.Max(0.0, scrollOffset - AnchorHeight);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
