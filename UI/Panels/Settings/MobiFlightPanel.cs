using MobiFlight.UI.Dialogs;
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

        
        private String FirmwareUpdatePath = "";
        public List<MobiFlightModuleInfo> modulesForFlashing = new List<MobiFlightModuleInfo>();
        public List<MobiFlightModule> modulesForUpdate = new List<MobiFlightModule>();
        public MobiFlightModuleInfo PreselectedMobiFlightBoard { get; internal set; }
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
            mfTreeViewImageList.Images.Add(DeviceType.MuxDriver.ToString(), MobiFlight.Properties.Resources.mux_driver);
            mfTreeViewImageList.Images.Add("Changed", MobiFlight.Properties.Resources.module_changed);
            mfTreeViewImageList.Images.Add("Changed-arcaze", MobiFlight.Properties.Resources.arcaze_changed);
            mfTreeViewImageList.Images.Add("new-arcaze", MobiFlight.Properties.Resources.arcaze_new);
            //mfModulesTreeView.ImageList = mfTreeViewImageList;
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
                        Version latestVersion = new Version(module.Board.Info.LatestFirmwareVersion);

                        Version currentVersion;
                        try
                        {
                            currentVersion = new Version(module.Version != null ? module.Version : "0.0.0");
                        }
                        catch (Exception ex)
                        {
                            currentVersion = new Version("0.0.0");
                        }
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

            mfModulesTreeView.Select();
            
            FwAutoInstallCheckBox.Checked = Properties.Settings.Default.FwAutoUpdateCheck;
#endif
        }

        public void SaveSettings()
        {
            // MobiFlight Tab
            // only the Firmware Auto Check Update needs to be synchronized 
            Properties.Settings.Default.FwAutoUpdateCheck = FwAutoInstallCheckBox.Checked;
        }

        private void updateFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = this.mfModulesTreeView.SelectedNode;
            if (parentNode == null) return;

            if (this.mfModulesTreeView.SelectedNode == null) return;

            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            MobiFlightModule module = parentNode.Tag as MobiFlightModule;

            List<MobiFlightModule> modules = new List<MobiFlightModule>();
            modules.Add(module);
            UpdateModules(modules);
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

        private void mfModulesTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            mfModulesTreeView.SelectedNode = e.Node;
        }

        private void mfModulesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null) return;
            mfModulesTreeView.SelectedNode = e.Node;
        }

        private void mfModulesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            TreeNode parentNode = e.Node;
            while (parentNode.Level > 0) parentNode = parentNode.Parent;

            mfSettingsPanel.Controls.Clear();
            if (parentNode.Tag == null) return;

            bool isMobiFlightBoard = (parentNode.Tag as MobiFlightModule).HasMfFirmware();

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
                node.Tag = new MobiFlightModule(module.Port, module.Board);
            }

            return node;
        }

        /// <summary>
        /// Show the necessary options for a selected device which is attached to a MobiFlight module
        /// </summary>
        /// <param name="selectedNode"></param>
        private void syncPanelWithSelectedDevice(TreeNode selectedNode)
        {
            if (selectedNode == null) return;
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
                            panel = new MFOutputPanel(dev as MobiFlight.Config.Output, module.GetPins(), module.Board);
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

                        case DeviceType.InputShiftRegister:
                            panel = new MFInputShiftRegisterPanel(dev as MobiFlight.Config.InputShiftRegister, module.GetPins());
                            (panel as MFInputShiftRegisterPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.DigInputMux:
                            panel = new MFDigInputMuxPanel(dev as MobiFlight.Config.DigInputMux, module.GetPins());
                            (panel as MFDigInputMuxPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
                            break;

                        case DeviceType.MuxDriver:
                            panel = new MFMuxDriverPanel (dev as MobiFlight.Config.MuxDriver, module.GetPins());
                            (panel as MFMuxDriverPanel).Changed += new EventHandler(mfConfigDeviceObject_changed);
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
        /// Helper function to return the node corresponding to current module
        /// </summary>
        ///<returns>TreeNode of the module of current selected item; null if no item selected.</returns>
        /// 
        private TreeNode getNodeOfCurrentModule()
        {
            TreeNode parentNode = mfModulesTreeView.SelectedNode;   // (not yet parent)
            if (parentNode == null) {
                // should never happen... when creating a new device in a module, a selection is either made by the user or forced
                return null;
                // otherwise: how can we determine which module we're in within the TreeView?
            }
            // Shift parentNode to actual root parent
            while (parentNode.Level > 0) parentNode = parentNode.Parent;
            return parentNode;
        }

        /// <summary>
        /// Helper function to add a node in the current module structure
        /// </summary>
        ///<returns>Newly created TreeNode if correctly added, null otherwise</returns>
        /// <param name="device">Device to add as node</param>
        /// <param name="pos">Position in the list (null = last)</param>
        /// 
        private TreeNode addDeviceToModule(MobiFlight.Config.BaseDevice device, int pos = -1)
        {
            bool added = false;

            TreeNode parentNode = getNodeOfCurrentModule();
            if (parentNode == null) return null;

            // Build a list of all names in the tree
            List<String> NodeNames = new List<String>();
            foreach (TreeNode node in parentNode.Nodes) {
                NodeNames.Add(node.Text);
            }
            // Use the list of names for verification to build a new unique name
            device.Name = MobiFlightModule.GenerateUniqueDeviceName(NodeNames.ToArray(), device.Name);

            // Build new node containing the device
            TreeNode newNode = new TreeNode(device.Name);
            newNode.SelectedImageKey = newNode.ImageKey = device.Type.ToString();
            newNode.Tag = device;

            // Add newly built node to the tree
            if (pos < 0 || pos > parentNode.Nodes.Count) {
                parentNode.Nodes.Add(newNode);
            } else {
                parentNode.Nodes.Insert(pos, newNode);
            }
            parentNode.ImageKey = "Changed";
            parentNode.SelectedImageKey = "Changed";

            // Make it the current selected one
            mfModulesTreeView.SelectedNode = newNode;
            return newNode;
        }

        /// <summary>
        /// Tell whether a device requires the presence of a MuxDriver 
        /// </summary>
        /// <returns>true if required, false otherwise</returns>
        /// This function could be turned to a method of MobiFlight.Config.BaseDevice
        private bool requiresMuxDriver(MobiFlight.Config.BaseDevice device)
        {
            // Currently, the only device type requiring a mux driver is the Digital Input Mux;
            // in a prospective improvement, multiplexers can easily be used to route many other devices.
            // These devices may basically be the exact same ones that are in use now directly attached,
            // provided that a "channel" attribute is added. A device assigned to channel #N will be polled
            // by the firmware (and correctly routed) by setting the mux driver channel to N.
            // When/if this will be implemented, the following condition will only have to be modified to check
            // if any device has a non-null "channel" value.
            return (device.Type == DeviceType.DigInputMux);
        }

        /// <summary>
        /// Helper function to check whether a MuxDriver device exists in the current module
        /// </summary>
        /// <returns>Object if existing, null otherwise</returns>
        private TreeNode findMuxDriverInTree()
        {
            // if there is a muxDriver, it is supposed to be the first element of the tree,
            // but to be safe we check if there's one anywhere regardless of its position.
            TreeNode parentNode = getNodeOfCurrentModule();
            if (parentNode == null) return null;

            foreach (TreeNode node in parentNode.Nodes) {
                if ((node.Tag as MobiFlight.Config.BaseDevice).Type == DeviceType.MuxDriver) {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Helper function to check whether a MuxDriver device exists in the current module
        /// and add it if required
        /// </summary>
        /// <returns>true if added or existing, false if it can't be created</returns>
        private bool tryAddMuxDriverToModule()
        {
            // if there is a muxDriver in the tree, we're already done
            if (findMuxDriverInTree() != null) return true;

            // otherwise see if we can create one
            List<MobiFlightPin> freePins = getVirtualModuleFromTree().GetFreePins();
            if(freePins.Count < 4) {
                MessageBox.Show(i18n._tr("uiMessageNotEnoughPinsMessage"),
                                i18n._tr("uiMessageNotEnoughPinsHint"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            MobiFlight.Config.MuxDriver newMux = new MobiFlight.Config.MuxDriver();
            for(var i=0; i<4; i++) {
                newMux.PinSx[i] = freePins.ElementAt(i).ToString();
            }

            addDeviceToModule(newMux, 0);   // Add at the beginning of the list
            
            // Do not select newly added item (the invoking item will have the selection) 
            return true;
        }

        /// <summary>
        /// Helper function to check whether a MuxDriver device exists in the current module
        /// and if it's still required; otherwise it is removed.
        /// </summary>
        /// <returns>true if removed or already missing, false if still required</returns>
        private bool tryRemoveMuxDriverFromModule()
        {
            TreeNode muxNode = findMuxDriverInTree();
            // if there is no muxDriver in the tree, we're already done
            if (muxNode == null) return true;

            TreeNode parentNode = getNodeOfCurrentModule();
            if (parentNode == null) return true;
            foreach (TreeNode node in parentNode.Nodes) {
                if (requiresMuxDriver(node.Tag as MobiFlight.Config.BaseDevice)) {
                    return false;
                }
            }
            mfModulesTreeView.Nodes.Remove(muxNode);
            // do not bother to change parentNode.ImageKey: this is only a helper function, therefore there is
            // a caller that is removing another device and will take care of that.

            return true;
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
                if (tempModule == null) return;
                tempModule.LoadConfig();
                Dictionary<String, int> statistics = tempModule.GetConnectedDevicesStatistics();

                switch ((sender as ToolStripMenuItem).Name)
                {
                    case "servoToolStripMenuItem":
                    case "addServoToolStripMenuItem":
                        if (statistics[MobiFlightServo.TYPE] == tempModule.Board.ModuleLimits.MaxServos)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightServo.TYPE, tempModule.Board.ModuleLimits.MaxServos);
                        }

                        cfgItem = new MobiFlight.Config.Servo();
                        (cfgItem as MobiFlight.Config.Servo).DataPin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        break;
                    case "stepperToolStripMenuItem":
                    case "addStepperToolStripMenuItem":
                        if (statistics[MobiFlightStepper.TYPE] == tempModule.Board.ModuleLimits.MaxSteppers)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightStepper.TYPE, tempModule.Board.ModuleLimits.MaxSteppers);
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
                        if (statistics[MobiFlightOutput.TYPE] == tempModule.Board.ModuleLimits.MaxOutputs)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightOutput.TYPE, tempModule.Board.ModuleLimits.MaxOutputs);
                        }

                        cfgItem = new MobiFlight.Config.Output();
                        (cfgItem as MobiFlight.Config.Output).Pin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        break;
                    case "ledSegmentToolStripMenuItem":
                    case "addLedModuleToolStripMenuItem":
                        if (statistics[MobiFlightLedModule.TYPE] == tempModule.Board.ModuleLimits.MaxLedSegments)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightLedModule.TYPE, tempModule.Board.ModuleLimits.MaxLedSegments);
                        }

                        cfgItem = new MobiFlight.Config.LedModule();
                        (cfgItem as MobiFlight.Config.LedModule).DinPin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        (cfgItem as MobiFlight.Config.LedModule).ClkPin = getVirtualModuleFromTree().GetFreePins().ElementAt(1).Pin.ToString();
                        (cfgItem as MobiFlight.Config.LedModule).ClsPin = getVirtualModuleFromTree().GetFreePins().ElementAt(2).Pin.ToString();
                        break;
                    case "analogDeviceToolStripMenuItem1":
                    case "analogDeviceToolStripMenuItem":
                        if (statistics[MobiFlightAnalogInput.TYPE] == tempModule.Board.ModuleLimits.MaxAnalogInputs)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightAnalogInput.TYPE, tempModule.Board.ModuleLimits.MaxAnalogInputs);
                        }
                        cfgItem = new MobiFlight.Config.AnalogInput();
                        (cfgItem as MobiFlight.Config.AnalogInput).Pin = getVirtualModuleFromTree().GetFreePins().FindAll(x=>x.isAnalog==true).ElementAt(0).Pin.ToString();
                        break;                        
                    case "buttonToolStripMenuItem":
                    case "addButtonToolStripMenuItem":
                        if (statistics[MobiFlightButton.TYPE] == tempModule.Board.ModuleLimits.MaxButtons)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightButton.TYPE, tempModule.Board.ModuleLimits.MaxButtons);
                        }

                        cfgItem = new MobiFlight.Config.Button();
                        (cfgItem as MobiFlight.Config.Button).Pin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        break;
                    case "encoderToolStripMenuItem":
                    case "addEncoderToolStripMenuItem":
                        if (statistics[MobiFlightEncoder.TYPE] == tempModule.Board.ModuleLimits.MaxEncoders)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightEncoder.TYPE, tempModule.Board.ModuleLimits.MaxEncoders);
                        }

                        cfgItem = new MobiFlight.Config.Encoder();
                        (cfgItem as MobiFlight.Config.Encoder).PinLeft = getVirtualModuleFromTree().GetFreePins().ElementAt(0).Pin.ToString();
                        (cfgItem as MobiFlight.Config.Encoder).PinRight = getVirtualModuleFromTree().GetFreePins().ElementAt(1).Pin.ToString();
                        break;
                    case "inputShiftRegisterToolStripMenuItem":
                    case "addInputShiftRegisterToolStripMenuItem":
                        if (statistics[MobiFlightInputShiftRegister.TYPE] == tempModule.Board.ModuleLimits.MaxInputShifters)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightInputShiftRegister.TYPE, tempModule.Board.ModuleLimits.MaxInputShifters);
                        }
                        cfgItem = new MobiFlight.Config.InputShiftRegister();
                        (cfgItem as MobiFlight.Config.InputShiftRegister).DataPin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        (cfgItem as MobiFlight.Config.InputShiftRegister).ClockPin = getVirtualModuleFromTree().GetFreePins().ElementAt(1).ToString();
                        (cfgItem as MobiFlight.Config.InputShiftRegister).LatchPin = getVirtualModuleFromTree().GetFreePins().ElementAt(2).ToString();
                        break;
                    case "digInputMuxToolStripMenuItem":
                    case "addDigInputMuxToolStripMenuItem":
                        if (statistics[MobiFlightDigInputMux.TYPE] == tempModule.Board.ModuleLimits.MaxDigInputMuxes) {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightDigInputMux.TYPE, tempModule.Board.ModuleLimits.MaxDigInputMuxes);
                        }
                        tryAddMuxDriverToModule();
                        cfgItem = new MobiFlight.Config.DigInputMux();
                        (cfgItem as MobiFlight.Config.DigInputMux).DataPin = getVirtualModuleFromTree().GetFreePins().ElementAt(0).ToString();
                        break;
                    case "LcdDisplayToolStripMenuItem":
                    case "addLcdDisplayToolStripMenuItem":
                        if (statistics[MobiFlightLcdDisplay.TYPE] == tempModule.Board.ModuleLimits.MaxLcdI2C)
                        {
                            throw new MaximumDeviceNumberReachedMobiFlightException(MobiFlightLcdDisplay.TYPE, tempModule.Board.ModuleLimits.MaxLcdI2C);
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

                TreeNode newNode = addDeviceToModule(cfgItem);
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
            if (node.Level == 0) return;    // removing a device, not a module

            TreeNode parentNode = getNodeOfCurrentModule();

            mfModulesTreeView.Nodes.Remove(node);
            
            // if we're removing a device that uses the multiplexer driver (currently DigInputMux only),
            // check if any other device uses it, and otherwise make sure to remove that too 
            if (requiresMuxDriver(node.Tag as MobiFlight.Config.BaseDevice)) {
                tryRemoveMuxDriverFromModule();
            }

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
            String LogMessage = String.Join("", module.Config.ToInternal(module.Board.Connection.MessageSize).ToArray());
            if (LogMessage.Length > module.Board.Connection.EEPROMSize)
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
                TimeoutMessageDialog tmd = new TimeoutMessageDialog();
                tmd.StartPosition = FormStartPosition.CenterParent;
                tmd.DefaultDialogResult = DialogResult.Cancel;
                tmd.Message = i18n._tr("uiMessageUploadConfigurationFinished");
                tmd.Text = i18n._tr("uiMessageUploadConfigurationHint");
                tmd.HasCancelButton = false;
                tmd.ShowDialog();
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

            MobiFlightModule module = new MobiFlightModule((parentNode.Tag as MobiFlightModule).Port, (parentNode.Tag as MobiFlightModule).Board);
            
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

            List<MobiFlightModule> modules = new List<MobiFlightModule>();

            foreach (MobiFlightModuleInfo moduleInfo in modulesForFlashing)
            {
                MobiFlightModule module = new MobiFlightModule(moduleInfo.Port, moduleInfo.Board);
                modules.Add(module);
            }

            foreach (MobiFlightModule module in modulesForUpdate)
            {
                modules.Add(module);
            }

            UpdateModules(modules);

            if (PreselectedMobiFlightBoard != null)
                PreSelectMobiFlightBoard(PreselectedMobiFlightBoard);
        }

        private void PreSelectMobiFlightBoard(MobiFlightModuleInfo preselectedMobiFlightBoard)
        {
            TreeNode resultNode = findNodeByPort(preselectedMobiFlightBoard.Port);

            if (resultNode != null)
            {
                resultNode.Expand();
                mfModulesTreeView.SelectedNode = resultNode;
            }

            PreselectedMobiFlightBoard = null;
        }

        private void UpdateModules(List<MobiFlightModule> modules)
        {
            if (modules.Count == 0) return;
            FirmwareUpdateProcessForm.ClearModules();
            modules.ForEach(
                module => FirmwareUpdateProcessForm.AddModule(module)
            );
            
            FirmwareUpdateProcessForm.OnBeforeFirmwareUpdate -= FirmwareUpdateProcessForm_OnBeforeFirmwareUpdate;
            FirmwareUpdateProcessForm.OnBeforeFirmwareUpdate += FirmwareUpdateProcessForm_OnBeforeFirmwareUpdate;
            FirmwareUpdateProcessForm.OnAfterFirmwareUpdate -= FirmwareUpdateProcessForm_OnAfterFirmwareUpdate;
            FirmwareUpdateProcessForm.OnAfterFirmwareUpdate += FirmwareUpdateProcessForm_OnAfterFirmwareUpdate;
            FirmwareUpdateProcessForm.OnFinished -= FirmwareUpdateProcessForm_OnFinished;
            FirmwareUpdateProcessForm.OnFinished += FirmwareUpdateProcessForm_OnFinished;
            FirmwareUpdateProcessForm.ShowDialog();
        }

        private void FirmwareUpdateProcessForm_OnFinished(List<MobiFlightModule> modules)
        {
            String Message = i18n._tr("uiMessageFirmwareUploadSuccessful");


            TimeoutMessageDialog tmd = new TimeoutMessageDialog();
            tmd.Width = FirmwareUpdateProcessForm.Width;
            tmd.Height = FirmwareUpdateProcessForm.Height;
            tmd.HasCancelButton = false;
            tmd.StartPosition = FormStartPosition.CenterParent;
            tmd.Text = i18n._tr("uiMessageFirmwareUploadTitle");

            if (modules.Count > 0)
            {
                if (modules.Count > 4)
                    tmd.Height += ((modules.Count - 4) * 12);

                List<string> ModuleNames = new List<string>();
                modules.ForEach(module => ModuleNames.Add(module.Name + " (" + module.Port + ")"));

                Message = string.Format(
                                    i18n._tr("uiMessageFirmwareUploadError"),
                                    string.Join("\n", ModuleNames)
                                    ); ;
            }

            tmd.Message = Message;           
            tmd.ShowDialog();
        }

        private void FirmwareUpdateProcessForm_OnBeforeFirmwareUpdate(object sender, EventArgs e)
        {
            OnBeforeFirmwareUpdate?.Invoke(sender, e);
        }

        private void FirmwareUpdateProcessForm_OnAfterFirmwareUpdate(object sender, EventArgs e)
        {
            // update presentation in treeView
            MobiFlightModule module = (MobiFlightModule)sender;

            // If the update fails for some reason, e.g. the board definition file was missing the settings for the
            // update, then module will be null.
            if (module == null)
            {
                return;
            }           

            module.Connect();
            MobiFlightModuleInfo newInfo = module.GetInfo() as MobiFlightModuleInfo;

            // Issue 611
            // If the board definition file is correct but the firmware failed to flash and the result is
            // a bare module with no MobiFlight firmware on it then the serial number will be null
            // and the module should not be refreshed.
            if (String.IsNullOrEmpty(newInfo.Serial))
            {
                return;
            }

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
