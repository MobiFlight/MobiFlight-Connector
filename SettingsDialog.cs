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
using System.IO;

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
            mfTreeViewImageList.Images.Add(DeviceType.Button.ToString(), ArcazeUSB.Properties.Resources.cd);
            mfTreeViewImageList.Images.Add(DeviceType.Stepper.ToString(), ArcazeUSB.Properties.Resources.dvd);
            mfTreeViewImageList.Images.Add(DeviceType.Servo.ToString(), ArcazeUSB.Properties.Resources.dvd);
            mfTreeViewImageList.Images.Add(DeviceType.Output.ToString(), ArcazeUSB.Properties.Resources.lightbulb_on);
            mfTreeViewImageList.Images.Add(DeviceType.LedModule.ToString(), ArcazeUSB.Properties.Resources.sound);
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

            // synchronize the toolbar icons
            uploadToolStripButton.Enabled = false;
            openToolStripButton.Enabled = true;
            saveToolStripButton.Enabled = false;
            addDeviceToolStripDropDownButton.Enabled = false;
            removeDeviceToolStripButton.Enabled = false;

            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();

            mfModulesTreeView.Nodes.Clear();
            try
            {
                foreach (MobiFlightModuleInfo module in mobiflightCache.getConnectedModules())
                {
                    TreeNode node = new TreeNode(module.Name);
                    node.Tag = mobiflightCache.GetModule(module);
                    mfModulesTreeView.Nodes.Add(node);
                    
                    foreach (MobiFlight.Config.BaseDevice device in (node.Tag as MobiFlightModule).Config.Items)
                    {
                        TreeNode deviceNode = new TreeNode(device.Name);
                        deviceNode.Tag = device;
                        deviceNode.SelectedImageKey = deviceNode.ImageKey = device.Type.ToString();                        
                        node.Nodes.Add(deviceNode);
                    }
                    
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                // this happens when the modules are connecting
                mfConfiguredModulesGroupBox.Enabled = false;
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
            if (checkIfMobiFlightSettingsHaveChanged() && 
                MessageBox.Show("You have unsaved changes in one of your module's settings. \n Do you want to cancel and loose your changes?", "Unsaved changes", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK
                )
            {
                DialogResult = DialogResult.Cancel;
            }
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

            if (e.Node.Level == 0)
            {
                // this is the module node
                // set the add device icon enabled
                addDeviceToolStripDropDownButton.Enabled = true;
                removeDeviceToolStripButton.Enabled = false;
                uploadToolStripButton.Enabled = e.Node.Nodes.Count>0;
                saveToolStripButton.Enabled = e.Node.Nodes.Count > 0;
                return;
            }

            syncPanelWithSelectedDevice(e.Node);
        }

        private void syncPanelWithSelectedDevice(TreeNode selectedNode)
        {
            try
            {
                Control panel = null;
                removeDeviceToolStripButton.Enabled = true;
                uploadToolStripButton.Enabled = true;
                saveToolStripButton.Enabled = true;
                MobiFlight.Config.BaseDevice dev = (selectedNode.Tag as MobiFlight.Config.BaseDevice);
                switch (dev.Type)
                {
                    case DeviceType.LedModule:
                        panel = new MobiFlight.Panels.MFLedSegmentPanel(dev as MobiFlight.Config.LedModule);
                        (panel as MobiFlight.Panels.MFLedSegmentPanel).Changed += new EventHandler(mfConfigObject_changed);
                        break;

                    case DeviceType.Stepper:
                        panel = new MobiFlight.Panels.MFStepperPanel(dev as MobiFlight.Config.Stepper);
                        (panel as MobiFlight.Panels.MFStepperPanel).Changed += new EventHandler(mfConfigObject_changed);
                        break;

                    case DeviceType.Servo:
                        panel = new MobiFlight.Panels.MFServoPanel(dev as MobiFlight.Config.Servo);
                        (panel as MobiFlight.Panels.MFServoPanel).Changed+=new EventHandler(mfConfigObject_changed);
                        break;

                    case DeviceType.Button:
                        panel = new MobiFlight.Panels.MFButtonPanel(dev as MobiFlight.Config.Button);
                        (panel as MobiFlight.Panels.MFButtonPanel).Changed += new EventHandler(mfConfigObject_changed);
                        break;

                    case DeviceType.Encoder:
                        panel = new MobiFlight.Panels.MFEncoderPanel(dev as MobiFlight.Config.Encoder);
                        (panel as MobiFlight.Panels.MFEncoderPanel).Changed += new EventHandler(mfConfigObject_changed);
                        break;

                    case DeviceType.Output:
                        panel = new MobiFlight.Panels.MFOutputPanel(dev as MobiFlight.Config.Output);
                        (panel as MobiFlight.Panels.MFOutputPanel).Changed += new EventHandler(mfConfigObject_changed);
                        break;
                    // output
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

        void mfConfigObject_changed(object sender, EventArgs e)
        {
            mfModulesTreeView.SelectedNode.Text = (sender as MobiFlight.Config.BaseDevice).Name;
        }

        private void addDeviceTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MobiFlight.Config.BaseDevice cfgItem = null; 
            switch ((sender as ToolStripMenuItem).Name) {
                case "addServoToolStripMenuItem":
                    cfgItem = new MobiFlight.Config.Servo();
                    break;
                case "addStepperToolStripMenuItem":
                    cfgItem = new MobiFlight.Config.Stepper();
                    break;
                case "addOutputToolStripMenuItem":
                    cfgItem = new MobiFlight.Config.Output();
                    break;
                case "addLedModuleToolStripMenuItem":
                    cfgItem = new MobiFlight.Config.LedModule();
                    break;
                case "addButtonToolStripMenuItem":
                    cfgItem = new MobiFlight.Config.Button();
                    break;
                case "addEncoderToolStripMenuItem":
                    cfgItem = new MobiFlight.Config.Encoder();
                    break;
                default:
                    // do nothing
                    return;
            }

            TreeNode newNode = new TreeNode(cfgItem.Name);
            newNode.SelectedImageKey = newNode.ImageKey = cfgItem.Type.ToString(); 
            newNode.Tag = cfgItem;
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;
            
            parentNode.Nodes.Add(newNode);
            //(parentNode.Tag as MobiFlightModule).Config.AddItem(cfgItem);
            
            mfModulesTreeView.SelectedNode = newNode;
            syncPanelWithSelectedDevice(newNode);
            
        }

        private void uploadToolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to update your module with the current configuration?", "Upload configuration", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            MobiFlight.Config.Config newConfig = new MobiFlight.Config.Config();

            foreach (TreeNode node in parentNode.Nodes)
            {
                newConfig.Items.Add(node.Tag as MobiFlight.Config.BaseDevice);
            }

            module.Config = newConfig;
            
            if (MessageBox.Show(module.Config.ToInternal(), "Confirm", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                module.SaveConfig();
            }
        }

        private bool checkIfMobiFlightSettingsHaveChanged()
        {
            return false;
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            MobiFlight.Config.Config newConfig = new MobiFlight.Config.Config();

            foreach (TreeNode node in parentNode.Nodes)
            {
                newConfig.Items.Add(node.Tag as MobiFlight.Config.BaseDevice);
            }

            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "Mobiflight Module Config (*.mfmc)|*.mfmc";
            fd.FileName = parentNode.Text + ".mfmc";

            if (DialogResult.OK == fd.ShowDialog())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MobiFlight.Config.Config));
                TextWriter textWriter = new StreamWriter(fd.FileName);
                serializer.Serialize(textWriter, newConfig);
                textWriter.Close();
            } 
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;
           
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Mobiflight Module Config (*.mfmc)|*.mfmc";

            if (DialogResult.OK == fd.ShowDialog())
            {
                TextReader textReader = new StreamReader(fd.FileName);
                XmlSerializer serializer = new XmlSerializer(typeof(MobiFlight.Config.Config));
                MobiFlight.Config.Config newConfig;
                newConfig = (MobiFlight.Config.Config)serializer.Deserialize(textReader);
                textReader.Close();

                parentNode.Nodes.Clear();

                foreach( MobiFlight.Config.BaseDevice device in newConfig.Items) {
                    TreeNode newNode = new TreeNode(device.Name);
                    newNode.Tag = device;
                    parentNode.Nodes.Add(newNode);
                }
            } 
        }

        private void removeDeviceToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode node = mfModulesTreeView.SelectedNode;
            mfModulesTreeView.Nodes.Remove(node);

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
