using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using SimpleSolutions.Usb;
using MobiFlight;

namespace ArcazeUSB
{
    public partial class SettingsDialog : Form
    {
        List <ArcazeModuleSettings> moduleSettings;
        ExecutionManager execManager;
        int lastSelectedIndex = -1;

        public SettingsDialog()
        {
            Init();
        }

        public SettingsDialog(ExecutionManager execManager)
        {
            this.execManager = execManager;
            Init();

            ArcazeCache arcazeCache = execManager.getModuleCache();
            
            // init the drop down
            arcazeSerialComboBox.Items.Clear();
            arcazeSerialComboBox.Items.Add(MainForm._tr("Please_Choose"));
                        
            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                ArcazeListItem arcazeItem = new ArcazeListItem();
                arcazeItem.Text = module.Name + "/ " + module.Serial;
                arcazeItem.Value = module as ArcazeModuleInfo;

                arcazeSerialComboBox.Items.Add( arcazeItem );
            }

            arcazeSerialComboBox.SelectedIndex = 0;
        }

        private void Init()
        {
            InitializeComponent();

            arcazeModuleTypeComboBox.Items.Clear();
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.InternalIo);
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.DisplayDriver.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.LedDriver2.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.LedDriver3.ToString());
            arcazeModuleTypeComboBox.SelectedIndex = 0;

            // initialize mftreeviewimagelist
            mfTreeViewImageList.Images.Add("SERVO", ArcazeUSB.Properties.Resources.cd);
            mfTreeViewImageList.Images.Add("STEPPER", ArcazeUSB.Properties.Resources.dvd);
            mfTreeViewImageList.Images.Add("OUTPUT", ArcazeUSB.Properties.Resources.lightbulb_on);
            mfTreeViewImageList.Images.Add("LEDMODULE", ArcazeUSB.Properties.Resources.sound);
            //mfModulesTreeView.ImageList = mfTreeViewImageList;
            loadSettings();

#if MOBIFLIGHT
            // do nothing
#else
            tabControl1.TabPages.Remove(mobiFlightTabPage);
#endif
        }

        private void loadSettings ()
        {            
            // Recent Files max count
            recentFilesNumericUpDown.Value = Properties.Settings.Default.RecentFilesMaxCount;

            // TestMode speed
            // (1s) 0 - 4 (50ms)
            testModeSpeedTrackBar.Value = 0;
            if (Properties.Settings.Default.TestTimerInterval == 500) testModeSpeedTrackBar.Value = 1;
            else if (Properties.Settings.Default.TestTimerInterval == 250) testModeSpeedTrackBar.Value = 2;
            else if (Properties.Settings.Default.TestTimerInterval == 125) testModeSpeedTrackBar.Value = 3;
            else if (Properties.Settings.Default.TestTimerInterval == 50) testModeSpeedTrackBar.Value = 4;

            // FSUIPC poll interval
            fsuipcPollIntervalTrackBar.Value = (int) Math.Floor(Properties.Settings.Default.PollInterval / 50.0);

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
                catch (Exception e)
                {
                }
            }

            loadMobiFlightSettings();
        }

        private void loadMobiFlightSettings()
        {
#if MOBIFLIGHT
            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();

            mfModulesTreeView.Nodes.Clear();
            foreach (MobiFlight.MobiFlightModule module in mobiflightCache.GetModules()) {
                module.GetInfo();
                TreeNode node = new TreeNode(module.Name);
                node.Tag = module;
                mfModulesTreeView.Nodes.Add(node);
                foreach (IConnectedDevice device in module.GetConnectedDevices())
                {
                    TreeNode deviceNode = new TreeNode(device.Name);
                    deviceNode.Tag = device;
                    switch (device.Type) {
                        case "LEDMODULE":
                            deviceNode.ImageKey = "LEDMODULE";                            
                            break;

                        case "STEPPER":
                            deviceNode.ImageKey = "STEPPER";
                            break;

                        case "OUTPUT":
                            deviceNode.ImageKey = "OUTPUT";
                            break;

                        case "SERVO":
                            deviceNode.ImageKey = "SERVO";
                            break;
                    }
                    node.Nodes.Add(deviceNode);
                }
            }
#endif
        }


        private void saveSettings()
        {            
            if (testModeSpeedTrackBar.Value == 0) Properties.Settings.Default.TestTimerInterval = 1000;
            else if (testModeSpeedTrackBar.Value == 1) Properties.Settings.Default.TestTimerInterval = 500;
            else if (testModeSpeedTrackBar.Value == 2) Properties.Settings.Default.TestTimerInterval = 250;
            else if (testModeSpeedTrackBar.Value == 3) Properties.Settings.Default.TestTimerInterval = 125;
            else Properties.Settings.Default.TestTimerInterval = 50;

            // Recent Files max count
            Properties.Settings.Default.RecentFilesMaxCount = (int) recentFilesNumericUpDown.Value;
            // FSUIPC poll interval
            Properties.Settings.Default.PollInterval = (int) (fsuipcPollIntervalTrackBar.Value * 50);

            try
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(List<ArcazeModuleSettings>));                
                System.IO.StringWriter w = new System.IO.StringWriter();
                SerializerObj.Serialize(w, moduleSettings);
                Properties.Settings.Default.ModuleSettings = w.ToString();                
            }
            catch (Exception e)
            {
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                return;
            }
            DialogResult = DialogResult.OK;
            if (0 < arcazeSerialComboBox.SelectedIndex)
            {
                _syncToModuleSettings(arcazeSerialComboBox.SelectedItem.ToString());
            }
            saveSettings();
        }

        private void _syncToModuleSettings(string serial) {
            ArcazeModuleSettings settingsToSave = null;
            if (serial.Contains("/"))
                serial = serial.Split('/')[1].Trim();

            foreach (ArcazeModuleSettings settings in moduleSettings)
            {
                if (settings.serial != serial) continue;

                settingsToSave = settings;
            }

            if (settingsToSave == null)
            {
                settingsToSave = new ArcazeModuleSettings() { serial = serial };
                moduleSettings.Add(settingsToSave);
            }

            settingsToSave.type = settingsToSave.stringToExtModuleType(arcazeModuleTypeComboBox.SelectedItem.ToString());
            settingsToSave.numModules = (byte) numModulesNumericUpDown.Value;
            settingsToSave.globalBrightness = (byte) (255 * ((globalBrightnessTrackBar.Value) / (double) (globalBrightnessTrackBar.Maximum)));
        }

        private void _syncFromModuleSettings(string serial) {
            if (moduleSettings == null) return;

            foreach (ArcazeModuleSettings settings in moduleSettings)
            {
                if (serial.Contains("/"))
                    serial = serial.Split('/')[1].Trim();

                if (settings.serial != serial) continue;

                arcazeModuleTypeComboBox.SelectedItem = settings.type.ToString();
                numModulesNumericUpDown.Value = settings.numModules;
                int range = globalBrightnessTrackBar.Maximum - globalBrightnessTrackBar.Minimum;

                globalBrightnessTrackBar.Value = (int) ((settings.globalBrightness / (double) 255) *  (range)) + globalBrightnessTrackBar.Minimum;
            }
        }

        private void arcazeSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            arcazeModuleSettingsGroupBox.Visible =  cb.SelectedIndex > 0;

            // store settings of last item
            if (lastSelectedIndex > 0)
            {
                _syncToModuleSettings(arcazeSerialComboBox.Items[lastSelectedIndex].ToString());
            }

            // load settings of new item
            if (cb.SelectedIndex > 0)
            {
                _syncFromModuleSettings(cb.SelectedItem.ToString());
            }           

            lastSelectedIndex = cb.SelectedIndex;
        }

        private void ledDisplaysTabPage_Validating(object sender, CancelEventArgs e)
        {
            // check that for all available arcaze serials there is an entry in module settings
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void arcazeModuleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            numModulesLabel.Visible = (sender as ComboBox).SelectedIndex != 0;
            numModulesNumericUpDown.Visible = (sender as ComboBox).SelectedIndex != 0;

            bool brightnessVisible = (sender as ComboBox).SelectedIndex != 0 && ((sender as ComboBox).SelectedItem.ToString() != ArcazeCommand.ExtModuleType.LedDriver2.ToString());
            globalBrightnessLabel.Visible = brightnessVisible;
            globalBrightnessTrackBar.Visible = brightnessVisible;

            // check if the extension is compatible
            // but only if not the first item (please select) == 0
            // or none selected yet == -1
            if (arcazeSerialComboBox.SelectedIndex <= 0) return;
            
            IModuleInfo devInfo = (IModuleInfo) ((arcazeSerialComboBox.SelectedItem as ArcazeListItem).Value);
            
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
                MessageBox.Show(MainForm._tr(errMessage));
            }
        }

        private void mobiflightSettingsLabel_Click(object sender, EventArgs e)
        {

        }

        private void mobiflightSettingsToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void mfModulesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            mfModulesTreeView.SelectedNode = e.Node;
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            
            if (e.Node.Nodes.Count!=0) return;
            try
            {
                Control panel = null;
                IConnectedDevice dev = (e.Node.Tag as IConnectedDevice);
                switch (dev.Type)
                {
                    case "LEDMODULE":
                        MobiFlightLedModule ledModule = dev as MobiFlightLedModule;
                        panel = new MobiFlight.Panels.MFLedSegmentPanel();
                        break;

                    case "STEPPER":
                        MobiFlightStepper28BYJ stepper = dev as MobiFlightStepper28BYJ;
                        panel = new MobiFlight.Panels.MFStepperPanel();
                        break;

                    case "SERVO":
                        MobiFlightServo servo = dev as MobiFlightServo;
                        panel = new MobiFlight.Panels.MFServoPanel();
                        break;
                }

                if (panel != null)
                {
                    mfSettingsPanel.Controls.Clear();
                    mfSettingsPanel.Controls.Add(panel);
                    panel.Dock = DockStyle.Fill;
                    
                }
            }
            catch (Exception ex)
            {
                // Show error message
            }
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
