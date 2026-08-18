using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MythosHelper.Converters
{
    /// <summary>
    /// Subtracts the ConverterParameter from the input value.
    /// Input: double, Parameter: double (as string), Output: value - parameter
    /// </summary>
    public class SubtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double input = System.Convert.ToDouble(value);
            double subtract = 0.0;

            if (parameter != null)
                double.TryParse(parameter.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out subtract);

            return input - subtract;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Multiplies the input value by the ConverterParameter.
    /// Input: double, Parameter: double (as string), Output: value * parameter
    /// </summary>
    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double input = System.Convert.ToDouble(value);
            double factor = 1.0;

            if (parameter != null)
                double.TryParse(parameter.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out factor);

            return input * factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Clamps the input value between min and max.
    /// Input: double, Parameter: "min,max" (string), Output: Clamp(value, min, max)
    /// </summary>
    public class MathClampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return 0.0;

            double input = System.Convert.ToDouble(value);

            if (parameter is string paramStr)
            {
                var parts = paramStr.Split(',');
                if (parts.Length == 2)
                {
                    if (double.TryParse(parts[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double min) &&
                        double.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double max))
                    {
                        return Math.Max(min, Math.Min(max, input));
                    }
                }
            }

            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Converts a collection or IEnumerable to Visibility.Visible if non-null and Count > 0, otherwise Visibility.Collapsed.
    /// </summary>
    public class HasItemsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return Visibility.Collapsed;

            if (value is System.Collections.ICollection col)
                return col.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

            if (value is System.Collections.IEnumerable enumVal)
            {
                var enumerator = enumVal.GetEnumerator();
                return enumerator.MoveNext() ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
