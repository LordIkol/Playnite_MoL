using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MythosHelper.Settings;

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
                double h = System.Convert.ToDouble(value);
                if (h > 0)
                {
                    StickyOffsetConverter.AnchorHeight = h;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Computes the Y offset for DetailsHeader based on HeaderBehaviorSetting:
    /// - Sticky: Math.Max(0, scrollOffset - anchor)
    /// - BelowBanner: scrollOffset (banner and bar stay fixed together)
    /// - TopFixed: scrollOffset - (anchor + 68) (bar fixed at top of screen above banner)
    /// </summary>
    public class StickyOffsetConverter : IValueConverter
    {
        public static double AnchorHeight { get; set; } = 250.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double scrollOffset = System.Convert.ToDouble(value);
            double anchor = AnchorHeight > 0 ? AnchorHeight : 250.0;

            var behavior = MythosHelperPlugin.Instance?.Settings?.HeaderBehaviorSetting ?? HeaderBehavior.Sticky;
            if (behavior == HeaderBehavior.BelowBanner)
            {
                return scrollOffset;
            }
            else if (behavior == HeaderBehavior.TopFixed)
            {
                return scrollOffset - anchor;
            }
            else
            {
                return Math.Max(0.0, scrollOffset - anchor);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Computes the Y offset for HeaderImagesGrid (Banner):
    /// - BelowBanner: scrollOffset (banner stays fixed at top)
    /// - Sticky / TopFixed: 0.0 (scrolls naturally)
    /// </summary>
    public class BannerOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double scrollOffset = System.Convert.ToDouble(value);
            var behavior = MythosHelperPlugin.Instance?.Settings?.HeaderBehaviorSetting ?? HeaderBehavior.Sticky;
            if (behavior == HeaderBehavior.BelowBanner)
            {
                return scrollOffset;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Computes the Margin for HeaderImagesGrid (Banner).
    /// </summary>
    public class BannerMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Computes the Y offset to keep the header fixed at the very top of the viewport (Y=0).
    /// Input: ScrollViewer.VerticalOffset
    /// Returns: VerticalOffset - AnchorHeight
    /// </summary>
    public class TopFixedOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double scrollOffset = System.Convert.ToDouble(value);
            double anchor = StickyOffsetConverter.AnchorHeight > 0 ? StickyOffsetConverter.AnchorHeight : 250.0;

            return scrollOffset - anchor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
