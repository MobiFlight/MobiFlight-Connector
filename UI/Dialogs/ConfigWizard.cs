using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;
using MobiFlight.Base;
using MobiFlight.UI.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class ConfigWizard : Form
    {
        public event EventHandler PreconditionTreeNodeChanged;
        public event EventHandler SettingsDialogRequested;

        static int lastTabActive = 0;

        ExecutionManager _execManager = null;
        int displayPanelHeight = -1;
        List<UserControl> displayPanels = new List<UserControl>();
        OutputConfigItem config = null;
        OutputConfigItem originalConfig = null;
        ErrorProvider errorProvider = new ErrorProvider();
        DataSet _dataSetConfig = null;

#if ARCAZE
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        Dictionary<string, ArcazeModuleSettings> moduleSettings;
#endif

        Panels.DisplayPinPanel              displayPinPanel             = new Panels.DisplayPinPanel();
        Panels.DisplayBcdPanel              displayBcdPanel             = new Panels.DisplayBcdPanel();
        Panels.DisplayLedDisplayPanel       displayLedDisplayPanel      = new Panels.DisplayLedDisplayPanel();
        Panels.DisplayNothingSelectedPanel  displayNothingSelectedPanel = new Panels.DisplayNothingSelectedPanel();
        Panels.LCDDisplayPanel              displayLcdDisplayPanel      = new Panels.LCDDisplayPanel();
        Panels.ServoPanel                   servoPanel                  = new Panels.ServoPanel();
        Panels.StepperPanel                 stepperPanel                = new Panels.StepperPanel();
        Panels.DisplayShiftRegisterPanel    displayShiftRegisterPanel   = new Panels.DisplayShiftRegisterPanel();

        public ConfigWizard( ExecutionManager mainForm, 
                             OutputConfigItem cfg,
#if ARCAZE
                             ArcazeCache arcazeCache, 
                             Dictionary<string, ArcazeModuleSettings> moduleSettings, 
#endif
                             DataSet dataSetConfig, 
                             String filterGuid)
        {
            Init(mainForm, cfg);
#if ARCAZE
            this.moduleSettings = moduleSettings;
            initWithArcazeCache(arcazeCache);
#else
            initWithoutArcazeCache();
#endif
            preparePreconditionPanel(dataSetConfig, filterGuid);            
        }

        public bool ConfigHasChanged()
        {
            return !originalConfig.Equals(config);
        }

        protected void Init(ExecutionManager mainForm, OutputConfigItem cfg)
        {
            this._execManager = mainForm;
            config = cfg;

            // Until with have the preconditions completely refactored,
            // add an empty precondition in case the current cfg doesn't have one
            // we removed addEmptyNode but add an empty Precondition here
            if (cfg.Preconditions.Count == 0) 
                cfg.Preconditions.Add(new Precondition());

            originalConfig = cfg.Clone() as OutputConfigItem;
            InitializeComponent();
            comparisonSettingsPanel.Enabled = false;
            
            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            if (cfg.FSUIPC.Offset == OutputConfig.FsuipcOffset.OffsetNull)
            {
                lastTabActive = 0;
            }
            tabControlFsuipc.SelectedIndex = lastTabActive;

            _initDisplayPanels();
            _initPreconditionPanel();
            fsuipcConfigPanel.setMode(true);
            fsuipcConfigPanel.syncFromConfig(cfg);

            // displayLedDisplayComboBox.Items.Clear(); 
            simConnectPanel1.OnGetLVarListRequested += SimConnectPanel1_OnGetLVarListRequested;
            _execManager.GetSimConnectCache().LVarListUpdated += ConfigWizard_LVarListUpdated;
        }

        private void SimConnectPanel1_OnGetLVarListRequested(object sender, EventArgs e)
        {
            if(_execManager.GetSimConnectCache().IsConnected())
            {
                _execManager.GetSimConnectCache().RefreshLVarsList();
            }
        }

        private void ConfigWizard_LVarListUpdated(object sender, EventArgs e)
        {
            simConnectPanel1.LVars = (sender as List<String>);
        }

        private void _initPreconditionPanel()
        {
            preConditionTypeComboBox.Items.Clear();
            List<ListItem> preconTypes = new List<ListItem>() {
                new ListItem() { Value = "none",    Label = i18n._tr("LabelPrecondition_None") },
                new ListItem() { Value = "config",  Label = i18n._tr("LabelPrecondition_ConfigItem") },
                new ListItem() { Value = "pin",     Label = i18n._tr("LabelPrecondition_ArcazePin") }
            };
            preConditionTypeComboBox.DataSource = preconTypes;
            preConditionTypeComboBox.DisplayMember = "Label";
            preConditionTypeComboBox.ValueMember = "Value";
            preConditionTypeComboBox.SelectedIndex = 0;

            preconditionConfigComboBox.SelectedIndex = 0;
            preconditionRefOperandComboBox.SelectedIndex = 0;

            // init the pin-type config panel
            List<ListItem> preconPinValues = new List<ListItem>() {
                new ListItem() { Value = "0", Label = "Off" },
                new ListItem() { Value = "1", Label = "On" },                
            };

            preconditionPinValueComboBox.DataSource = preconPinValues;
            preconditionPinValueComboBox.DisplayMember = "Label";
            preconditionPinValueComboBox.ValueMember = "Value";
            preconditionPinValueComboBox.SelectedIndex = 0;

            preconditionSettingsPanel.Enabled = false;
            preconditionApplyButton.Visible = false;
        }

        protected void _initDisplayPanels () {
            // make all panels small and store the common height
            groupBoxDisplaySettings.Controls.Add(displayPinPanel);
            displayPinPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayBcdPanel);
            displayBcdPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayLedDisplayPanel);
            displayLedDisplayPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayNothingSelectedPanel);
            displayNothingSelectedPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(servoPanel);
            servoPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(stepperPanel);
            stepperPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayShiftRegisterPanel);
            displayShiftRegisterPanel.Dock = DockStyle.Top;
            stepperPanel.OnManualCalibrationTriggered += new EventHandler<Panels.ManualCalibrationTriggeredEventArgs>(stepperPanel_OnManualCalibrationTriggered);
            stepperPanel.OnSetZeroTriggered += new EventHandler(stepperPanel_OnSetZeroTriggered);
            stepperPanel.OnStepperSelected +=  StepperPanel_OnStepperSelected;


            groupBoxDisplaySettings.Controls.Add(displayLcdDisplayPanel);
            displayLcdDisplayPanel.AutoSize = false;
            displayLcdDisplayPanel.Height = 0;
            displayLcdDisplayPanel.Dock = DockStyle.Top;

            displayPanels.Clear();
            displayPanelHeight = 0;
            displayPanels.Add(displayPinPanel);
            displayPanels.Add(displayBcdPanel);
            displayPanels.Add(displayLedDisplayPanel);
            displayPanels.Add(displayNothingSelectedPanel);
            displayPanels.Add(servoPanel);
            displayPanels.Add(stepperPanel);
            displayPanels.Add(displayLcdDisplayPanel);
            displayPanels.Add(displayShiftRegisterPanel);

            foreach (UserControl p in displayPanels)
            {
                if (p.Height > 0 && (p.Height > displayPanelHeight)) displayPanelHeight = p.Height;                
                p.Height = 0;
            } //foreach

            displayNothingSelectedPanel.Height = displayPanelHeight;
            displayNothingSelectedPanel.Enabled = true;
        }

        private void StepperPanel_OnStepperSelected(object sender, EventArgs e)
        {
            // in case the module has changed
            // we have to sync those changes to properly
            // find the stepper and be able to
            // show manual calibration group box or not.
            display_syncToConfig();

            String stepperAddress = (sender as ComboBox).SelectedValue.ToString();
            String serial = config.DisplaySerial;
            if (serial.Contains('/'))
            {
                serial = serial.Split('/')[1].Trim();
            }

            MobiFlightStepper stepper = _execManager.getMobiFlightModuleCache()
                                            .GetModuleBySerial(serial)
                                            .GetStepper(stepperAddress);
            stepperPanel.ShowManualCalibation(!stepper.HasAutoZero);
            // sorry for this hack...
        }

        void stepperPanel_OnSetZeroTriggered(object sender, EventArgs e)
        {
            _syncFormToConfig();
            String serial = config.DisplaySerial;
            if (serial.Contains('/'))
            {
                serial = serial.Split('/')[1].Trim();
            }
            _execManager.getMobiFlightModuleCache().resetStepper(serial, config.Stepper.Address);
        }

        void stepperPanel_OnManualCalibrationTriggered(object sender, Panels.ManualCalibrationTriggeredEventArgs e)
        {
            _syncFormToConfig();
            int steps = e.Steps;
            
            String serial = config.DisplaySerial;
            if (serial == null) return;

            if (serial.Contains('/'))
            {
                serial = serial.Split('/')[1].Trim();
            }

            MobiFlightStepper stepper = _execManager.getMobiFlightModuleCache()
                                                    .GetModuleBySerial(serial)
                                                    .GetStepper(config.Stepper.Address);

            int CurrentValue = stepper.Position();
            int NextValue = (CurrentValue + e.Steps);

            _execManager.getMobiFlightModuleCache().setStepper(
                serial,
                config.Stepper.Address,
                (NextValue).ToString(),
                Int16.Parse(config.Stepper.OutputRev),
                Int16.Parse(config.Stepper.OutputRev),
                config.Stepper.CompassMode
            );
            
        }        

        private void preparePreconditionPanel(DataSet dataSetConfig, String filterGuid)
        {
            _dataSetConfig = dataSetConfig;
            DataRow[] rows = dataSetConfig.Tables["config"].Select("guid <> '" + filterGuid +"'");         
   
            // this filters the current config
            DataView dv = new DataView (dataSetConfig.Tables["config"]);
            dv.RowFilter = "guid <> '" + filterGuid + "'";
            preconditionConfigComboBox.DataSource = dv;
            preconditionConfigComboBox.ValueMember = "guid";
            preconditionConfigComboBox.DisplayMember = "description";

            if (preconditionConfigComboBox.Items.Count == 0)
            {
                List<ListItem> preconTypes = new List<ListItem>() {
                new ListItem() { Value = "none",    Label = i18n._tr("LabelPrecondition_None") },
                new ListItem() { Value = "pin",     Label = i18n._tr("LabelPrecondition_ArcazePin") }
                };
                preConditionTypeComboBox.DataSource = preconTypes;
                preConditionTypeComboBox.DisplayMember = "Label";
                preConditionTypeComboBox.ValueMember = "Value";
                preConditionTypeComboBox.SelectedIndex = 0;
            }

            configRefPanel.SetConfigRefsDataView(dv, filterGuid);
            displayLedDisplayPanel.SetConfigRefsDataView(dv, filterGuid);
            //displayShiftRegisterPanel.SetConfigRefsDataView(dv, filterGuid);
        }

#if ARCAZE
        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        /// <param name="arcazeCache"></param>
        public void initWithArcazeCache (ArcazeCache arcazeCache)
        {
            
            // update the display box with
            // modules
            displayModuleNameComboBox.Items.Clear();
            preconditionPinSerialComboBox.Items.Clear();
            displayModuleNameComboBox.Items.Add("-");
            preconditionPinSerialComboBox.Items.Add("-");

            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                arcazeFirmware[module.Serial] = module.Version;
                displayModuleNameComboBox.Items.Add(module.Name + "/ " + module.Serial);
                preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }


            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                displayModuleNameComboBox.Items.Add(module.Name + "/ " + module.Serial);

                // Not yet supported for pins
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }

            displayModuleNameComboBox.SelectedIndex = 0;
            preconditionPinSerialComboBox.SelectedIndex = 0;            
        }
#endif
#if MOBIFLIGHT
        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        public void initWithoutArcazeCache()
        {

            // update the display box with
            // modules
            displayModuleNameComboBox.Items.Clear();
            preconditionPinSerialComboBox.Items.Clear();
            displayModuleNameComboBox.Items.Add("-");
            preconditionPinSerialComboBox.Items.Add("-");

            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                displayModuleNameComboBox.Items.Add(module.Name + "/ " + module.Serial);

                // Not yet supported for pins
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }

            displayModuleNameComboBox.SelectedIndex = 0;
            preconditionPinSerialComboBox.SelectedIndex = 0;
        }
#endif
        /// <summary>
        /// sync the values from config with the config wizard form
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool _syncConfigToForm(OutputConfigItem config)
        {
            string serial = null;
            if (config == null) throw new Exception(i18n._tr("uiException_ConfigItemNotFound"));

            _syncFsuipcTabFromConfig(config);

            _syncComparisonTabFromConfig(config);
            
            // third tab
            if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Display Type ComboBox", LogSeverity.Debug);
            }

            if (config.DisplaySerial != null && config.DisplaySerial != "")
            {
                serial = config.DisplaySerial;
                if (serial.Contains('/'))
                {
                    serial = serial.Split('/')[1].Trim();
                }
                if (!ComboBoxHelper.SetSelectedItemByPart(displayModuleNameComboBox, serial))
                {
                    // TODO: provide error message
                }
            }


            displayPinPanel.syncFromConfig(config);

            displayBcdPanel.syncFromConfig(config);

            displayLedDisplayPanel.syncFromConfig(config);

            servoPanel.syncFromConfig(config);

            // it is not nice but we haev to check what kind of stepper the stepper is
            // to show or not show the manual calibration piece.
            stepperPanel.syncFromConfig(config);

            displayLcdDisplayPanel.syncFromConfig(config);

            displayShiftRegisterPanel.SyncFromConfig(config);

            preconditionListTreeView.Nodes.Clear();

            foreach (Precondition p in config.Preconditions)
            {
                TreeNode tmpNode = new TreeNode();
                tmpNode.Text = p.ToString();
                tmpNode.Tag = p;
                tmpNode.Checked = p.PreconditionActive;
                try
                {
                    _updateNodeWithPrecondition(tmpNode, p);
                    preconditionListTreeView.Nodes.Add(tmpNode);
                }
                catch (IndexOutOfRangeException e)
                {
                    Log.Instance.log("An orphaned precondition has been found", LogSeverity.Error);
                    continue;
                }                
            }

            overridePreconditionCheckBox.Checked = config.Preconditions.ExecuteOnFalse;
            overridePreconditionTextBox.Text = config.Preconditions.FalseCaseValue;

            return true;
        }

        private void _syncComparisonTabFromConfig(OutputConfigItem config)
        {
            // second tab
            comparisonActiveCheckBox.Checked = config.Comparison.Active;
            comparisonValueTextBox.Text = config.Comparison.Value;

            if (!ComboBoxHelper.SetSelectedItem(comparisonOperandComboBox, config.Comparison.Operand))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Comparison ComboBox", LogSeverity.Debug);
            }
            comparisonIfValueTextBox.Text = config.Comparison.IfValue;
            comparisonElseValueTextBox.Text = config.Comparison.ElseValue;

            interpolationCheckBox.Checked = config.Interpolation.Active;
            interpolationPanel1.syncFromConfig(config.Interpolation);
        }

        private void _syncFsuipcTabFromConfig(OutputConfigItem config)
        {
            OffsetTypeFsuipRadioButton.Checked = (config.SourceType == SourceType.FSUIPC);
            OffsetTypeSimConnectRadioButton.Checked = (config.SourceType == SourceType.SIMCONNECT);
            OffsetTypeVariableRadioButton.Checked = (config.SourceType == SourceType.VARIABLE);

            fsuipcConfigPanel.syncFromConfig(config);

            simConnectPanel1.syncFromConfig(config);
            variablePanel1.syncFromConfig(config);
            configRefPanel.syncFromConfig(config);
        }
        
        /// <summary>
        /// sync current status of form values to config
        /// </summary>
        /// <returns></returns>
        protected bool _syncFormToConfig()
        {
            config.SourceType = SourceType.FSUIPC;
            if (OffsetTypeSimConnectRadioButton.Checked) config.SourceType = SourceType.SIMCONNECT;
            if (OffsetTypeVariableRadioButton.Checked) config.SourceType = SourceType.VARIABLE;

            if (config.SourceType == SourceType.FSUIPC)
                fsuipcConfigPanel.syncToConfig(config);
            else if (config.SourceType == SourceType.SIMCONNECT)
                simConnectPanel1.syncToConfig(config);
            else if (config.SourceType == SourceType.VARIABLE)
                variablePanel1.syncToConfig(config);

            configRefPanel.syncToConfig(config);

            // refactor!!!
            comparisonPanel_syncToConfig();

            config.Interpolation.Active = interpolationCheckBox.Checked;
            interpolationPanel1.syncToConfig(config.Interpolation);

            display_syncToConfig();

            // sync the two properties that are not part of the preconditions list

            config.Preconditions.ExecuteOnFalse = overridePreconditionCheckBox.Checked;
            config.Preconditions.FalseCaseValue = overridePreconditionTextBox.Text;
            

            // sync panels
            displayPinPanel.syncToConfig(config);

            displayLedDisplayPanel.syncToConfig(config);

            displayBcdPanel.syncToConfig(config);

            servoPanel.syncToConfig(config);

            stepperPanel.syncToConfig(config);

            displayLcdDisplayPanel.syncToConfig(config);

            displayShiftRegisterPanel.SyncToConfig(config);

            return true;
        }

        private void comparisonPanel_syncToConfig()
        {
            // comparison panel
            config.Comparison.Active = comparisonActiveCheckBox.Checked;
            config.Comparison.Value = comparisonValueTextBox.Text;
            config.Comparison.Operand = comparisonOperandComboBox.Text;
            config.Comparison.IfValue = comparisonIfValueTextBox.Text;
            config.Comparison.ElseValue = comparisonElseValueTextBox.Text;
        }

        private void display_syncToConfig()
        {
            config.DisplayType = displayTypeComboBox.Text;
            config.DisplayTrigger = "normal";
            config.DisplaySerial = displayModuleNameComboBox.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _testModeStop();
            try {
                if (!ValidateChildren())
                {
                    return;
                }
            } catch (System.InvalidOperationException eOp)
            {
                Log.Instance.log("ConfigWizard:button1_Click: " + eOp.Message, LogSeverity.Debug);
            }
            _syncFormToConfig();
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _testModeStop();
            DialogResult = DialogResult.Cancel;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void displaySerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            Log.Instance.log("displaySerialComboBox_SelectedIndexChanged: called.", LogSeverity.Debug);
#endif
            // check which extension type is available to current serial
            ComboBox cb = (sender as ComboBox);

            try
            {
                // disable test button
                // in case that no display is selected                
                String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());

                displayTypeComboBox.Enabled = groupBoxDisplaySettings.Enabled = testSettingsGroupBox.Enabled = (serial != "");
                // serial is empty if no module is selected (on init of form)
                //if (serial == "") return;                

                // update the available types depending on the 
                // type of module
                if (serial.IndexOf("SN") != 0)
                {
                    displayTypeComboBox.Items.Clear();
                    displayTypeComboBox.Items.Add("Pin");
                    displayTypeComboBox.Items.Add(ArcazeLedDigit.TYPE);
                    displayTypeComboBox.Items.Add(MobiFlightShiftRegister.TYPE);
                    //displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                }
                else
                {
                    displayTypeComboBox.Items.Clear();
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);
                    foreach (DeviceType devType in module.GetConnectedOutputDeviceTypes())
                    {
#if DEBUG
                        Log.Instance.log("displaySerialComboBox_SelectedIndexChanged: Adding Device Type: " + devType.ToString(), LogSeverity.Debug);
#endif
                        switch (devType)
                        {
                            case DeviceType.LedModule:
                                displayTypeComboBox.Items.Add(ArcazeLedDigit.TYPE);
                                break;

                            case DeviceType.Output:
                                displayTypeComboBox.Items.Add("Pin");
                                //displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                                break;

                            case DeviceType.Servo:
                                displayTypeComboBox.Items.Add(DeviceType.Servo.ToString("F"));
                                break;

                            case DeviceType.Stepper:
                                displayTypeComboBox.Items.Add(DeviceType.Stepper.ToString("F"));
                                break;

                            case DeviceType.LcdDisplay:
                                displayTypeComboBox.Items.Add(DeviceType.LcdDisplay.ToString("F"));
                                break;

                            case DeviceType.ShiftRegister:
                                displayTypeComboBox.Items.Add(MobiFlightShiftRegister.TYPE);
                                break;
                        }
                    }

                    if (displayTypeComboBox.Items.Count == 0)
                    {
                        if (MessageBox.Show(
                                i18n._tr("uiMessageSelectedModuleDoesNotContainAnyOutputDevices"),
                                i18n._tr("Hint"),
                                MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes
                            )
                        {
                            if (SettingsDialogRequested != null)
                            {
                                SettingsDialogRequested(this, new EventArgs());

                                // trigger reload of Type ComboBox
                                int CurrentIdx = displayModuleNameComboBox.SelectedIndex;
                                displayModuleNameComboBox.SelectedIndex = 0;
                                displayModuleNameComboBox.SelectedIndex = CurrentIdx;
                            }
                        }
                    }
                }

                // third tab
                if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
                {
                    // TODO: provide error message
                    Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Problem setting Display Type ComboBox", LogSeverity.Debug);
                }

            }
            catch (Exception ex)
            {
                displayPinPanel.displayPinBrightnessPanel.Visible = false;
                displayPinPanel.displayPinBrightnessPanel.Enabled = false;
                Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Some Exception occurred" + ex.Message, LogSeverity.Debug);
            }
        }

        private void displayTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log.Instance.log("displayTypeComboBox_SelectedIndexChanged: called.", LogSeverity.Debug);

            foreach (UserControl p in displayPanels)
            {
                p.Enabled = false;
                p.AutoSize = false;
                p.Height = 0;
            } //foreach

            try
            {
                bool panelEnabled = true;
                // get the deviceinfo for the current arcaze
                ComboBox cb = displayModuleNameComboBox;                
                String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());

                // we remove the callback method to ensure, that it is not added more than once
                displayLedDisplayPanel.displayLedAddressComboBox.SelectedIndexChanged -= displayLedAddressComboBox_SelectedIndexChanged;

#if ARCAZE
                if (arcazeFirmware.ContainsKey(serial))
                {

                    switch ((sender as ComboBox).SelectedItem.ToString())
                    {
                        case "DisplayDriver":
                            panelEnabled = ushort.Parse(arcazeFirmware[serial]) > 0x529;
                            break;

                        case "LedDriver2":
                            panelEnabled = ushort.Parse(arcazeFirmware[serial]) > 0x554;
                            break;

                        case "LedDriver3":
                            panelEnabled = ushort.Parse(arcazeFirmware[serial]) > 0x550;
                            break;
                    }

                    displayPinPanel.displayPinBrightnessPanel.Visible = (moduleSettings[serial].type == SimpleSolutions.Usb.ArcazeCommand.ExtModuleType.LedDriver3);
                    displayPinPanel.displayPinBrightnessPanel.Enabled = (displayPinPanel.displayPinBrightnessPanel.Visible && (cb.SelectedIndex > 1));
                    
                    //preconditionPortComboBox.Items.Clear();
                    //preconditionPinComboBox.Items.Clear();

                    List<ListItem> ports = new List<ListItem>();                    

                    foreach (String v in ArcazeModule.getPorts())
                    {
                        ports.Add(new ListItem() { Label = v, Value = v });
                        if (v == "B" || v == "E" || v == "H" || v == "K")
                        {
                            ports.Add(new ListItem() { Label = "-----", Value = "-----" });
                        }

                        if (v == "A" || v == "B")
                        {
                            //preconditionPortComboBox.Items.Add(v);
                        }
                    }

                    displayPinPanel.SetPorts(ports);
                    displayBcdPanel.SetPorts(ports);

                    List<ListItem> pins = new List<ListItem>();
                    foreach (String v in ArcazeModule.getPins())
                    {
                        pins.Add(new ListItem() { Label = v, Value = v });
                        //preconditionPinComboBox.Items.Add(v);
                    }

                    displayPinPanel.SetPins(pins);
                    displayBcdPanel.SetPins(pins);
                    displayPinPanel.WideStyle = false;

                    List<ListItem> addr = new List<ListItem>();
                    List<ListItem> connectors = new List<ListItem>();
                    foreach (string v in ArcazeModule.getDisplayAddresses()) addr.Add(new ListItem() { Label = v, Value = v });
                    foreach (string v in ArcazeModule.getDisplayConnectors()) connectors.Add(new ListItem() { Label = v, Value = v });
                    displayLedDisplayPanel.WideStyle = false;
                    displayLedDisplayPanel.SetAddresses(addr);
                    displayLedDisplayPanel.SetConnectors(connectors);                    
                }
                else 
#endif
                if (serial.IndexOf("SN") == 0)
                {
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

                    displayPinPanel.SetModule(module);
                    displayPinPanel.displayPinBrightnessPanel.Visible = true;
                    displayPinPanel.displayPinBrightnessPanel.Enabled = (displayPinPanel.displayPinBrightnessPanel.Visible && (cb.SelectedIndex > 1));

                    List<ListItem> outputs = new List<ListItem>();
                    List<ListItem> ledSegments = new List<ListItem>();
                    List<ListItem> servos = new List<ListItem>();
                    List<ListItem> stepper = new List<ListItem>();
                    List<ListItem> lcdDisplays = new List<ListItem>();
                    List<ListItem> shiftRegisters = new List<ListItem>();


                    foreach (IConnectedDevice device in module.GetConnectedDevices())
                    {
                        Log.Instance.log("displayTypeComboBox_SelectedIndexChanged: Adding connected device: " + device.Type.ToString() + ", " + device.Name, LogSeverity.Debug);
                        switch (device.Type)
                        {
                            case DeviceType.LedModule:
                                ledSegments.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.Output:
                                outputs.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.Servo:
                                servos.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.Stepper:
                                stepper.Add(new ListItem() { Value = device.Name, Label = device.Name });
                                break;

                            case DeviceType.LcdDisplay:
                                int Cols = (device as MobiFlightLcdDisplay).Cols;
                                int Lines= (device as MobiFlightLcdDisplay).Lines;
                                lcdDisplays.Add(new ListItem() { Value = device.Name+","+ Cols+","+Lines, Label = device.Name });
                                break;
                            
                            case DeviceType.ShiftRegister:
                                shiftRegisters.Add(new ListItem() { Value = device.Name, Label = device.Name });                                
                                
                                break;
                        }                        
                    }
                    displayPinPanel.WideStyle = true;
                    displayPinPanel.SetPorts(new List<ListItem>());
                    displayPinPanel.SetPins(outputs);

                    displayLedDisplayPanel.WideStyle = true;
                    displayLedDisplayPanel.displayLedAddressComboBox.SelectedIndexChanged += new EventHandler(displayLedAddressComboBox_SelectedIndexChanged);
                    displayLedDisplayPanel.SetAddresses(ledSegments);

                    servoPanel.SetAdresses(servos);

                    stepperPanel.SetAdresses(stepper);

                    displayShiftRegisterPanel.shiftRegistersComboBox.SelectedIndexChanged -= shiftRegistersComboBox_selectedIndexChanged;
                    displayShiftRegisterPanel.shiftRegistersComboBox.SelectedIndexChanged += new EventHandler(shiftRegistersComboBox_selectedIndexChanged);
                    displayShiftRegisterPanel.SetAddresses(shiftRegisters);

                    displayLcdDisplayPanel.SetAddresses(lcdDisplays);
                }
                if ((sender as ComboBox).Text == "Pin")
                {
                    displayPinPanel.Enabled = panelEnabled;
                    displayPinPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == ArcazeBcd4056.TYPE)
                {
                    displayBcdPanel.Enabled = panelEnabled;
                    displayBcdPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == ArcazeLedDigit.TYPE)
                {
                    displayLedDisplayPanel.Enabled = panelEnabled;
                    displayLedDisplayPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == DeviceType.Servo.ToString("F"))
                {
                    servoPanel.Enabled = panelEnabled;
                    servoPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == DeviceType.Stepper.ToString("F"))
                {
                    stepperPanel.Enabled = panelEnabled;
                    stepperPanel.Height = displayPanelHeight;
                }
                if ((sender as ComboBox).Text == DeviceType.LcdDisplay.ToString("F"))
                {
                    displayLcdDisplayPanel.Enabled = panelEnabled;
                    displayLcdDisplayPanel.AutoSize = true;
                    displayLcdDisplayPanel.Height = displayPanelHeight;
                }

                if ((sender as ComboBox).Text == DeviceType.ShiftRegister.ToString("F"))
                {
                    displayShiftRegisterPanel.Enabled = panelEnabled;
                    displayShiftRegisterPanel.Height = displayPanelHeight;
                }
            }
            catch (Exception exc)
            {
                Log.Instance.log("ConfigWizard.displayTypeComboBox_SelectedIndexChanged: EXC " + exc.Message, LogSeverity.Debug);
                MessageBox.Show(i18n._tr("uiMessageNotImplementedYet"), 
                                i18n._tr("Hint"), 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
        }

        void displayLedAddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = displayModuleNameComboBox;                
            String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
            MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

            List<ListItem> connectors = new List<ListItem>();

            foreach (IConnectedDevice device in module.GetConnectedDevices())
            {
                if (device.Type != DeviceType.LedModule) continue;
                if (device.Name != ((sender as ComboBox).SelectedItem as ListItem).Value) continue;
                for (int i = 0; i< (device as MobiFlightLedModule).SubModules; i++) {
                    connectors.Add(new ListItem() { Label = (i + 1).ToString(), Value = (i + 1).ToString() });
                }
            }
            displayLedDisplayPanel.SetConnectors(connectors);
        }

        private void shiftRegistersComboBox_selectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = displayModuleNameComboBox;
            String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
            MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);
            bool pwmSupport = false;

            int numModules = 0;
            foreach (IConnectedDevice device in module.GetConnectedDevices())
            {
                if (device.Type != DeviceType.ShiftRegister) continue;
                if (device.Name != ((sender as ComboBox).SelectedItem as ListItem).Value) continue;
                numModules = (device as MobiFlightShiftRegister).NumberOfShifters;
            }
            displayShiftRegisterPanel.SetNumModules(numModules);
        }

        private void fsuipcOffsetTextBox_Validating(object sender, CancelEventArgs e)
        {
            _validatingHexFields(sender, e, 4);            
        }

        private void comparisonActiveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            comparisonSettingsPanel.Enabled = (sender as CheckBox).Checked;
        }

        private void ConfigWizard_Load(object sender, EventArgs e)
        {
            _syncConfigToForm(config);
        }
        
        private void fsuipcMultiplyTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                float.Parse((sender as TextBox).Text);
                removeError(sender as Control);
            }
            catch (Exception exc)
            {
                Log.Instance.log("fsuipcMultiplyTextBox_Validating : Parsing problem, " + exc.Message, LogSeverity.Debug);
                displayError(sender as Control, i18n._tr("uiMessageFsuipcConfigPanelMultiplyWrongFormat"));
                e.Cancel = true;
            }
        }
        
        private void _validatingHexFields(object sender, CancelEventArgs e, int length)
        {
            try
            {
                string tmp = (sender as TextBox).Text.Replace("0x", "").ToUpper();
                (sender as TextBox).Text = "0x" + Int64.Parse(tmp, System.Globalization.NumberStyles.HexNumber).ToString("X" + length.ToString());
            }
            catch (Exception exc)
            {                
                e.Cancel = true;
                Log.Instance.log("_validatingHexFields : Parsing problem, " + exc.Message, LogSeverity.Debug);
                MessageBox.Show(i18n._tr("uiMessageConfigWizard_ValidHexFormat"), i18n._tr("Hint"));
            }
        }

        private void displayError(Control control, String message)
        {
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, i18n._tr("Hint"));
        }

        private void removeError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }

        private void displayArcazeSerialComboBox_Validating(object sender, CancelEventArgs e)
        {
            /* disabled this validation to permit configs even without module or
             * as precondition only
             
            if (displayArcazeSerialComboBox.Text.Trim() == "-")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = displayTabPage;
                displayArcazeSerialComboBox.Focus();
                displayError(displayArcazeSerialComboBox, i18n._tr("uiMessageConfigWizard_SelectArcaze"));                
            }
            else
            {
               removeError(displayArcazeSerialComboBox);             
            }
             */
        }

        private void portComboBox_Validating(object sender, CancelEventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            if (!cb.Parent.Visible) return;
            if (null == cb.SelectedItem) return;
            if (cb.SelectedItem.ToString() == "-----")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = displayTabPage;
                cb.Focus();
                displayError(cb, i18n._tr("Please_select_a_port"));
            }
            else
            {
                removeError(cb);
            }
        }

        private void displayLedDisplayComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (displayTypeComboBox.Text == ArcazeLedDigit.TYPE)                
            {                
                try
                {
                    int.Parse(displayLedDisplayPanel.displayLedAddressComboBox.Text);
                    removeError(displayLedDisplayPanel.displayLedAddressComboBox);
                }
                catch (Exception exc)
                {
                    Log.Instance.log("displayLedDisplayComboBox_Validating : Parsing problem, " + exc.Message, LogSeverity.Debug);
                    e.Cancel = true;
                    tabControlFsuipc.SelectedTab = displayTabPage;
                    displayLedDisplayPanel.displayLedAddressComboBox.Focus();
                    displayError(displayLedDisplayPanel.displayLedAddressComboBox, i18n._tr("uiMessageConfigWizard_ProvideLedDisplayAddress"));
                }                
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value = Int16.Parse((sender as ComboBox).Text);
            for (int i = 0; i < 8; i++)
            {
                displayLedDisplayPanel.displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"].Visible = i < value;
                displayLedDisplayPanel.displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"].Visible = i < value;
                displayLedDisplayPanel.Controls["displayLedDisplayLabel" + i].Visible = i < value;

                // uncheck all invisible checkboxes to ensure correct mask
                if (i >= value)
                {
                    (displayLedDisplayPanel.displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"] as CheckBox).Checked = false;
                    (displayLedDisplayPanel.displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"] as CheckBox).Checked = false;
                }
            }
        }

        private void preConditionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = ((sender as ComboBox).SelectedItem as ListItem).Value;
            preconditionSettingsGroupBox.Visible = selected != "none";
            preconditionRuleConfigPanel.Visible = false;
            preconditionRuleConfigPanel.Visible = selected == "config";
            preconditionPinPanel.Visible = selected == "pin";
        }

        private void preconditionRuleConfigPanel_Validating(object sender, CancelEventArgs e)
        {            
        }
        
        private void preconditionRefValueTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionRuleConfigPanel).Visible)
            {
                removeError(preconditionRefValueTextBox);
                return;
            }

            if (preconditionRefValueTextBox.Text.Trim() == "")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionRefValueTextBox, i18n._tr("uiMessageConfigWizard_SelectComparison"));
            }
            else
            {
                removeError(preconditionRefValueTextBox);
            }
        }

        private void preconditionPinSerialComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionRefValueTextBox);
                return;
            }

            if (preconditionPinSerialComboBox.Items.Count > 1 && preconditionPinSerialComboBox.Text.Trim() == "-")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                preconditionPinSerialComboBox.Focus();
                displayError(preconditionPinSerialComboBox, i18n._tr("uiMessageConfigWizard_SelectArcaze"));
            }
            else
            {
                removeError(preconditionPinSerialComboBox);
            }

        }

        private void preconditionPinComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionPinComboBox);
                return;
            }

            if (preconditionPinSerialComboBox.SelectedIndex > 0 && preconditionPinComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionPinComboBox, i18n._tr("Please_select_a_pin"));
            }
            else
            {
                removeError(preconditionPinComboBox);
            }
        }

        private void preconditionPortComboBox_Validating(object sender, CancelEventArgs e) {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionPortComboBox);
                return;
            }

            if (preconditionPinSerialComboBox.SelectedIndex > 0 && preconditionPortComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionPortComboBox, i18n._tr("Please_select_a_port"));
            }
            else
            {
                removeError(preconditionPortComboBox);
            }
        }

        private void displayPinTestButton_Click(object sender, EventArgs e)
        {
            _testModeStart();
        }

        private void displayPinTestStopButton_Click(object sender, EventArgs e)
        {
            _testModeStop();
        }

        private void _testModeStart()
        {
            _syncFormToConfig();
            displayPinTestStopButton.Enabled = true;
            displayPinTestButton.Enabled = false;
            displayTypeGroupBox.Enabled = false;
            groupBoxDisplaySettings.Enabled = false;
            _execManager.ExecuteTestOn(config);
        }

        private void _testModeStop()
        {
            // check if running in test mode otherwise simply return
            if (!displayPinTestStopButton.Enabled) return;

            displayPinTestStopButton.Enabled = false;
            displayPinTestButton.Enabled = true;
            displayTypeGroupBox.Enabled = true;
            groupBoxDisplaySettings.Enabled = true;
            _execManager.ExecuteTestOff(config);
        }

        private void tabControlFsuipc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if running in test mode
            lastTabActive = (sender as TabControl).SelectedIndex;
            _testModeStop();
        }
        
        private void fsuipcOffsetTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //updateByteSizeComboBox();
        }

        /*
        private void updateByteSizeComboBox()
        {
            string selectedText = fsuipcSizeComboBox.Text;
            fsuipcSizeComboBox.Items.Clear();
            fsuipcSizeComboBox.Enabled = true;

            if (fsuipcOffsetTypeComboBox.SelectedValue.ToString() == FSUIPCOffsetType.Integer.ToString())
            {
                fsuipcSizeComboBox.Items.Add("1");
                fsuipcSizeComboBox.Items.Add("2");
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText);
            }
            else if (fsuipcOffsetTypeComboBox.SelectedValue.ToString() == FSUIPCOffsetType.Float.ToString())
            {                
                fsuipcSizeComboBox.Items.Add("4");
                fsuipcSizeComboBox.Items.Add("8");
                ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, selectedText);
            }
            else if (fsuipcOffsetTypeComboBox.SelectedValue.ToString() == FSUIPCOffsetType.String.ToString())
            {
                fsuipcSizeComboBox.Enabled = false;
            }
        }
        */
        
        private void preconditionListTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            preconditionListTreeView.SelectedNode = e.Node;
            //if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            Precondition config = (e.Node.Tag as Precondition);
            preConditionTypeComboBox.SelectedValue = config.PreconditionType;
            preconditionSettingsPanel.Enabled = true;
            preconditionApplyButton.Visible = true;
            config.PreconditionActive = e.Node.Checked;

            switch (config.PreconditionType)
            {
                case "config":
                    try
                    {                        
                        preconditionConfigComboBox.SelectedValue = config.PreconditionRef;
                    }
                    catch (Exception exc)
                    {
                        // precondition could not be loaded
                        Log.Instance.log("preconditionListTreeView_NodeMouseClick : Precondition could not be loaded, " + exc.Message, LogSeverity.Debug);
                    }

                    ComboBoxHelper.SetSelectedItem(preconditionRefOperandComboBox, config.PreconditionOperand);
                    preconditionRefValueTextBox.Text = config.PreconditionValue;
                    break;

                case "pin":
                    ArcazeIoBasic io = new ArcazeIoBasic(config.PreconditionPin);                    
                    ComboBoxHelper.SetSelectedItemByPart(preconditionPinSerialComboBox, config.PreconditionSerial);
                    preconditionPinValueComboBox.SelectedValue = config.PreconditionValue;
                    preconditionPortComboBox.SelectedIndex = io.Port;
                    preconditionPinComboBox.SelectedIndex = io.Pin;
                    break;
            }  

            aNDToolStripMenuItem.Checked = config.PreconditionLogic == "and";
            oRToolStripMenuItem.Checked = config.PreconditionLogic == "or";            
        }

        private void preconditionApplyButton_Click(object sender, EventArgs e)
        {
            // sync the selected node with the current settings from the panels
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            if (selectedNode == null) return;

            Precondition c = selectedNode.Tag as Precondition;
            
            c.PreconditionType = (preConditionTypeComboBox.SelectedItem as ListItem).Value;
            switch (c.PreconditionType)
            {
                case "config":
                    c.PreconditionRef = preconditionConfigComboBox.SelectedValue.ToString();
                    c.PreconditionOperand = preconditionRefOperandComboBox.Text;
                    c.PreconditionValue = preconditionRefValueTextBox.Text;
                    c.PreconditionActive = true;
                    break;
                
                case "pin":                    
                    c.PreconditionSerial = preconditionPinSerialComboBox.Text;
                    c.PreconditionValue = preconditionPinValueComboBox.SelectedValue.ToString();
                    c.PreconditionPin = preconditionPortComboBox.Text + preconditionPinComboBox.Text;
                    c.PreconditionActive = true;
                    break;                    
            }

            _updateNodeWithPrecondition(selectedNode, c);
        }    
    
        private void _updateNodeWithPrecondition (TreeNode node, Precondition p) 
        {
            String label = p.PreconditionLabel;
            if (p.PreconditionType == "config")
            {
                String replaceString = "[unknown]";
                if (_dataSetConfig != null)
                {
                    DataRow[] rows = _dataSetConfig.Tables["config"].Select("guid = '" + p.PreconditionRef + "'");
                    if (rows.Count() == 0) throw new IndexOutOfRangeException(); // an orphaned entry has been found
                    replaceString = rows[0]["description"] as String;
                }
                label = label.Replace("<Ref:" + p.PreconditionRef  + ">", replaceString);
            }
            else if (p.PreconditionType == "pin")
            {
                label = label.Replace("<Serial:" + p.PreconditionSerial + ">", p.PreconditionSerial.Split('/')[0]);
            }
            
            label = label.Replace("<Logic:and>", " (AND)").Replace("<Logic:or>", " (OR)");
            node.Checked = p.PreconditionActive;
            node.Tag = p;
            node.Text = label;
            aNDToolStripMenuItem.Checked = p.PreconditionLogic == "and";
            oRToolStripMenuItem.Checked = p.PreconditionLogic == "or";
        }

        private void addPreconditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Precondition p = new Precondition();
            TreeNode n = new TreeNode();
            n.Tag = p;
            config.Preconditions.Add(p);
            preconditionListTreeView.Nodes.Add(n);
            _updateNodeWithPrecondition(n, p);
        }

        private void andOrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            Precondition p = selectedNode.Tag as Precondition;
            if ((sender as ToolStripMenuItem).Text == "AND")
                p.PreconditionLogic = "and";
            else
                p.PreconditionLogic = "or";

            _updateNodeWithPrecondition(selectedNode, p);            
        }

        private void removePreconditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            Precondition p = selectedNode.Tag as Precondition;
            config.Preconditions.Remove(p);
            preconditionListTreeView.Nodes.Remove(selectedNode);
        }

        private void preconditionPinSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the deviceinfo for the current arcaze
            ComboBox cb = preconditionPinSerialComboBox;
            String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
            // if (serial == "" && config.DisplaySerial != null) serial = ArcazeModuleSettings.ExtractSerial(config.DisplaySerial);

            if (serial.IndexOf("SN") != 0)
            {
                preconditionPortComboBox.Items.Clear();
                preconditionPinComboBox.Items.Clear();

                List<ListItem> ports = new List<ListItem>();

                foreach (String v in ArcazeModule.getPorts())
                {
                    ports.Add(new ListItem() { Label = v, Value = v });
                    if (v == "B" || v == "E" || v == "H" || v == "K")
                    {
                        ports.Add(new ListItem() { Label = "-----", Value = "-----" });
                    }

                    if (v == "A" || v == "B")
                    {
                        preconditionPortComboBox.Items.Add(v);
                    }
                }

                List<ListItem> pins = new List<ListItem>();
                foreach (String v in ArcazeModule.getPins())
                {
                    pins.Add(new ListItem() { Label = v, Value = v });
                    preconditionPinComboBox.Items.Add(v);
                }
            }
        }

        private void interpolationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            interpolationPanel1.Enabled = (sender as CheckBox).Checked;
            if ((sender as CheckBox).Checked)
                interpolationPanel1.Save = true;
        }

        private void OffsetTypeFsuipRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            FsuipcSettingsPanel.Visible = (sender as RadioButton) == OffsetTypeFsuipRadioButton;
            simConnectPanel1.Visible = (sender as RadioButton) == OffsetTypeSimConnectRadioButton;
            variablePanel1.Visible = (sender as RadioButton) == OffsetTypeVariableRadioButton;
        }

        private void ConfigWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _execManager.GetSimConnectCache().LVarListUpdated -= ConfigWizard_LVarListUpdated;
        }
    }
}
