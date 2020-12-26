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
        Dictionary<String, List<String>> data = new Dictionary<string, List<String>>();
        ErrorProvider errorProvider = new ErrorProvider();

        public MSFS2020InputPanel()
        {
            InitializeComponent();
            PresetFile = Properties.Settings.Default.PresetFileMSFS2020Events;
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
                        var cols = line.Split(':');
                        if (cols.Count() == 2) {
                            GroupKey = cols[0];
                            data.Add(GroupKey, new List<String>());
                        }
                        else
                        {
                            data[GroupKey].Add(cols[0]);
                        }
                    }

                    DeviceComboBox.Items.Clear();
                    EventIdComboBox.Items.Clear();

                    foreach (String key in data.Keys)
                    {
                        DeviceComboBox.Items.Add(key);
                    }
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
                }
            }

            DeviceComboBox.Enabled = isLoaded;
 
        }

        internal void syncFromConfig(InputConfig.MSFS2020EventIdInputAction eventIdInputAction)
        {
            if (eventIdInputAction == null) return;
            if (eventIdInputAction.EventId == null) return; 

            EventIdComboBox.Text = eventIdInputAction.EventId.ToString();

            foreach (String key in data.Keys)
            {
                if (!data[key].Contains(eventIdInputAction.EventId.ToString())) continue;

                DeviceComboBox.SelectedIndexChanged -= DeviceComboBox_SelectedIndexChanged;
                DeviceComboBox.Text = key;
                EventIdComboBox.Items.Clear();

                foreach (String eventId in data[key])
                {
                    EventIdComboBox.Items.Add(eventId);
                }          

                DeviceComboBox.SelectedIndexChanged += DeviceComboBox_SelectedIndexChanged;
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

            foreach (String eventId in data[selectedDevice]) {
                EventIdComboBox.Items.Add(eventId);
            }
        }
    }
}
