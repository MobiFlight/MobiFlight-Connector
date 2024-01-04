using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class PeripheralsPanel : UserControl
    {
        private MidiBoardManager MidiMgr;
        private JoystickManager JoystickMgr;        

        public PeripheralsPanel()
        {
            InitializeComponent();
        }

        public void Init(JoystickManager joystickManager, MidiBoardManager midiBoardManager)
        {
            JoystickMgr = joystickManager;
            MidiMgr = midiBoardManager;
        }

        public void LoadSettings()
        {
            // Activation
            checkBoxJoystickSupport.Checked = Properties.Settings.Default.EnableJoystickSupport;
            checkBoxMidiSupport.Checked = Properties.Settings.Default.EnableMidiSupport;

            // Joysticks
            JoystickMgr.GetJoysticks().ForEach(js => { listBoxJoysticks.Items.Add(js.Name, true); });
            JoystickMgr.GetExcludedJoysticks().ForEach(js => { listBoxJoysticks.Items.Add(js.Name, false); });

            // MidiBoards
            MidiMgr.GetMidiBoards().ForEach(mb => { listBoxMidiBoards.Items.Add(mb.Name, true); });
            MidiMgr.GetExcludedMidiBoards().ForEach(mb => { listBoxMidiBoards.Items.Add(mb.Name, false); });
        }

        private bool UpdateExclusionList(List<string> exclusionList, CheckedListBox listBox)
        {
            bool listChanged = false;
            foreach (string item in listBox.Items)
            {
                if (!listBox.CheckedItems.Contains(item) && !exclusionList.Contains(item))
                {
                    // Add to exclusion List
                    exclusionList.Add(item);
                    listChanged = true;
                }
                else if (listBox.CheckedItems.Contains(item) && exclusionList.Contains(item))
                {
                    // Remove from exclusion List
                    exclusionList.Remove(item);
                    listChanged = true;
                }
            }
            return listChanged;
        }

        public void SaveSettings()
        {
            List<string> excludedBoards = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.ExcludedMidiBoards);
            List<string> excludedJoysticks = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.ExcludedJoysticks);

            bool excludedJoysticksChanged = UpdateExclusionList(excludedJoysticks, listBoxJoysticks);
            bool excludedBoardsChanged = UpdateExclusionList(excludedBoards, listBoxMidiBoards);

            bool enableJoysticksChanged = Properties.Settings.Default.EnableJoystickSupport != checkBoxJoystickSupport.Checked;
            bool enableMidiChanged = Properties.Settings.Default.EnableMidiSupport != checkBoxMidiSupport.Checked;

            Properties.Settings.Default.ExcludedMidiBoards = JsonConvert.SerializeObject(excludedBoards);
            Properties.Settings.Default.ExcludedJoysticks = JsonConvert.SerializeObject(excludedJoysticks);
            Properties.Settings.Default.EnableJoystickSupport = checkBoxJoystickSupport.Checked;
            Properties.Settings.Default.EnableMidiSupport = checkBoxMidiSupport.Checked;

            // Update joystick management
            if (!checkBoxJoystickSupport.Checked && enableJoysticksChanged)
            {
                JoystickMgr.Shutdown();
            }
            else if (checkBoxJoystickSupport.Checked && (enableJoysticksChanged || excludedJoysticksChanged))
            {
                JoystickMgr.Shutdown();
                JoystickMgr.Connect();
            }

            // Update MIDI management
            if (!checkBoxMidiSupport.Checked && enableMidiChanged)
            {
                MidiMgr.Shutdown();
            }
            else if (checkBoxMidiSupport.Checked && (enableMidiChanged || excludedBoardsChanged))
            {
                MidiMgr.Shutdown();
                MidiMgr.Connect();
            }
        }
    }
}
