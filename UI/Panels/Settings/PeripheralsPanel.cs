﻿using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class PeripheralsPanel : UserControl
    {
        public PeripheralsPanel()
        {
            InitializeComponent();
        }

        public void Init(JoystickManager joystickManager, MidiBoardManager midiBoardManager)
        {
            // Active Joysticks and MidiBoards
            var joysticks = joystickManager.GetJoysticks();
            var midiBoards = midiBoardManager.GetMidiBoards();
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
            // Activation
            Properties.Settings.Default.EnableJoystickSupport = checkBoxJoystickSupport.Checked;
            Properties.Settings.Default.EnableMidiSupport = checkBoxMidiSupport.Checked;
        }
    }
}
