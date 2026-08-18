using System.Windows.Controls;

namespace MythosHelper.Settings
{
    public partial class MythosHelperSettingsView : UserControl
    {
        public System.Collections.Generic.IEnumerable<HeaderBehavior> HeaderBehaviorOptions
        {
            get { return (HeaderBehavior[])System.Enum.GetValues(typeof(HeaderBehavior)); }
        }

        public MythosHelperSettingsView()
        {
            InitializeComponent();
        }
    }
}
