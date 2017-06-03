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
using System.IO;
using System.Text.RegularExpressions;

namespace MobiFlight
{
    public partial class SettingsDialog : Form
    {
        List <ArcazeModuleSettings> moduleSettings;
        ExecutionManager execManager;
        int lastSelectedIndex = -1;
        Forms.FirmwareUpdateProcess FirmwareUpdateProcessForm = new Forms.FirmwareUpdateProcess();
        public bool MFModuleConfigChanged { get; set; }

        const bool StepperSupport = true;
        const bool ServoSupport = true;
        private int NumberOfModulesForFirmwareUpdate = 0;
        bool IgnoreArcazeModuleSettingsChangeEvents = false;

        public List<MobiFlightModuleInfo> modulesForFlashing;
        public List<MobiFlightModule> modulesForUpdate;

        public SettingsDialog()
        {
            Init();
        }

        public SettingsDialog(ExecutionManager execManager)
        {
            this.execManager = execManager;
            modulesForFlashing = new List<MobiFlightModuleInfo>();
            modulesForUpdate = new List<MobiFlightModule>();
            Init();

            InitArcazeModuleTreeView(execManager);
        }
        
        private void InitArcazeModuleTreeView(ExecutionManager execManager)
        {
            ArcazeCache arcazeCache = execManager.getModuleCache();

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

            if (ArcazeModuleTreeView.Nodes.Count == 0)
            {
                TreeNode NewNode = new TreeNode();
                NewNode.Text = MainForm._tr("none");
                NewNode.SelectedImageKey = NewNode.ImageKey = "module-arcaze";
                ArcazeModuleTreeView.Nodes.Add(NewNode);
                ArcazeModuleTreeView.Enabled = false;
                arcazeModuleSettingsGroupBox.Enabled = false;
            }
        }

        private void Init()
        {
            InitializeComponent();

            arcazeModuleTypeComboBox.Items.Clear();
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.InternalIo.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.DisplayDriver.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.LedDriver2.ToString());
            arcazeModuleTypeComboBox.Items.Add(ArcazeCommand.ExtModuleType.LedDriver3.ToString());
            arcazeModuleTypeComboBox.SelectedIndex = 0;

            // initialize mftreeviewimagelist
            mfTreeViewImageList.Images.Add("module", MobiFlight.Properties.Resources.module_mobiflight);
            mfTreeViewImageList.Images.Add("module-arduino", MobiFlight.Properties.Resources.module_arduino);
            mfTreeViewImageList.Images.Add("module-update", MobiFlight.Properties.Resources.module_mobiflight_update);
            mfTreeViewImageList.Images.Add("module-unknown", MobiFlight.Properties.Resources.module_arduino);
            mfTreeViewImageList.Images.Add("module-arcaze", MobiFlight.Properties.Resources.arcaze_module);
            mfTreeViewImageList.Images.Add(DeviceType.Button.ToString(), MobiFlight.Properties.Resources.button);
            mfTreeViewImageList.Images.Add(DeviceType.Encoder.ToString(), MobiFlight.Properties.Resources.encoder);
            mfTreeViewImageList.Images.Add(DeviceType.Stepper.ToString(), MobiFlight.Properties.Resources.stepper);
            mfTreeViewImageList.Images.Add(DeviceType.Servo.ToString(), MobiFlight.Properties.Resources.servo);
            mfTreeViewImageList.Images.Add(DeviceType.Output.ToString(), MobiFlight.Properties.Resources.output);
            mfTreeViewImageList.Images.Add(DeviceType.LedModule.ToString(), MobiFlight.Properties.Resources.led7);
            mfTreeViewImageList.Images.Add(DeviceType.LcdDisplay.ToString(), MobiFlight.Properties.Resources.led7);
            mfTreeViewImageList.Images.Add("Changed", MobiFlight.Properties.Resources.module_changed);
            mfTreeViewImageList.Images.Add("Changed-arcaze", MobiFlight.Properties.Resources.arcaze_changed);
            mfTreeViewImageList.Images.Add("new-arcaze", MobiFlight.Properties.Resources.arcaze_new);
            //mfModulesTreeView.ImageList = mfTreeViewImageList;

            addStepperToolStripMenuItem.Visible = stepperToolStripMenuItem.Visible = StepperSupport;
            addServoToolStripMenuItem.Visible = servoToolStripMenuItem.Visible = ServoSupport;

            loadSettings();

#if MOBIFLIGHT
            // do nothing
#else
            tabControl1.TabPages.Remove(mobiFlightTabPage);
#endif

            ModuleConfigChanged = false;
            MFModuleConfigChanged = false;
            
            // setup the background worker for firmware update
            firmwareUpdateBackgroundWorker.DoWork += new DoWorkEventHandler(firmwareUpdateBackgroundWorker_DoWork);
            firmwareUpdateBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(firmwareUpdateBackgroundWorker_RunWorkerCompleted);
        }

        
        /// <summary>
        /// Load all settings for each tabns
        /// </summary>
        private void loadSettings ()
        {            
            //
            // TAB General
            //
            // Recent Files max count
            recentFilesNumericUpDown.Value = Properties.Settings.Default.RecentFilesMaxCount;

            // TestMode speed
            // (1s) 0 - 4 (50ms)
            testModeSpeedTrackBar.Value = 0;
            if (Properties.Settings.Default.TestTimerInterval == 500) testModeSpeedTrackBar.Value = 1;
            else if (Properties.Settings.Default.TestTimerInterval == 250) testModeSpeedTrackBar.Value = 2;
            else if (Properties.Settings.Default.TestTimerInterval == 125) testModeSpeedTrackBar.Value = 3;
            else if (Properties.Settings.Default.TestTimerInterval == 50) testModeSpeedTrackBar.Value = 4;

            logLevelCheckBox.Checked = Properties.Settings.Default.LogEnabled;
            ComboBoxHelper.SetSelectedItem(logLevelComboBox, Properties.Settings.Default.LogLevel);

            //
            // TAB Arcaze
            //
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
                    Log.Instance.log("Exception on DEserializing arcaze extension module settings: " + e.Message, LogSeverity.Debug);
                }
            }

            //
            // TAB MobiFlight
            //
            loadMobiFlightSettings();

            //
            // TAB FSUIPC
            //
            // FSUIPC poll interval
            fsuipcPollIntervalTrackBar.Value = (int)Math.Floor(Properties.Settings.Default.PollInterval / 50.0);
        }

        /// <summary>
        /// Initialize the MobiFlight Tab
        /// </summary>
        private void loadMobiFlightSettings()
        {
#if MOBIFLIGHT

            // synchronize the toolbar icons
            mobiflightSettingsToolStrip.Enabled = false;
            uploadToolStripButton.Enabled = false;
            openToolStripButton.Enabled = true;
            saveToolStripButton.Enabled = false;
            addDeviceToolStripDropDownButton.Enabled = false;
            removeDeviceToolStripButton.Enabled = false;

            //
            // Build the tree
            // 
            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();

            mfModulesTreeView.Nodes.Clear();
            try
            {
                foreach (MobiFlightModuleInfo module in mobiflightCache.getConnectedModules())
                {
                    TreeNode node = new TreeNode();
                    node = mfModulesTreeView_initNode(module, node);
                    if (!module.HasMfFirmware())
                    {
                        node.SelectedImageKey = node.ImageKey = "module-arduino";
                    }
                    else
                    {
                        Version latestVersion = new Version(MobiFlightModuleInfo.LatestFirmwareMega);
                        switch(module.Type)
                        {
                            case MobiFlightModuleInfo.TYPE_ARDUINO_MICRO:
                                latestVersion = new Version(MobiFlightModuleInfo.LatestFirmwareMicro);
                                break;

                            case MobiFlightModuleInfo.TYPE_ARDUINO_UNO:
                                latestVersion = new Version(MobiFlightModuleInfo.LatestFirmwareUno);
                                break;
                        }
                        Version currentVersion = new Version(module.Version);
                        if (currentVersion.CompareTo(latestVersion) < 0)
                        {
                            node.SelectedImageKey = node.ImageKey = "module-update";
                            node.ToolTipText = MainForm._tr("uiMessageSettingsDlgOldFirmware");
                        }
                    }
                    
                    mfModulesTreeView.Nodes.Add(node);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                // this happens when the modules are connecting
                mfConfiguredModulesGroupBox.Enabled = false;
                Log.Instance.log("Problem on building module tree. Still connecting", LogSeverity.Error);
            }

            if (mfModulesTreeView.Nodes.Count == 0)
            {
                TreeNode NewNode = new TreeNode();
                NewNode.Text = MainForm._tr("none");
                NewNode.SelectedImageKey = NewNode.ImageKey = "module-arduino";
                mfModulesTreeView.Nodes.Add(NewNode);
                mfModulesTreeView.Enabled = false;
            }

            firmwareArduinoIdePathTextBox.Text = Properties.Settings.Default.ArduinoIdePath;
            FwAutoUpdateCheckBox.Checked = Properties.Settings.Default.FwAutoUpdateCheck;
#endif
        }

        private TreeNode mfModulesTreeView_initNode(MobiFlightModuleInfo module, TreeNode node)
        {
            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();

            node.Text = module.Name;
            if (module.HasMfFirmware())
            {
                node.SelectedImageKey = node.ImageKey = "module";
                node.Tag = mobiflightCache.GetModule(module);
                node.Nodes.Clear();

                foreach (MobiFlight.Config.BaseDevice device in (node.Tag as MobiFlightModule).Config.Items)
                {
                    TreeNode deviceNode = new TreeNode(device.Name);
                    deviceNode.Tag = device;
                    deviceNode.SelectedImageKey = deviceNode.ImageKey = device.Type.ToString();
                    node.Nodes.Add(deviceNode);
                }
            }
            else
            {
                node.Tag = new MobiFlightModule(new MobiFlightModuleConfig() { ComPort = module.Port, Type = module.Type });
            }

            return node;
        }

        /// <summary>
        /// Save the settings from tabs in Properties.Settings
        /// This does not apply to MF modules
        /// </summary>
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

            // log settings
            Properties.Settings.Default.LogEnabled = logLevelCheckBox.Checked;
            Properties.Settings.Default.LogLevel = logLevelComboBox.SelectedItem as String;            
            Log.Instance.Enabled = logLevelCheckBox.Checked;
            Log.Instance.Severity = (LogSeverity) Enum.Parse(typeof(LogSeverity), Properties.Settings.Default.LogLevel);

            // MobiFlight Tab
            // only the Firmware Auto Check Update needs to be synchronized 
            Properties.Settings.Default.FwAutoUpdateCheck = FwAutoUpdateCheckBox.Checked;

            try
            {
                XmlSerializer SerializerObj = new XmlSerializer(typeof(List<ArcazeModuleSettings>));                
                System.IO.StringWriter w = new System.IO.StringWriter();
                SerializerObj.Serialize(w, moduleSettings);
                Properties.Settings.Default.ModuleSettings = w.ToString();
                Log.Instance.log("Serialized Arcaze Extension Module Settings: " + Properties.Settings.Default.ModuleSettings, LogSeverity.Debug);
            }
            catch (Exception e)
            {
                Log.Instance.log("Exception on serializing arcaze extension module settings: " + e.Message, LogSeverity.Debug);
            }
        }

        /// <summary>
        /// Callback for OK Button, used to close the form and save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                return;
            }

            MFModuleConfigChanged = false;
            foreach (TreeNode node in mfModulesTreeView.Nodes)
            {
                if (node.ImageKey == "Changed")
                {
                    MFModuleConfigChanged = true;
                    break;
                }
            }

            if (MFModuleConfigChanged)
            {
                if (MessageBox.Show(MainForm._tr("MFModuleConfigChanged"),
                                    MainForm._tr("Hint"),
                                    MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                {
                    tabControl1.SelectedIndex = 2;
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            if (ArcazeModuleTreeView.SelectedNode != null)
            {
                _syncToModuleSettings((ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo).Serial);
            }
            saveSettings();
        }

        /// <summary>
        /// Save the module settings for the different Arcaze Boards
        /// </summary>
        /// <param name="serial"></param>
        private void _syncToModuleSettings(string serial) {
            if (IgnoreArcazeModuleSettingsChangeEvents) return;

            ArcazeModuleSettings settingsToSave = null;
            if (serial.Contains("/"))
                serial = serial.Split('/')[1].Trim();

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
                if (serial.Contains("/"))
                    serial = serial.Split('/')[1].Trim();

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

        /// <summary>
        /// Validate settings, e.g. ensure that every Arcaze has been configured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ledDisplaysTabPage_Validating(object sender, CancelEventArgs e)
        {
            // check that for all available arcaze serials there is an entry in module settings
        }

        /// <summary>
        /// Callback for cancel button - discard changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (checkIfMobiFlightSettingsHaveChanged()) {
                if (MessageBox.Show("You have unsaved changes in one of your module's settings. \n Do you want to cancel and loose your changes?", "Unsaved changes", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                }                
            }

            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Callback if extension type is changed for a selected Arcaze Board
        /// Show the correct options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                MessageBox.Show(MainForm._tr(errMessage));
            }

            _syncToModuleSettings((ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo).Serial);
        }

        private void mobiflightSettingsLabel_Click(object sender, EventArgs e)
        {

        }

        private void mobiflightSettingsToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /// <summary>
        /// Eventhandler whenever a module has been selected in treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mfModulesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
        }

        /// <summary>
        /// Show the necessary options for a selected device which is attached to a MobiFlight module
        /// </summary>
        /// <param name="selectedNode"></param>
        private void syncPanelWithSelectedDevice(TreeNode selectedNode)
        {
            try
            {
                Control panel = null;
                removeDeviceToolStripButton.Enabled = selectedNode.Level > 0;
                uploadToolStripButton.Enabled = true;
                saveToolStripButton.Enabled = true;
                mfSettingsPanel.Controls.Clear();

                if (selectedNode.Level == 0)
                {
                    panel = new MobiFlight.Panels.MFModulePanel((selectedNode.Tag as MobiFlightModule));
                    (panel as MobiFlight.Panels.MFModulePanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                }
                else
                {
                    TreeNode parentNode = mfModulesTreeView.SelectedNode;
                    if (parentNode == null) return;
                    while (parentNode.Level > 0) parentNode = parentNode.Parent;
                    MobiFlightModule module = getModuleFromTree();
                    
                    MobiFlight.Config.BaseDevice dev = (selectedNode.Tag as MobiFlight.Config.BaseDevice);
                    switch (dev.Type)
                    {
                        case DeviceType.LedModule:
                            panel = new MobiFlight.Panels.MFLedSegmentPanel(dev as MobiFlight.Config.LedModule, module.GetFreePins());
                            (panel as MobiFlight.Panels.MFLedSegmentPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Stepper:
                            panel = new MobiFlight.Panels.MFStepperPanel(dev as MobiFlight.Config.Stepper, module.GetFreePins());
                            (panel as MobiFlight.Panels.MFStepperPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Servo:
                            panel = new MobiFlight.Panels.MFServoPanel(dev as MobiFlight.Config.Servo, module.GetFreePins());
                            (panel as MobiFlight.Panels.MFServoPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Button:
                            panel = new MobiFlight.Panels.MFButtonPanel(dev as MobiFlight.Config.Button, module.GetFreePins());
                            (panel as MobiFlight.Panels.MFButtonPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Encoder:
                            panel = new MobiFlight.Panels.MFEncoderPanel(dev as MobiFlight.Config.Encoder, module.GetFreePins());
                            (panel as MobiFlight.Panels.MFEncoderPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Output:
                            panel = new MobiFlight.Panels.MFOutputPanel(dev as MobiFlight.Config.Output, module.GetFreePins());
                            (panel as MobiFlight.Panels.MFOutputPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.LcdDisplay:
                            panel = new MobiFlight.Panels.MFLcddDisplayPanel(dev as MobiFlight.Config.LCDDisplay, module.GetFreePins());
                            (panel as MobiFlight.Panels.MFLcddDisplayPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;
                            // output
                    }
                }

                if (panel != null)
                {
                    panel.Padding = new Padding(2,0,0,0);
                    mfSettingsPanel.Controls.Add(panel);
                    panel.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                // Show error message
                Log.Instance.log("syncPanelWithSelectedDevice: Exception: " + ex.Message, LogSeverity.Debug);
            }
        }

        /// <summary>
        /// Update the name of a module in the TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mfConfigDeviceObject_changed(object sender, EventArgs e)
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            String UniqueName;
            bool BaseDeviceHasChanged = (sender is MobiFlight.Config.BaseDevice);

            if (BaseDeviceHasChanged)
                UniqueName = (sender as MobiFlight.Config.BaseDevice).Name;
            else
                UniqueName = (sender as MobiFlight.MobiFlightModule).Name;

            if (!MobiFlightModule.IsValidDeviceName(UniqueName))
            {
                String invalidCharacterList = "";
                foreach (String c in MobiFlightModule.ReservedChars)
                {
                    invalidCharacterList += c + "  ";
                }
                invalidCharacterList = invalidCharacterList.Replace(@"\\\", "");

                displayError(mfSettingsPanel.Controls[0], 
                        String.Format(MainForm._tr("uiMessageDeviceNameContainsInvalidCharsOrTooLong"),
                                      invalidCharacterList,
                                      MobiFlightModule.MaxDeviceNameLength.ToString()));
                UniqueName = UniqueName.Substring(0, UniqueName.Length - 1);

                if (BaseDeviceHasChanged)
                    (sender as MobiFlight.Config.BaseDevice).Name = UniqueName;
                else
                    (sender as MobiFlight.MobiFlightModule).Name = UniqueName;

                syncPanelWithSelectedDevice(mfModulesTreeView.SelectedNode);
                return;
            }

            removeError(mfSettingsPanel.Controls[0]);

            if (BaseDeviceHasChanged)
            {
                List<String> NodeNames = new List<String>();
                foreach (TreeNode node in parentNode.Nodes)
                {
                    if (node == mfModulesTreeView.SelectedNode) continue;
                    NodeNames.Add(node.Text);
                }

                UniqueName = MobiFlightModule.GenerateUniqueDeviceName(NodeNames.ToArray(), UniqueName);

                if (UniqueName != (sender as MobiFlight.Config.BaseDevice).Name)
                {
                    (sender as MobiFlight.Config.BaseDevice).Name = UniqueName;
                    MessageBox.Show(MainForm._tr("uiMessageDeviceNameAlreadyUsed"), MainForm._tr("Hint"), MessageBoxButtons.OK);
                }

                mfModulesTreeView.SelectedNode.Text = (sender as MobiFlight.Config.BaseDevice).Name;
            }
            else
            {
                mfModulesTreeView.SelectedNode.Text = (sender as MobiFlight.MobiFlightModule).Name;
            }

            parentNode.ImageKey = "Changed";
            parentNode.SelectedImageKey = "Changed";
        }

        /// <summary>
        /// EventHandler to add a selected device to the current module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDeviceTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MobiFlight.Config.BaseDevice cfgItem = null;
                switch ((sender as ToolStripMenuItem).Name)
                {
                    case "servoToolStripMenuItem":
                    case "addServoToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.Servo();
                        (cfgItem as MobiFlight.Config.Servo).DataPin = getModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        break;
                    case "stepperToolStripMenuItem":
                    case "addStepperToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.Stepper();
                        (cfgItem as MobiFlight.Config.Stepper).Pin1 = getModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        (cfgItem as MobiFlight.Config.Stepper).Pin2 = getModuleFromTree().GetFreePins().ElementAt(1).ToString();
                        (cfgItem as MobiFlight.Config.Stepper).Pin3 = getModuleFromTree().GetFreePins().ElementAt(2).ToString();
                        (cfgItem as MobiFlight.Config.Stepper).Pin4 = getModuleFromTree().GetFreePins().ElementAt(3).ToString();
                        break;
                    case "ledOutputToolStripMenuItem":
                    case "addOutputToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.Output();
                        (cfgItem as MobiFlight.Config.Output).Pin = getModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        break;
                    case "ledSegmentToolStripMenuItem":
                    case "addLedModuleToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.LedModule();
                        (cfgItem as MobiFlight.Config.LedModule).DinPin = getModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        (cfgItem as MobiFlight.Config.LedModule).ClkPin = getModuleFromTree().GetFreePins().ElementAt(1).ToString();
                        (cfgItem as MobiFlight.Config.LedModule).ClsPin = getModuleFromTree().GetFreePins().ElementAt(2).ToString();
                        break;
                    case "buttonToolStripMenuItem":
                    case "addButtonToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.Button();
                        (cfgItem as MobiFlight.Config.Button).Pin = getModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        break;
                    case "encoderToolStripMenuItem":
                    case "addEncoderToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.Encoder();
                        (cfgItem as MobiFlight.Config.Encoder).PinLeft = getModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        (cfgItem as MobiFlight.Config.Encoder).PinRight = getModuleFromTree().GetFreePins().ElementAt(1).ToString();
                        break;
                    case "LcdDisplayToolStripMenuItem":
                    case "addLcdDisplayToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.LCDDisplay();
                        // does not deal yet with these kind of pins because we use I2C
                        break;
                    default:
                        // do nothing
                        return;
                }
                TreeNode parentNode = mfModulesTreeView.SelectedNode;
                if (parentNode == null) return;

                while (parentNode.Level > 0) parentNode = parentNode.Parent;
                List<String> NodeNames = new List<String>();
                foreach (TreeNode node in parentNode.Nodes)
                {
                    NodeNames.Add(node.Text);
                }
                cfgItem.Name = MobiFlightModule.GenerateUniqueDeviceName(NodeNames.ToArray(), cfgItem.Name);

                TreeNode newNode = new TreeNode(cfgItem.Name);
                newNode.SelectedImageKey = newNode.ImageKey = cfgItem.Type.ToString();
                newNode.Tag = cfgItem;

                parentNode.Nodes.Add(newNode);
                parentNode.ImageKey = "Changed";
                parentNode.SelectedImageKey = "Changed";

                //(parentNode.Tag as MobiFlightModule).Config.AddItem(cfgItem);

                mfModulesTreeView.SelectedNode = newNode;
                syncPanelWithSelectedDevice(newNode);
            }catch(ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(MainForm._tr("uiMessageNotEnoughPinsMessage"),
                                MainForm._tr("uiMessageNotEnoughPinsHint"),
                                MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            
        }

        /// <summary>
        /// EventHandler for upload button, this uploads the new config to the module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadToolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(MainForm._tr("uiMessageUploadConfigurationConfirm"), 
                                MainForm._tr("uiMessageUploadConfigurationHint"), 
                                MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            MobiFlight.Config.Config newConfig = new MobiFlight.Config.Config();

            foreach (TreeNode node in parentNode.Nodes)
            {
                newConfig.Items.Add(node.Tag as MobiFlight.Config.BaseDevice);
            }

            module.Config = newConfig;

            String LogMessage = String.Join("", module.Config.ToInternal(module.MaxMessageSize).ToArray());
            if (LogMessage.Length > module.EepromSize) {
                MessageBox.Show(MainForm._tr("uiMessageUploadConfigurationTooLong"), 
                                MainForm._tr("uiMessageUploadConfigurationHint"), 
                                MessageBoxButtons.OK);
                return;
            }

            Log.Instance.log("Uploading config: " + LogMessage, LogSeverity.Info);
            
            bool uploadResult = module.SaveConfig();
            module.Config = null;
            //module.ResetBoard();
            module.LoadConfig();
            execManager.getMobiFlightModuleCache().updateConnectedModuleName(module);

            parentNode.ImageKey = "";
            parentNode.SelectedImageKey = "";

            if (uploadResult)
            {
                MessageBox.Show(MainForm._tr("uiMessageUploadConfigurationFinished"),
                                MainForm._tr("uiMessageUploadConfigurationHint"),
                                MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(MainForm._tr("uiMessageUploadConfigurationFinishedWithError"),
                                MainForm._tr("uiMessageUploadConfigurationHint"),
                                MessageBoxButtons.OK);
            }
                        
        }

        /// <summary>
        /// Check whether some settings have changed and return bool
        /// </summary>
        /// <returns></returns>
        private bool checkIfMobiFlightSettingsHaveChanged()
        {
            return false;
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

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
            if (parentNode == null) return;

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
                    newNode.SelectedImageKey = newNode.ImageKey = device.Type.ToString();
                    parentNode.Nodes.Add(newNode);
                }
            } 
        }

        private void removeDeviceToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode node = mfModulesTreeView.SelectedNode;
            if (node == null) return;
            if (node.Level == 0) return;

            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            mfModulesTreeView.Nodes.Remove(node);

            parentNode.ImageKey = "Changed";
            parentNode.SelectedImageKey = "Changed";
        }

        public bool ModuleConfigChanged { get; set; }

        private void updateFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = this.mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            if (this.mfModulesTreeView.SelectedNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;
            
            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            updateFirmware(module);
        }

        protected void updateFirmware (MobiFlightModule module) {
            String arduinoIdePath = firmwareArduinoIdePathTextBox.Text;
            String firmwarePath = Directory.GetCurrentDirectory() + "\\firmware";

            if (!MobiFlightFirmwareUpdater.IsValidArduinoIdePath(arduinoIdePath))
            {
                MessageBox.Show(
                    MainForm._tr("uiMessageFirmwareCheckPath"),
                    MainForm._tr("Hint"), MessageBoxButtons.OK);
                return;
            }

            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();
            execManager.AutoConnectStop();
            module.Disconnect();
            
            MobiFlightFirmwareUpdater.ArduinoIdePath = arduinoIdePath;
            MobiFlightFirmwareUpdater.FirmwarePath = firmwarePath;

            NumberOfModulesForFirmwareUpdate++;
            firmwareUpdateBackgroundWorker.RunWorkerAsync(module);
            FirmwareUpdateProcessForm.ShowDialog();
        }

        void firmwareUpdateBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NumberOfModulesForFirmwareUpdate--;
            if (NumberOfModulesForFirmwareUpdate==0)
                FirmwareUpdateProcessForm.Hide();

            if (e.Error != null)
            {
                MessageBox.Show("There was an error on uploading the firmware!\nEnable Debug Logging for more details.", 
                                MainForm._tr("Hint"), MessageBoxButtons.OK);
                return;
            }

            // update presentation in treeView
            MobiFlightModule module = (MobiFlightModule) e.Result;
            
            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();

            module.Connect();
            MobiFlightModuleInfo newInfo = module.GetInfo() as MobiFlightModuleInfo;
            mobiflightCache.RefreshModule(module);
            
            execManager.AutoConnectStart();

            // Update the corresponding TreeView Item
            //// Find the parent node that matches the Port
            TreeNode parentNode = findNodeByPort(module.Port);

            if (parentNode != null)
            {
                mfModulesTreeView_initNode(newInfo, parentNode);
                // make sure that we retrigger all events and sync the panel
                mfModulesTreeView.SelectedNode = null;
                mfModulesTreeView.SelectedNode = parentNode;
            }

            if (NumberOfModulesForFirmwareUpdate == 0)
                MessageBox.Show(
                    MainForm._tr("uiMessageFirmwareUploadSuccessful"), 
                    MainForm._tr("Hint"), 
                    MessageBoxButtons.OK
                );
        }

        private TreeNode findNodeByPort(string port)
        {
            TreeNode resultNode = null;
            foreach(TreeNode node in mfModulesTreeView.Nodes)
            {
                if (node.Tag is MobiFlightModule)
                {
                    if (port != (node.Tag as MobiFlightModule).Port) continue;
                    resultNode = node;
                    break;
                }
            }
            return resultNode;
        }

        void firmwareUpdateBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MobiFlightModule module = (MobiFlightModule)e.Argument;
            bool UpdateResult = MobiFlightFirmwareUpdater.Update(module);
            e.Result = module;
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (Directory.Exists(firmwareArduinoIdePathTextBox.Text))
            {
                fbd.SelectedPath = firmwareArduinoIdePathTextBox.Text;
            }
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                firmwareArduinoIdePathTextBox.Text = fbd.SelectedPath;
                firmwareArduinoIdePathTextBox.Focus();
                (sender as Button).Focus();
            }
        }

        private void mfModuleSettingsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void mfModulesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            TreeNode parentNode = e.Node;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            mfSettingsPanel.Controls.Clear();
            if (parentNode.Tag == null) return;

            bool isMobiFlightBoard = (parentNode.Tag as MobiFlightModule).Type != MobiFlightModuleInfo.TYPE_ARDUINO_MEGA
                                  && (parentNode.Tag as MobiFlightModule).Type != MobiFlightModuleInfo.TYPE_ARDUINO_MICRO
                                  && (parentNode.Tag as MobiFlightModule).Type != MobiFlightModuleInfo.TYPE_ARDUINO_UNO;

            mobiflightSettingsToolStrip.Enabled = isMobiFlightBoard;
            // this is the module node
            // set the add device icon enabled
            addDeviceToolStripDropDownButton.Enabled = isMobiFlightBoard;
            removeDeviceToolStripButton.Enabled = isMobiFlightBoard & (e.Node.Level > 0);
            uploadToolStripButton.Enabled = (parentNode.Nodes.Count > 0) || (parentNode.ImageKey == "Changed");
            saveToolStripButton.Enabled = parentNode.Nodes.Count > 0;
            

            // Toggle visibility of items in context menu
            // depending on whether it is a MobiFlight Board or not
            // only upload of firmware is allowed for all boards
            // this is by default true
            addToolStripMenuItem.Enabled = isMobiFlightBoard;
            removeToolStripMenuItem.Enabled = isMobiFlightBoard & (e.Node.Level > 0);
            uploadToolStripMenuItem.Enabled = (parentNode.Nodes.Count > 0) || (parentNode.ImageKey == "Changed");
            openToolStripMenuItem.Enabled = isMobiFlightBoard;
            saveToolStripMenuItem.Enabled = parentNode.Nodes.Count > 0;
            saveAsToolStripMenuItem.Enabled = parentNode.Nodes.Count > 0;

            syncPanelWithSelectedDevice(e.Node);
        }

        private void firmwareArduinoIdePathTextBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void firmwareArduinoIdePathTextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();
            
            if (mobiflightCache.getConnectedModules().Count > 0 && 
                !MobiFlightFirmwareUpdater.IsValidArduinoIdePath(tb.Text))
            {
                displayError(tb, MainForm._tr("uiMessageInvalidArduinoIdePath"));
            }
            else
            {
                removeError(tb);
            }
            Properties.Settings.Default.ArduinoIdePath = tb.Text;
        }

        private void displayError(Control control, String message)
        {
            if (errorProvider1.Tag as Control != control)
                MessageBox.Show(message, MainForm._tr("Hint"));

            errorProvider1.SetError(
                    control,
                    message);
            errorProvider1.Tag = control;
        }

        private void removeError(Control control)
        {
            errorProvider1.Tag = null;
            errorProvider1.SetError(
                    control,
                    "");
        }

        private void regenerateSerialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = this.mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;
            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            try
            {
                module.GenerateNewSerial();
            }
            catch (FirmwareVersionTooLowException exc)
            {
                MessageBox.Show(MainForm._tr("uiMessageSettingsDialogFirmwareVersionTooLowException"), MainForm._tr("Hint"));
                return;
            }

            MobiFlightCache mobiflightCache = execManager.getMobiFlightModuleCache();
            mobiflightCache.RefreshModule(module);
            MobiFlightModuleInfo newInfo = module.GetInfo() as MobiFlightModuleInfo;
            mfModulesTreeView_initNode(newInfo, parentNode);
            syncPanelWithSelectedDevice(parentNode);
        }

        private void ArcazeModuleTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            TreeNode parentNode = e.Node;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;
            arcazeModuleSettingsGroupBox.Visible = true;
            _syncFromModuleSettings((parentNode.Tag as ArcazeModuleInfo).Serial);
        }

        private void numModulesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (ArcazeModuleTreeView.SelectedNode != null)
            {
                _syncToModuleSettings((ArcazeModuleTreeView.SelectedNode.Tag as ArcazeModuleInfo).Serial);
            }
        }

        private void numModulesNumericUpDown_Leave(object sender, EventArgs e)
        {

        }

        private void SettingsDialog_Shown(object sender, EventArgs e)
        {
            // Auto Update Functionality
            if (modulesForFlashing.Count > 0 || modulesForUpdate.Count > 0)
            {
                String arduinoIdePath = firmwareArduinoIdePathTextBox.Text;
                String firmwarePath = Directory.GetCurrentDirectory() + "\\firmware";

                if (!MobiFlightFirmwareUpdater.IsValidArduinoIdePath(arduinoIdePath))
                {
                    MessageBox.Show(
                        MainForm._tr("uiMessageFirmwareCheckPath"),
                        MainForm._tr("Hint"), MessageBoxButtons.OK);
                    return;
                }
              
            }
            foreach(MobiFlightModuleInfo moduleInfo in modulesForFlashing)
            {
                MobiFlightModule module = new MobiFlightModule(new MobiFlightModuleConfig() { ComPort = moduleInfo.Port, Type = moduleInfo.Type });
                updateFirmware(module);
            }

            foreach (MobiFlightModule module in modulesForUpdate)
            {
                updateFirmware(module);
            }
        }

        private void reloadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = this.mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;
            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            module.Config = null;
            module.LoadConfig();
            mfModulesTreeView_initNode(module.GetInfo() as MobiFlightModuleInfo, parentNode);
        }

        private TreeNode getModuleNode(TreeNode node)
        {
            TreeNode moduleNode = node;
            while (moduleNode.Level > 0) moduleNode = moduleNode.Parent;
            return moduleNode;
        }

        private MobiFlightModule getModuleFromTree()
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            if (parentNode == null) return null;

            parentNode = getModuleNode(parentNode);

            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            MobiFlight.Config.Config newConfig = new MobiFlight.Config.Config();

            foreach (TreeNode node in parentNode.Nodes)
            {
                newConfig.Items.Add(node.Tag as MobiFlight.Config.BaseDevice);
            }

            module.Config = newConfig;

            return module;
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
