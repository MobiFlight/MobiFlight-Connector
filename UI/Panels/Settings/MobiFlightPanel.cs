using MobiFlight.UI.Forms;
using MobiFlight.UI.Panels.Settings.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MobiFlightPanel : UserControl
    {
        public event EventHandler OnBeforeFirmwareUpdate;
        public event EventHandler OnAfterFirmwareUpdate;
        public event EventHandler OnModuleConfigChanged;

        Forms.FirmwareUpdateProcess FirmwareUpdateProcessForm = new Forms.FirmwareUpdateProcess();

        const bool StepperSupport = true;
        const bool ServoSupport = true;

        private int NumberOfModulesForFirmwareUpdate = 0;
        private String FirmwareUpdatePath = "";
        public List<MobiFlightModuleInfo> modulesForFlashing = new List<MobiFlightModuleInfo>();
        public List<MobiFlightModule> modulesForUpdate = new List<MobiFlightModule>();
        public bool MFModuleConfigChanged { get { return _IsModified(); } }

        MobiFlightCache mobiflightCache;

        public MobiFlightPanel()
        {
            InitializeComponent();
        }

        public void Init(MobiFlightCache mobiFlightCache)
        {
            this.mobiflightCache = mobiFlightCache;

            FirmwareUpdatePath = Properties.Settings.Default.ArduinoIdePathDefault;

            addStepperToolStripMenuItem.Visible = stepperToolStripMenuItem.Visible = StepperSupport;
            addServoToolStripMenuItem.Visible = servoToolStripMenuItem.Visible = ServoSupport;

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

            // setup the background worker for firmware update
            firmwareUpdateBackgroundWorker.DoWork += new DoWorkEventHandler(firmwareUpdateBackgroundWorker_DoWork);
            firmwareUpdateBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(firmwareUpdateBackgroundWorker_RunWorkerCompleted);
        }

        /// <summary>
        /// Initialize the MobiFlight Tab
        /// </summary>
        public void LoadSettings()
        {
#if MOBIFLIGHT

            // synchronize the toolbar icons
            mobiflightSettingsToolStrip.Enabled = false;
            uploadToolStripButton.Enabled = false;
            openToolStripButton.Enabled = true;
            saveToolStripButton.Enabled = false;
            addDeviceToolStripDropDownButton.Enabled = false;
            removeDeviceToolStripButton.Enabled = false;

            mfModulesTreeView.Nodes.Clear();
            try
            {
                foreach (MobiFlightModuleInfo module in mobiflightCache.GetDetectedArduinoModules())
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
                        switch (module.Type)
                        {
                            case MobiFlightModuleInfo.TYPE_ARDUINO_MICRO:
                                latestVersion = new Version(MobiFlightModuleInfo.LatestFirmwareMicro);
                                break;

                            case MobiFlightModuleInfo.TYPE_ARDUINO_UNO:
                                latestVersion = new Version(MobiFlightModuleInfo.LatestFirmwareUno);
                                break;
                        }
                        Version currentVersion = new Version(module.Version != "n/a" && module.Version != "" ? module.Version : "0.0.0");
                        if (currentVersion.CompareTo(latestVersion) < 0)
                        {
                            node.SelectedImageKey = node.ImageKey = "module-update";
                            node.ToolTipText = i18n._tr("uiMessageSettingsDlgOldFirmware");
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
                NewNode.Text = i18n._tr("none");
                NewNode.SelectedImageKey = NewNode.ImageKey = "module-arduino";
                mfModulesTreeView.Nodes.Add(NewNode);
                mfModulesTreeView.Enabled = false;
            }

            FwAutoUpdateCheckBox.Checked = Properties.Settings.Default.FwAutoUpdateCheck;
#endif
        }

        public void SaveSettings()
        {
            // MobiFlight Tab
            // only the Firmware Auto Check Update needs to be synchronized 
            Properties.Settings.Default.FwAutoUpdateCheck = FwAutoUpdateCheckBox.Checked;
        }

        private void updateFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = this.mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            if (this.mfModulesTreeView.SelectedNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            updateFirmware(module);
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
                MessageBox.Show(i18n._tr("uiMessageSettingsDialogFirmwareVersionTooLowException"), i18n._tr("Hint"));
                return;
            }

            mobiflightCache.RefreshModule(module);
            MobiFlightModuleInfo newInfo = module.GetInfo() as MobiFlightModuleInfo;
            mfModulesTreeView_initNode(newInfo, parentNode);
            syncPanelWithSelectedDevice(parentNode);
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

        private TreeNode mfModulesTreeView_initNode(MobiFlightModuleInfo module, TreeNode node)
        {
            node.Text = module.Name;
            if (module.HasMfFirmware())
            {
                node.SelectedImageKey = node.ImageKey = "module";
                node.Tag = mobiflightCache.GetModule(module);
                node.Nodes.Clear();

                if (null == (node.Tag as MobiFlightModule).Config) return node;

                foreach (MobiFlight.Config.BaseDevice device in (node.Tag as MobiFlightModule).Config.Items)
                {
                    if (device == null) continue; // Happens if working on an older firmware version. Ok.

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
                    panel = new MFModulePanel((selectedNode.Tag as MobiFlightModule));
                    (panel as MFModulePanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                }
                else
                {
                    TreeNode parentNode = mfModulesTreeView.SelectedNode;
                    if (parentNode == null) return;
                    while (parentNode.Level > 0) parentNode = parentNode.Parent;
                    MobiFlightModule module = getVirtualModuleFromTree();

                    MobiFlight.Config.BaseDevice dev = (selectedNode.Tag as MobiFlight.Config.BaseDevice);
                    switch (dev.Type)
                    {
                        case DeviceType.LedModule:
                            panel = new MFLedSegmentPanel(dev as MobiFlight.Config.LedModule, module.GetPins());
                            (panel as MFLedSegmentPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Stepper:
                            panel = new MFStepperPanel(dev as MobiFlight.Config.Stepper, module.GetPins());
                            (panel as MFStepperPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;
                            
                        case DeviceType.Servo:
                            panel = new MFServoPanel(dev as MobiFlight.Config.Servo, module.GetPins());
                            (panel as MFServoPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.AnalogInput:
                            panel = new MFAnalogPanel(dev as MobiFlight.Config.AnalogInput, module.GetPins());
                            (panel as MFAnalogPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Button:
                            panel = new MFButtonPanel(dev as MobiFlight.Config.Button, module.GetPins());
                            (panel as MFButtonPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;
                            
                        case DeviceType.Encoder:
                            panel = new MFEncoderPanel(dev as MobiFlight.Config.Encoder, module.GetPins());
                            (panel as MFEncoderPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.Output:
                            panel = new MFOutputPanel(dev as MobiFlight.Config.Output, module.GetPins(), module.Type);
                            (panel as MFOutputPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.LcdDisplay:
                            panel = new MFLcddDisplayPanel(dev as MobiFlight.Config.LcdDisplay, module.GetPins());
                            (panel as MFLcddDisplayPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;
                        
                        case DeviceType.ShiftRegister:
                            panel = new MFShiftRegisterPanel(dev as MobiFlight.Config.ShiftRegister, module.GetPins());
                            (panel as MFShiftRegisterPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;
                            // output
                    }
                }

                if (panel != null)
                {
                    panel.Padding = new Padding(2, 0, 0, 0);
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
        /// EventHandler to add a selected device to the current module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDeviceTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MobiFlight.Config.BaseDevice cfgItem = null;
            MobiFlightModule tempModule = null;
            try
            {
                cfgItem = null;
                tempModule = getVirtualModuleFromTree();
                tempModule.LoadConfig();
                Dictionary<String, int> statistics = tempModule.GetConnectedDevicesStatistics();

                switch ((sender as ToolStripMenuItem).Name)
                {
                    case "servoToolStripMenuItem":
                    case "addServoToolStripMenuItem":
                        if (statistics[MobiFlightServo.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxServos)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightServo.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxServos);
                        }

                        cfgItem = new MobiFlight.Config.Servo();
                        (cfgItem as MobiFlight.Config.Servo).DataPin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        break;
                    case "stepperToolStripMenuItem":
                    case "addStepperToolStripMenuItem":
                        if (statistics[MobiFlightStepper.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxSteppers)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightStepper.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxSteppers);
                        }

                        cfgItem = new MobiFlight.Config.Stepper();
                        (cfgItem as MobiFlight.Config.Stepper).Pin1 = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        (cfgItem as MobiFlight.Config.Stepper).Pin2 = getVirtualModuleFromTree().GetFreePins().ElementAt(1).Pin.ToString();
                        (cfgItem as MobiFlight.Config.Stepper).Pin3 = getVirtualModuleFromTree().GetFreePins().ElementAt(2).Pin.ToString();
                        (cfgItem as MobiFlight.Config.Stepper).Pin4 = getVirtualModuleFromTree().GetFreePins().ElementAt(3).Pin.ToString();
                        //(cfgItem as MobiFlight.Config.Stepper).BtnPin = getModuleFromTree().GetFreePins().ElementAt(4).ToString();
                        break;
                    case "ledOutputToolStripMenuItem":
                    case "addOutputToolStripMenuItem":
                        if (statistics[MobiFlightOutput.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxOutputs)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightOutput.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxOutputs);
                        }

                        cfgItem = new MobiFlight.Config.Output();
                        (cfgItem as MobiFlight.Config.Output).Pin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        break;
                    case "ledSegmentToolStripMenuItem":
                    case "addLedModuleToolStripMenuItem":
                        if (statistics[MobiFlightLedModule.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxLedSegments)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightLedModule.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxLedSegments);
                        }

                        cfgItem = new MobiFlight.Config.LedModule();
                        (cfgItem as MobiFlight.Config.LedModule).DinPin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        (cfgItem as MobiFlight.Config.LedModule).ClkPin = getVirtualModuleFromTree().GetFreePins().ElementAt(1).Pin.ToString();
                        (cfgItem as MobiFlight.Config.LedModule).ClsPin = getVirtualModuleFromTree().GetFreePins().ElementAt(2).Pin.ToString();
                        break;
                    case "analogDeviceToolStripMenuItem1":
                    case "analogDeviceToolStripMenuItem":
                        if (statistics[MobiFlightAnalogInput.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxAnalogInputs)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightAnalogInput.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxAnalogInputs);
                        }
                        cfgItem = new MobiFlight.Config.AnalogInput();
                        (cfgItem as MobiFlight.Config.AnalogInput).Pin = getVirtualModuleFromTree().GetFreePins().FindAll(x=>x.isAnalog==true).ElementAt(0).Pin.ToString();
                        break;                        
                    case "buttonToolStripMenuItem":
                    case "addButtonToolStripMenuItem":
                        if (statistics[MobiFlightButton.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxButtons)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightButton.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxButtons);
                        }

                        cfgItem = new MobiFlight.Config.Button();
                        (cfgItem as MobiFlight.Config.Button).Pin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        break;
                    case "encoderToolStripMenuItem":
                    case "addEncoderToolStripMenuItem":
                        if (statistics[MobiFlightEncoder.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxEncoders)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightEncoder.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxEncoders);
                        }

                        cfgItem = new MobiFlight.Config.Encoder();
                        (cfgItem as MobiFlight.Config.Encoder).PinLeft = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        (cfgItem as MobiFlight.Config.Encoder).PinRight = getVirtualModuleFromTree().GetFreePins().ElementAt(1).Pin.ToString();
                        break;
                    case "LcdDisplayToolStripMenuItem":
                    case "addLcdDisplayToolStripMenuItem":
                        if (statistics[MobiFlightLcdDisplay.TYPE] == tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxLcdI2C)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightLcdDisplay.TYPE, tempModule.ToMobiFlightModuleInfo().GetCapabilities().MaxLcdI2C);
                        }

                        cfgItem = new MobiFlight.Config.LcdDisplay();
                        // does not deal yet with these kind of pins because we use I2C
                        break;

                    case "ShiftRegisterToolStripMenuItem":
                    case "addShiftRegisterToolStripMenuItem":
                        cfgItem = new MobiFlight.Config.ShiftRegister();
                        (cfgItem as MobiFlight.Config.ShiftRegister).DataPin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        (cfgItem as MobiFlight.Config.ShiftRegister).ClockPin = getVirtualModuleFromTree().GetFreePins().ElementAt(1).ToString();
                        (cfgItem as MobiFlight.Config.ShiftRegister).LatchPin = getVirtualModuleFromTree().GetFreePins().ElementAt(2).ToString();                        
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

                mfModulesTreeView.SelectedNode = newNode;
                syncPanelWithSelectedDevice(newNode);
            }
            catch (MaximumDeviceNumberReachedMobiFlightException ex)
            {
                MessageBox.Show(String.Format(i18n._tr("uiMessageMaxNumberOfDevicesReached"), ex.MaxNumber, ex.DeviceType, tempModule.Type),
                                i18n._tr("uiMessageNotEnoughPinsHint"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(i18n._tr("uiMessageNotEnoughPinsMessage"),
                                i18n._tr("uiMessageNotEnoughPinsHint"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        String.Format(i18n._tr("uiMessageDeviceNameContainsInvalidCharsOrTooLong"),
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
                    MessageBox.Show(i18n._tr("uiMessageDeviceNameAlreadyUsed"), i18n._tr("Hint"), MessageBoxButtons.OK);
                }

                mfModulesTreeView.SelectedNode.Text = (sender as MobiFlight.Config.BaseDevice).Name;
            }
            else
            {
                mfModulesTreeView.SelectedNode.Text = (sender as MobiFlight.MobiFlightModule).Name;
            }

            parentNode.ImageKey = "Changed";
            parentNode.SelectedImageKey = "Changed";

            OnModuleConfigChanged?.Invoke(sender, null);
        }


        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            MobiFlightModule module = parentNode.Tag as MobiFlightModule;
            MobiFlight.Config.Config newConfig = new MobiFlight.Config.Config();
            newConfig.ModuleName = module.Name;

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

                if (newConfig.ModuleName != null && newConfig.ModuleName != "")
                {
                    parentNode.Text = (parentNode.Tag as MobiFlightModule).Name = newConfig.ModuleName;

                }

                parentNode.Nodes.Clear();

                foreach (MobiFlight.Config.BaseDevice device in newConfig.Items)
                {
                    TreeNode newNode = new TreeNode(device.Name);
                    newNode.Tag = device;
                    newNode.SelectedImageKey = newNode.ImageKey = device.Type.ToString();
                    parentNode.Nodes.Add(newNode);
                }

                parentNode.ImageKey = "Changed";
                parentNode.SelectedImageKey = "Changed";
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

        /// <summary>
        /// EventHandler for upload button, this uploads the new config to the module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void uploadToolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(i18n._tr("uiMessageUploadConfigurationConfirm"),
                                i18n._tr("uiMessageUploadConfigurationHint"),
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

            // Prevent upload from too long configs that would exceed the available EEPROM size
            String LogMessage = String.Join("", module.Config.ToInternal(module.MaxMessageSize).ToArray());
            if (LogMessage.Length > module.EepromSize)
            {
                MessageBox.Show(i18n._tr("uiMessageUploadConfigurationTooLong"),
                                i18n._tr("uiMessageUploadConfigurationHint"),
                                MessageBoxButtons.OK);
                return;
            }

            Log.Instance.log("Uploading config: " + LogMessage, LogSeverity.Info);

            bool uploadResult = false;

            ConfigUploadProgress form = new ConfigUploadProgress();
            form.StartPosition = FormStartPosition.CenterParent;

            await Task.Run(new System.Action(() => {
                this.BeginInvoke(new System.Action(() => { form.ShowDialog(); }));
                form.Progress = 25;
                form.Status = "Uploading.";
                uploadResult = module.SaveConfig();
                form.Progress = 50;
                module.Config = null;
                form.Status = "Resetting Board.";
                module.ResetBoard();
                form.Progress = 75;

                form.Status = "Loading Config.";
                module.LoadConfig();
                form.Progress = 100;

                mobiflightCache.updateConnectedModuleName(module);
            })).ContinueWith(new Action<Task>(task =>
            {
                // Close modal dialog
                // - No need to use BeginInvoke here
                //   because ContinueWith was called with TaskScheduler.FromCurrentSynchronizationContext()
                form.Close();
            }), TaskScheduler.FromCurrentSynchronizationContext());

            if (uploadResult)
            {
                MessageBox.Show(i18n._tr("uiMessageUploadConfigurationFinished"),
                                i18n._tr("uiMessageUploadConfigurationHint"),
                                MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(i18n._tr("uiMessageUploadConfigurationFinishedWithError"),
                                i18n._tr("uiMessageUploadConfigurationHint"),
                                MessageBoxButtons.OK);
            }
            parentNode.ImageKey = "";
            parentNode.SelectedImageKey = "";
        }

        protected bool _IsModified()
        {
            foreach (TreeNode node in mfModulesTreeView.Nodes)
            {
                if (node.ImageKey == "Changed") return true;
            }
            return false;
        }

        private TreeNode getModuleNode(TreeNode node)
        {
            TreeNode moduleNode = node;
            while (moduleNode.Level > 0) moduleNode = moduleNode.Parent;
            return moduleNode;
        }

        private MobiFlightModule getVirtualModuleFromTree()
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;
            if (parentNode == null) return null;

            parentNode = getModuleNode(parentNode);

            MobiFlightModule module = new MobiFlightModule(
                new MobiFlightModuleConfig()
                {
                    ComPort = (parentNode.Tag as MobiFlightModule).Port,
                    Type = (parentNode.Tag as MobiFlightModule).Type
                }
            );
            
            MobiFlight.Config.Config newConfig = new MobiFlight.Config.Config();
            foreach (TreeNode node in parentNode.Nodes)
            {
                newConfig.Items.Add(node.Tag as MobiFlight.Config.BaseDevice);
            }

            module.Config = newConfig;


            return module;
        }

        private void MobiFlightPanel_Load(object sender, EventArgs e)
        {
            // Auto Update Functionality
            if (modulesForFlashing.Count > 0 || modulesForUpdate.Count > 0)
            {
                String arduinoIdePath = FirmwareUpdatePath;
                String firmwarePath = Directory.GetCurrentDirectory() + "\\firmware";

                if (!MobiFlightFirmwareUpdater.IsValidArduinoIdePath(arduinoIdePath))
                {
                    MessageBox.Show(
                        i18n._tr("uiMessageFirmwareCheckPath"),
                        i18n._tr("Hint"), MessageBoxButtons.OK);
                    return;
                }

            }
            foreach (MobiFlightModuleInfo moduleInfo in modulesForFlashing)
            {
                MobiFlightModule module = new MobiFlightModule(new MobiFlightModuleConfig() { ComPort = moduleInfo.Port, Type = moduleInfo.Type });
                updateFirmware(module);
            }

            foreach (MobiFlightModule module in modulesForUpdate)
            {
                updateFirmware(module);
            }
        }

        protected void updateFirmware(MobiFlightModule module)
        {
            String arduinoIdePath = Properties.Settings.Default.ArduinoIdePathDefault;
            String firmwarePath = Directory.GetCurrentDirectory() + "\\firmware";

            if (!MobiFlightFirmwareUpdater.IsValidArduinoIdePath(arduinoIdePath))
            {
                MessageBox.Show(
                    i18n._tr("uiMessageFirmwareCheckPath"),
                    i18n._tr("Hint"), MessageBoxButtons.OK);
                return;
            }

            OnBeforeFirmwareUpdate?.Invoke(module, null);
            module.Disconnect();

            MobiFlightFirmwareUpdater.ArduinoIdePath = arduinoIdePath;
            MobiFlightFirmwareUpdater.FirmwarePath = firmwarePath;

            NumberOfModulesForFirmwareUpdate++;
            firmwareUpdateBackgroundWorker.RunWorkerAsync(module);
            FirmwareUpdateProcessForm.ShowDialog();
        }

        private TreeNode findNodeByPort(string port)
        {
            TreeNode resultNode = null;
            foreach (TreeNode node in mfModulesTreeView.Nodes)
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

        void firmwareUpdateBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NumberOfModulesForFirmwareUpdate--;
            if (NumberOfModulesForFirmwareUpdate == 0)
                FirmwareUpdateProcessForm.Hide();

            if (e.Error != null)
            {
                MessageBox.Show(
                    i18n._tr("uiMessageFirmwareUploadError"),
                    i18n._tr("Hint"), 
                    MessageBoxButtons.OK
                );
                return;
            }

            // update presentation in treeView
            MobiFlightModule module = (MobiFlightModule)e.Result;

            module.Connect();
            MobiFlightModuleInfo newInfo = module.GetInfo() as MobiFlightModuleInfo;
            mobiflightCache.RefreshModule(module);

            OnAfterFirmwareUpdate?.Invoke(module, null);

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
                    i18n._tr("uiMessageFirmwareUploadSuccessful"),
                    i18n._tr("Hint"),
                    MessageBoxButtons.OK
                );
        }


        private void displayError(Control control, String message)
        {
            if (errorProvider1.Tag as Control != control)
                MessageBox.Show(message, i18n._tr("Hint"));

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
    }
}
