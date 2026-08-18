using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using MythosHelper.Converters;
using MythosHelper.Settings;
using Playnite.SDK;
using Playnite.SDK.Plugins;

namespace MythosHelper
{
    public class MythosHelperPlugin : GenericPlugin
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        public static MythosHelperPlugin Instance { get; private set; }

        public override Guid Id { get; } = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        public MythosHelperSettings Settings { get; private set; }

        public MythosHelperPlugin(IPlayniteAPI api) : base(api)
        {
            Instance = this;
            Settings = new MythosHelperSettings(this);

            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };

            AddConvertersSupport(new AddConvertersSupportArgs
            {
                SourceName = "MythosHelper_Loki",
                Converters = new List<IValueConverter>
                {
                    new StickyOffsetConverter(),
                    new SetAnchorHeightConverter(),
                    new SubtractConverter(),
                    new MultiplyConverter(),
                    new MathClampConverter()
                }
            });
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return Settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new MythosHelperSettingsView();
        }
    }
}
