using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Data;

namespace MythosHelper.Settings
{
    public enum HeaderBehavior
    {
        Sticky,
        TopFixed,
        BelowBanner
    }

    public class MythosHelperSettings : ObservableObject, ISettings
    {
        private readonly MythosHelperPlugin _plugin;

        // --- Layout ---

        private HeaderBehavior _headerBehavior = HeaderBehavior.Sticky;
        public HeaderBehavior HeaderBehaviorSetting
        {
            get => _headerBehavior;
            set
            {
                SetValue(ref _headerBehavior, value);
                OnPropertyChanged(nameof(HeaderMode));
            }
        }

        [DontSerialize]
        public string HeaderMode => _headerBehavior.ToString();

        private double _columnRatio = 0.7;
        /// <summary>Left column width proportion (0.5 to 0.9). Right column is the remainder.</summary>
        public double ColumnRatio
        {
            get => _columnRatio;
            set => SetValue(ref _columnRatio, value);
        }

        // --- Header Metadata Visibility ---

        private bool _showLastPlayed = true;
        public bool ShowLastPlayed
        {
            get => _showLastPlayed;
            set => SetValue(ref _showLastPlayed, value);
        }

        private bool _showTimePlayed = true;
        public bool ShowTimePlayed
        {
            get => _showTimePlayed;
            set => SetValue(ref _showTimePlayed, value);
        }

        private bool _showInstallSize = true;
        public bool ShowInstallSize
        {
            get => _showInstallSize;
            set => SetValue(ref _showInstallSize, value);
        }

        private bool _showInstallFolder = true;
        public bool ShowInstallFolder
        {
            get => _showInstallFolder;
            set => SetValue(ref _showInstallFolder, value);
        }

        private bool _showCompletionStatus = true;
        public bool ShowCompletionStatus
        {
            get => _showCompletionStatus;
            set => SetValue(ref _showCompletionStatus, value);
        }

        // --- Video ---

        private double _videoPlayerHeight = 300;
        /// <summary>Video player height in pixels (100 to 600).</summary>
        public double VideoPlayerHeight
        {
            get => _videoPlayerHeight;
            set => SetValue(ref _videoPlayerHeight, value);
        }

        // --- Serialization ---

        // Backup for cancel support
        private MythosHelperSettings _editingClone;

        public MythosHelperSettings() { }

        public MythosHelperSettings(MythosHelperPlugin plugin)
        {
            _plugin = plugin;
            var saved = plugin.LoadPluginSettings<MythosHelperSettings>();
            if (saved != null)
            {
                HeaderBehaviorSetting = saved.HeaderBehaviorSetting;
                ColumnRatio = saved.ColumnRatio;
                ShowLastPlayed = saved.ShowLastPlayed;
                ShowTimePlayed = saved.ShowTimePlayed;
                ShowInstallSize = saved.ShowInstallSize;
                ShowInstallFolder = saved.ShowInstallFolder;
                ShowCompletionStatus = saved.ShowCompletionStatus;
                VideoPlayerHeight = saved.VideoPlayerHeight;
            }
        }

        public void BeginEdit()
        {
            _editingClone = Serialization.GetClone(this);
        }

        public void CancelEdit()
        {
            HeaderBehaviorSetting = _editingClone.HeaderBehaviorSetting;
            ColumnRatio = _editingClone.ColumnRatio;
            ShowLastPlayed = _editingClone.ShowLastPlayed;
            ShowTimePlayed = _editingClone.ShowTimePlayed;
            ShowInstallSize = _editingClone.ShowInstallSize;
            ShowInstallFolder = _editingClone.ShowInstallFolder;
            ShowCompletionStatus = _editingClone.ShowCompletionStatus;
            VideoPlayerHeight = _editingClone.VideoPlayerHeight;
        }

        public void EndEdit()
        {
            _plugin.SavePluginSettings(this);
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();

            if (ColumnRatio < 0.3 || ColumnRatio > 0.95)
                errors.Add("Column ratio must be between 0.3 and 0.95");

            if (VideoPlayerHeight < 100 || VideoPlayerHeight > 800)
                errors.Add("Video player height must be between 100 and 800");

            return errors.Count == 0;
        }
    }
}
