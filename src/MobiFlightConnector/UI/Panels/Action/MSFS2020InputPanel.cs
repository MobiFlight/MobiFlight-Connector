using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class MSFS2020InputPanel : UserControl
    {
        public String PresetFile { get; set; }
        public String PresetFileUser { get; set; }
        Dictionary<String, List<String>> data = new Dictionary<string, List<String>>();
        ErrorProvider errorProvider = new ErrorProvider();

        public MSFS2020InputPanel()
        {
            InitializeComponent();
            PresetFile = Properties.Settings.Default.PresetFileMSFS2020Events;
            PresetFileUser = Properties.Settings.Default.PresetFileMSFS2020EventsUser;
            _loadPresets();
        }

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(i18n._tr("uiMessageConfigWizard_PresetsNotFound"), i18n._tr("Hint"));
            }
            else
            {

                try
                {
                    string[] lines = System.IO.File.ReadAllLines(PresetFile);
                    string GroupKey = "Default";
                    data.Add(GroupKey, new List<String>());

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("//")) continue;
                        var cols = line.Split(':');
                        if (cols.Count() == 2) {
                            GroupKey = cols[0];
                            if (data.ContainsKey(GroupKey)) continue;

                            data.Add(GroupKey, new List<String>());
                        }
                        else
                        {
                            data[GroupKey].Add(cols[0]);
                        }
                    }

                    if (data["Default"].Count == 0) data.Remove("Default");


                    if (System.IO.File.Exists(PresetFileUser))
                    {
                        Log.Instance.log("User events found.", LogSeverity.Debug);
                        lines = System.IO.File.ReadAllLines(PresetFileUser);
                        GroupKey = "User";
                        data.Add(GroupKey, new List<String>());

                        foreach (string line in lines)
                        {
                            if (line.StartsWith("//")) continue;
                            var cols = line.Split(':');
                            if (cols.Count() == 2)
                            {
                                GroupKey = "User: " + cols[0];
                                if (data.ContainsKey(GroupKey)) continue;

                                data.Add(GroupKey, new List<String>());
                            }
                            else
                            {
                                data[GroupKey].Add(cols[0]);
                            }
                        }

                        if (data["User"].Count == 0) data.Remove("User");
                    } else
                    {
                        Log.Instance.log("No user events found.", LogSeverity.Debug);
                    }

                    GroupComboBox.Items.Clear();
                    EventIdComboBox.Items.Clear();

                    foreach (String key in data.Keys)
                    {
                        GroupComboBox.Items.Add(key);
                    }
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
                }
            }

            GroupComboBox.Enabled = isLoaded;
 
        }

        internal void syncFromConfig(InputConfig.MSFS2020EventIdInputAction eventIdInputAction)
        {
            if (eventIdInputAction == null) return;
            if (eventIdInputAction.EventId == null) return; 

            EventIdComboBox.Text = eventIdInputAction.EventId.ToString();

            foreach (String key in data.Keys)
            {
                if (!data[key].Contains(eventIdInputAction.EventId.ToString())) continue;

                GroupComboBox.SelectedIndexChanged -= DeviceComboBox_SelectedIndexChanged;
                GroupComboBox.Text = key;
                EventIdComboBox.Items.Clear();

                foreach (String eventId in data[key])
                {
                    EventIdComboBox.Items.Add(eventId);
                }          

                GroupComboBox.SelectedIndexChanged += DeviceComboBox_SelectedIndexChanged;
            }
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.MSFS2020EventIdInputAction result = new InputConfig.MSFS2020EventIdInputAction();
            result.EventId = EventIdComboBox.Text as String;
                return result;
        }

        private void DeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selectedDevice = (sender as ComboBox).SelectedItem as String;
            if (!data.ContainsKey(selectedDevice)) return;

            EventIdComboBox.Items.Clear();
            EventIdComboBox.SelectedText = "";

            foreach (String eventId in data[selectedDevice]) {
                EventIdComboBox.Items.Add(eventId);
            }

            EventIdComboBox.SelectedIndex = 0;
        }
    }
}
