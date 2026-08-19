using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace MythosHelper.Filters
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter is T val ? val : default(T));
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public static class FilterService
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private static bool _isHooked = false;

        public static object GetMainModel()
        {
            try
            {
                var win = Application.Current?.MainWindow;
                if (win?.DataContext != null) return win.DataContext;

                foreach (Window w in Application.Current?.Windows)
                {
                    if (w?.DataContext != null && w.DataContext.GetType().Name.Contains("Main"))
                    {
                        return w.DataContext;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting MainModel");
            }
            return null;
        }

        public static object GetDatabaseFilters()
        {
            var mm = GetMainModel();
            if (mm == null) return null;

            try
            {
                var dbFiltersProp = mm.GetType().GetProperty("DatabaseFilters");
                var dbFilters = dbFiltersProp?.GetValue(mm);
                if (dbFilters != null)
                {
                    HookDatabaseFilters(dbFilters);
                    return dbFilters;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting DatabaseFilters");
            }

            return null;
        }

        public static object GetFilterSettings()
        {
            var mm = GetMainModel();
            if (mm == null) return null;

            try
            {
                var appSettingsProp = mm.GetType().GetProperty("AppSettings");
                var appSettings = appSettingsProp?.GetValue(mm);
                if (appSettings != null)
                {
                    var filterSettingsProp = appSettings.GetType().GetProperty("FilterSettings");
                    return filterSettingsProp?.GetValue(appSettings);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting FilterSettings");
            }

            return null;
        }

        private static void HookDatabaseFilters(object dbFilters)
        {
            if (_isHooked || dbFilters == null) return;

            try
            {
                Action notify = () =>
                {
                    Application.Current?.Dispatcher?.InvokeAsync(() =>
                    {
                        MythosHelperPlugin.Instance?.Settings?.IncrementFilterVersion();
                        RefreshAllChipButtons();
                    });
                };

                if (dbFilters is INotifyPropertyChanged inpcFilter)
                {
                    inpcFilter.PropertyChanged += (s, e) => notify();
                }

                var fs = GetFilterSettings();
                if (fs is INotifyPropertyChanged inpcSettings)
                {
                    inpcSettings.PropertyChanged += (s, e) =>
                    {
                        notify();
                        if (e?.PropertyName == "IsInstalled" || e?.PropertyName == "Favorite")
                        {
                            SelectFirstFilteredGame();
                        }
                    };
                }

                var props = dbFilters.GetType().GetProperties();
                foreach (var p in props)
                {
                    var list = p.GetValue(dbFilters);
                    if (list == null) continue;

                    var eventInfo = list.GetType().GetEvent("SelectionChanged");
                    if (eventInfo != null)
                    {
                        EventHandler handler = (s, e) => notify();
                        eventInfo.AddEventHandler(list, handler);
                    }

                    if (list is INotifyPropertyChanged inpcList)
                    {
                        inpcList.PropertyChanged += (s, e) => notify();
                    }
                }

                _isHooked = true;
                Logger.Info("[MythosHelper] Successfully hooked DatabaseFilters events.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error hooking DatabaseFilters events");
            }
        }

        public static void RefreshAllChipButtons()
        {
            try
            {
                var mainWin = Application.Current?.MainWindow;
                if (mainWin == null) return;

                foreach (var btn in FindVisualChildren<Button>(mainWin))
                {
                    var expr = btn.GetBindingExpression(FrameworkElement.TagProperty);
                    if (expr != null)
                    {
                        expr.UpdateTarget();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error refreshing chip buttons");
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        private static object GetSelectableList(string propertyName)
        {
            var dbFilters = GetDatabaseFilters();
            if (dbFilters == null || string.IsNullOrEmpty(propertyName)) return null;

            var type = dbFilters.GetType();
            string plural = propertyName.EndsWith("s", StringComparison.OrdinalIgnoreCase) ? propertyName : propertyName + "s";
            string ies = propertyName.EndsWith("y", StringComparison.OrdinalIgnoreCase) ? propertyName.Substring(0, propertyName.Length - 1) + "ies" : plural;

            var prop = type.GetProperty(plural) ??
                       type.GetProperty(ies) ??
                       type.GetProperty(propertyName);

            return prop?.GetValue(dbFilters);
        }

        public static PropertyInfo GetFilterSettingsProperty(object filterSettings, string categoryName)
        {
            if (filterSettings == null || string.IsNullOrEmpty(categoryName)) return null;
            var type = filterSettings.GetType();

            string singular = categoryName;
            if (singular.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
                singular = singular.Substring(0, singular.Length - 3) + "y";
            else if (singular.EndsWith("s", StringComparison.OrdinalIgnoreCase) && !singular.Equals("Series", StringComparison.OrdinalIgnoreCase))
                singular = singular.Substring(0, singular.Length - 1);

            return type.GetProperty(singular, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) ??
                   type.GetProperty(categoryName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        public static List<Guid> GetFilterIds(string propertyName)
        {
            try
            {
                var fs = GetFilterSettings();
                var fsProp = GetFilterSettingsProperty(fs, propertyName);
                if (fsProp != null && fs != null)
                {
                    var fsVal = fsProp.GetValue(fs);
                    if (fsVal != null)
                    {
                        var idsProp = fsVal.GetType().GetProperty("Ids");
                        if (idsProp?.GetValue(fsVal) is IEnumerable<Guid> gList)
                        {
                            return gList.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error reading IDs from FilterSettings");
            }

            var list = GetSelectableList(propertyName);
            if (list == null) return null;

            try
            {
                var method = list.GetType().GetMethod("GetSelectedIds");
                if (method != null)
                {
                    var res = method.Invoke(list, null);
                    if (res is IEnumerable<Guid> enumGuids)
                    {
                        return enumGuids.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error calling GetSelectedIds");
            }

            return null;
        }

        public static bool IsInFilter(string propertyName, Guid id)
        {
            if (id == Guid.Empty) return false;
            var ids = GetFilterIds(propertyName);
            return ids != null && ids.Contains(id);
        }

        public static void ClearMetadataFilters()
        {
            try
            {
                string[] categoriesToClear = new[]
                {
                    "Genres",
                    "Categories",
                    "Tags",
                    "Features",
                    "Series",
                    "Platforms",
                    "Publishers",
                    "Developers",
                    "Regions",
                    "Sources",
                    "AgeRatings"
                };

                var fs = GetFilterSettings();

                foreach (var cat in categoriesToClear)
                {
                    var list = GetSelectableList(cat);
                    if (list != null)
                    {
                        var setSelectionMethod = list.GetType().GetMethod("SetSelection", new[] { typeof(IEnumerable<Guid>) });
                        setSelectionMethod?.Invoke(list, new object[] { null });
                    }

                    if (fs != null)
                    {
                        var fsProp = GetFilterSettingsProperty(fs, cat);
                        fsProp?.SetValue(fs, null);
                    }
                }

                MythosHelperPlugin.Instance?.Settings?.IncrementFilterVersion();
                RefreshAllChipButtons();
                SelectFirstFilteredGame();
                Logger.Info("[MythosHelper] Cleared metadata filters while preserving installed, hidden, and match all filters settings.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error clearing metadata filters");
            }
        }

        public static void SelectFirstFilteredGame()
        {
            try
            {
                Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var api = MythosHelperPlugin.Instance?.PlayniteApi;
                        var firstGame = api?.MainView?.FilteredGames?.FirstOrDefault();
                        if (firstGame != null)
                        {
                            api.MainView.SelectGame(firstGame.Id);
                            Logger.Info($"[MythosHelper] Auto-selected first filtered game: {firstGame.Name} ({firstGame.Id})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error selecting first filtered game");
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error dispatching SelectFirstFilteredGame");
            }
        }

        public static void ToggleFilter(object item, string fallbackPropertyName)
        {
            Logger.Info($"[MythosHelper] ToggleFilter called: item={item}, category={fallbackPropertyName}");
            if (item == null) return;

            Guid id = Guid.Empty;
            string propertyName = fallbackPropertyName;

            if (item is DatabaseObject dbObj)
            {
                id = dbObj.Id;
                if (string.IsNullOrEmpty(propertyName))
                {
                    var typeName = item.GetType().Name;
                    if (typeName.Contains("Genre")) propertyName = "Genres";
                    else if (typeName.Contains("Feature")) propertyName = "Features";
                    else if (typeName.Contains("Category")) propertyName = "Categories";
                    else if (typeName.Contains("Tag")) propertyName = "Tags";
                    else if (typeName.Contains("Series")) propertyName = "Series";
                }
            }
            else if (item is Guid g)
            {
                id = g;
            }
            else
            {
                var idProp = item.GetType().GetProperty("Id");
                if (idProp != null && idProp.GetValue(item) is Guid gid)
                {
                    id = gid;
                }
            }

            if (id == Guid.Empty || string.IsNullOrEmpty(propertyName))
            {
                Logger.Warn($"[MythosHelper] ToggleFilter: Invalid id={id} or category={propertyName}");
                return;
            }

            try
            {
                var currentIds = GetFilterIds(propertyName) ?? new List<Guid>();

                if (currentIds.Contains(id))
                {
                    currentIds.Remove(id);
                    Logger.Info($"[MythosHelper] Removed {id} from {propertyName}");
                }
                else
                {
                    currentIds.Add(id);
                    Logger.Info($"[MythosHelper] Added {id} to {propertyName}");
                }

                // 1. Update DatabaseFilters (UI SelectableDbItemList in sidebar filter panel)
                var list = GetSelectableList(propertyName);
                if (list != null)
                {
                    var setSelectionMethod = list.GetType().GetMethod("SetSelection", new[] { typeof(IEnumerable<Guid>) });
                    setSelectionMethod?.Invoke(list, new object[] { currentIds.Count > 0 ? (IEnumerable<Guid>)currentIds : null });
                }

                // 2. Update FilterSettings (Core filtering engine)
                var fs = GetFilterSettings();
                var fsProp = GetFilterSettingsProperty(fs, propertyName);
                if (fsProp != null && fs != null)
                {
                    object newPropVal = null;
                    if (currentIds.Count > 0)
                    {
                        var propType = fsProp.PropertyType;
                        var ctor = propType.GetConstructor(new[] { typeof(List<Guid>) });
                        if (ctor != null)
                        {
                            newPropVal = ctor.Invoke(new object[] { currentIds });
                        }
                        else
                        {
                            newPropVal = Activator.CreateInstance(propType);
                            var idsSetter = propType.GetProperty("Ids");
                            idsSetter?.SetValue(newPropVal, currentIds);
                        }
                    }
                    fsProp.SetValue(fs, newPropVal);
                }

                MythosHelperPlugin.Instance?.Settings?.IncrementFilterVersion();
                RefreshAllChipButtons();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error setting selection on {propertyName}");
            }
        }
    }
}
