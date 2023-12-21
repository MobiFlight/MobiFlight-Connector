using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class JoystickMidiPanel : UserControl
    {
        public JoystickMidiPanel()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            // Activation
            checkBoxJoystickSupport.Checked = Properties.Settings.Default.DisableJoystickSupport;
            checkBoxMidiSupport.Checked = Properties.Settings.Default.DisableMidiSupport;
        }

        public void SaveSettings()
        {
            // Activation
            Properties.Settings.Default.DisableJoystickSupport = checkBoxJoystickSupport.Checked;
            Properties.Settings.Default.DisableMidiSupport = checkBoxMidiSupport.Checked;
        }
    }
}
