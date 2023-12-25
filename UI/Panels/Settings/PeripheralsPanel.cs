using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class PeripheralsPanel : UserControl
    {
        private MidiBoardManager MidiBoardMgr;
        private JoystickManager JoystickMgr;

        public PeripheralsPanel()
        {
            InitializeComponent();
        }

        public void Init(JoystickManager joystickManager, MidiBoardManager midiBoardManager)
        {
            JoystickMgr = joystickManager;
            MidiBoardMgr = midiBoardManager;

            // Active Joysticks and MidiBoards
            var joysticks = JoystickMgr.GetJoysticks();
            var midiBoards = MidiBoardMgr.GetMidiBoards();
            listBoxJoysticks.DataSource = joysticks.Select(x => x.Name).ToList();
            listBoxMidiBoards.DataSource = midiBoards.Select(x => x.Name).ToList();
        }

        public void LoadSettings()
        {
            // Activation
            checkBoxJoystickSupport.Checked = Properties.Settings.Default.EnableJoystickSupport;
            checkBoxMidiSupport.Checked = Properties.Settings.Default.EnableMidiSupport;
        }

        public void SaveSettings()
        {
            // Joystick activation changed
            if (Properties.Settings.Default.EnableJoystickSupport != checkBoxJoystickSupport.Checked)
            {
                if (checkBoxJoystickSupport.Checked) 
                {
                    JoystickMgr.Connect();
                }
                else 
                {
                    JoystickMgr.Shutdown();
                }
                Properties.Settings.Default.EnableJoystickSupport = checkBoxJoystickSupport.Checked;
            }

            // MIDI activation changed
            if (Properties.Settings.Default.EnableMidiSupport != checkBoxMidiSupport.Checked)
            {
                if (checkBoxMidiSupport.Checked)
                {
                    MidiBoardMgr.Connect();
                }
                else
                {
                    MidiBoardMgr.Shutdown();
                }
                Properties.Settings.Default.EnableMidiSupport = checkBoxMidiSupport.Checked;
            }
        }
    }
}
