using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using MythosHelper.Filters;
using Playnite.SDK.Models;

namespace MythosHelper.Converters
{
    public class ToggleFilterCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string categoryName = parameter?.ToString();
            return new RelayCommand<object>(item =>
            {
                FilterService.ToggleFilter(item ?? value, categoryName);
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ClearMetadataFiltersCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new RelayCommand<object>(_ =>
            {
                FilterService.ClearMetadataFilters();
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class IsFilterActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return targetType == typeof(bool) ? (object)false : "False";
            string categoryName = parameter?.ToString();
            if (string.IsNullOrEmpty(categoryName)) return targetType == typeof(bool) ? (object)false : "False";

            Guid id = Guid.Empty;

            if (value is System.Windows.FrameworkElement fe)
            {
                if (fe.DataContext is DatabaseObject db) id = db.Id;
                else if (fe.DataContext is Guid g) id = g;
            }
            else if (value is DatabaseObject dbObj)
            {
                id = dbObj.Id;
            }
            else if (value is Guid gid)
            {
                id = gid;
            }

            if (id == Guid.Empty) return targetType == typeof(bool) ? (object)false : "False";
            bool active = FilterService.IsInFilter(categoryName, id);

            if (targetType == typeof(bool))
            {
                return active;
            }
            return active ? "True" : "False";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
