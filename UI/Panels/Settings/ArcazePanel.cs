using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleSolutions.Usb;
using System.Xml.Serialization;
using MobiFlight.Base;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class ArcazePanel : UserControl
    {
        List <ArcazeModuleSettings> moduleSettings;
        ArcazeCache arcazeCache;
        bool IgnoreArcazeModuleSettingsChangeEvents = false;
        public bool ModuleConfigChanged { get; set; }

        public ArcazePanel()
        {
            InitializeComponent();
            ArcazePanelSettingsPanel.Visible = Properties.Settings.Default.ArcazeSupportEnabled;
            ArcazeNoModulesFoundPanel.Visible = false;
        }

        public void Init (ArcazeCache arcazeCache)
        {
            this.arcazeCache = arcazeCache;
        }

        private void InitArcazeModuleTreeView(ArcazeCache arcazeCache)
        {
            ArcazeModuleTreeView.Nodes.Clear();
            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                ArcazeListItem arcazeItem = new ArcazeListItem();
                arcazeItem.Text = module.Name + "/ " + module.Serial;
                arcazeItem.Value = module as ArcazeModuleInfo;

                TreeNode NewNode = new TreeNode();
                NewNode.Text = module.Name + "/ " + module.Serial;
                NewNode.Tag = module as ArcazeModuleInfo;
                NewNode.SelectedImageKey = NewNode.ImageKey = "module-arcaze";
                if (moduleSettings.Find(item => item.serial == module.Serial) == null)
                {
                    NewNode.SelectedImageKey = NewNode.ImageKey = "new-arcaze";
                }

                ArcazeModuleTreeView.Nodes.Add(NewNode);
            }

            ArcazeNoModulesFoundPanel.Visible = (ArcazeModuleTreeView.Nodes.Count == 0);
            ArcazePanelSettingsPanel.Visible = (ArcazeModuleTreeView.Nodes.Count > 0);
        }

        private void numModulesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (ArcazeModuleTreeView.SelectedNode != null)
            {
                _syncToModuleSettings((ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo).Serial);
            }
        }


        private void ArcazeModuleTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
#if ARCAZE
            if (e.Node == null) return;
            TreeNode parentNode = e.Node;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;
            arcazeModuleSettingsGroupBox.Visible = true;
            _syncFromModuleSettings((parentNode.Tag as ArcazeModuleInfo).Serial);
#endif
        }

        /// <summary>
        /// Callback if extension type is changed for a selected Arcaze Board
        /// Show the correct options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arcazeModuleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
#if ARCAZE
            numModulesLabel.Visible = (sender as ComboBox).SelectedIndex != 0;
            numModulesNumericUpDown.Visible = (sender as ComboBox).SelectedIndex != 0;

            bool brightnessVisible = (sender as ComboBox).SelectedIndex != 0 && ((sender as ComboBox).SelectedItem.ToString() != ArcazeCommand.ExtModuleType.LedDriver2.ToString());
            globalBrightnessLabel.Visible = brightnessVisible;
            globalBrightnessTrackBar.Visible = brightnessVisible;

            // check if the extension is compatible
            // but only if not the first item (please select) == 0
            // or none selected yet == -1
            if (ArcazeModuleTreeView.SelectedNode == null) return;
            
            // IModuleInfo devInfo = (IModuleInfo) ((arcazeSerialComboBox.SelectedItem as ArcazeListItem).Value);
            IModuleInfo devInfo = (IModuleInfo)(ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo);
            
            
            string errMessage = null;
            
            switch ((sender as ComboBox).SelectedItem.ToString()) {
                case "DisplayDriver":
                    // check for 5.30
                    break;
                case "LedDriver2":
                    // check for v.5.54
                    break;             
                case "LedDriver3":
                    // check for v.5.55
                    break;
            }

            if (errMessage != null)
            {
                MessageBox.Show(i18n._tr(errMessage));
            }

            _syncToModuleSettings((ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo).Serial);
#endif
        }

        /// <summary>
        /// Save the module settings for the different Arcaze Boards
        /// </summary>
        /// <param name="serial"></param>
        private void _syncToModuleSettings(string serial)
        {
#if ARCAZE
            if (IgnoreArcazeModuleSettingsChangeEvents) return;

            ArcazeModuleSettings settingsToSave = null;
            if (SerialNumber.IsRawSerial(serial))
                serial = SerialNumber.ExtractSerial(serial);

            foreach (ArcazeModuleSettings settings in moduleSettings)
            {
                if (settings.serial != serial) continue;

                settingsToSave = settings;
            }

            bool hasChanged = false;

            if (settingsToSave == null)
            {
                settingsToSave = new ArcazeModuleSettings() { serial = serial };
                moduleSettings.Add(settingsToSave);
            }

            settingsToSave.type = settingsToSave.stringToExtModuleType(arcazeModuleTypeComboBox.SelectedItem.ToString());
            settingsToSave.numModules = (byte) numModulesNumericUpDown.Value;
            settingsToSave.globalBrightness = (byte) (255 * ((globalBrightnessTrackBar.Value) / (double) (globalBrightnessTrackBar.Maximum)));

            if (settingsToSave.HasChanged && ArcazeModuleTreeView.SelectedNode != null)
            {
                ModuleConfigChanged = true;
                if ((ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo).Serial == serial)
                {
                    ArcazeModuleTreeView.SelectedNode.SelectedImageKey = ArcazeModuleTreeView.SelectedNode.ImageKey = "Changed-arcaze";
                }
            }
#endif
        }

        /// <summary>
        /// Restore the arcaze settings
        /// </summary>
        /// <param name="serial"></param>
        private void _syncFromModuleSettings(string serial) {
            if (moduleSettings == null) return;
            if (IgnoreArcazeModuleSettingsChangeEvents) return;

            bool moduleSettingsAvailable = false;
            IgnoreArcazeModuleSettingsChangeEvents = true;

            foreach (ArcazeModuleSettings settings in moduleSettings)
            {
                if (SerialNumber.IsRawSerial(serial))
                    serial = SerialNumber.ExtractSerial(serial);

                if (settings.serial != serial) continue;

                arcazeModuleTypeComboBox.SelectedItem = settings.type.ToString();
                numModulesNumericUpDown.Value = settings.numModules;
                int range = globalBrightnessTrackBar.Maximum - globalBrightnessTrackBar.Minimum;

                globalBrightnessTrackBar.Value = (int) ((settings.globalBrightness / (double) 255) *  (range)) + globalBrightnessTrackBar.Minimum;
                moduleSettingsAvailable = true;
                break;
            }

            if (!moduleSettingsAvailable)
            {
                arcazeModuleTypeComboBox.SelectedItem = ArcazeCommand.ExtModuleType.InternalIo.ToString();
                numModulesNumericUpDown.Value = 1;
                
                IgnoreArcazeModuleSettingsChangeEvents = false;
                _syncToModuleSettings(serial);
            }
            IgnoreArcazeModuleSettingsChangeEvents = false;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ArcazeSupportEnabled = ArcazeSupportEnabledCheckBox.Checked;

            if (!ArcazeSupportEnabledCheckBox.Checked)
            {
                return;
            }

            try
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(List<ArcazeModuleSettings>));                
                System.IO.StringWriter w = new System.IO.StringWriter();
                SerializerObj.Serialize(w, moduleSettings);
                Properties.Settings.Default.ModuleSettings = w.ToString();
                Log.Instance.log($"Serialized Arcaze extension module settings: {Properties.Settings.Default.ModuleSettings}.", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Exception on serializing Arcaze extension module settings: {ex.Message}", LogSeverity.Error);
            }

            if (ArcazeModuleTreeView.SelectedNode != null)
            {
                _syncToModuleSettings((ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo).Serial);
            }
        }

        public void LoadSettings()
        {
            ArcazeSupportEnabledCheckBox.CheckedChanged -= ArcazeSupportEnabledCheckBox_CheckedChanged;
            ArcazeSupportEnabledCheckBox.Checked = Properties.Settings.Default.ArcazeSupportEnabled;
            ArcazeSupportEnabledCheckBox.CheckedChanged += ArcazeSupportEnabledCheckBox_CheckedChanged;

            if (!ArcazeSupportEnabledCheckBox.Checked)
            {   
                return;
            }

            arcazeModuleTypeComboBox.Items.Clear();
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.InternalIo.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.DisplayDriver.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.LedDriver2.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.LedDriver3.ToString());
            arcazeModuleTypeComboBox.SelectedIndex = 0;

            moduleSettings = new List<ArcazeModuleSettings>();
            if ("" != Properties.Settings.Default.ModuleSettings)
            {
                try
                {                
                    XmlSerializer SerializerObj = new XmlSerializer(typeof(List<ArcazeModuleSettings>));
                    System.IO.StringReader w = new System.IO.StringReader(Properties.Settings.Default.ModuleSettings);
                    moduleSettings = (List<ArcazeModuleSettings>)SerializerObj.Deserialize(w);
                    string test = w.ToString();
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Exception on deserializing Arcaze extension module settings: {ex.Message}", LogSeverity.Error);
                }
            }

            InitArcazeModuleTreeView(arcazeCache);
        }

        private void ArcazeSupportEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                InitArcazeModuleTreeView(arcazeCache);
            } else
            {
                ArcazePanelSettingsPanel.Visible = false;
                ArcazeNoModulesFoundPanel.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Application.Restart();
            // Not sure if we need this next line
            Environment.Exit(0);
        }
    }

    public class ArcazeListItem
    {
        public string Text { get; set; }
        public ArcazeModuleInfo Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
