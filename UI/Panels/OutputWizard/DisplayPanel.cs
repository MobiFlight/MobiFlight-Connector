using MobiFlight.Base;
using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.OutputWizard
{
    public partial class DisplayPanel : UserControl
    {
        public event EventHandler<EventArgs> TestModeStopRequested;
        public event EventHandler<EventArgs> TestModeStartRequested;
        public event EventHandler<EventArgs> DisplayPanelValidatingError;

#if ARCAZE
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        Dictionary<string, ArcazeModuleSettings> moduleSettings;
#endif

        public event EventHandler SettingsDialogRequested;

        ErrorProvider errorProvider = new ErrorProvider();

        OutputConfigItem config = null;
        OutputConfigItem originalConfig = null;

        List<UserControl> displayPanels = new List<UserControl>();
        int displayPanelHeight = -1;

        ExecutionManager _execManager = null;

        Panels.DisplayPinPanel displayPinPanel = new Panels.DisplayPinPanel();
        Panels.DisplayBcdPanel displayBcdPanel = new Panels.DisplayBcdPanel();
        Panels.DisplayLedDisplayPanel displayLedDisplayPanel = new Panels.DisplayLedDisplayPanel();
        Panels.DisplayNothingSelectedPanel displayNothingSelectedPanel = new Panels.DisplayNothingSelectedPanel();
        Panels.LCDDisplayPanel displayLcdDisplayPanel = new Panels.LCDDisplayPanel();
        Panels.ServoPanel servoPanel = new Panels.ServoPanel();
        Panels.StepperPanel stepperPanel = new Panels.StepperPanel();
        Panels.DisplayShiftRegisterPanel displayShiftRegisterPanel = new Panels.DisplayShiftRegisterPanel();


        public DisplayPanel()
        {
            InitializeComponent();
            analogPanel1.OnPanelChanged += (s, e) => {
                inputActionGroupBox.AutoSize = false;
                inputActionGroupBox.Height = (s as UserControl).Height + 80 + AnalogInputActionLabel.Height;
            };

            buttonPanel1.OnPanelChanged += (s, e) => {
                inputActionGroupBox.AutoSize = false;
                inputActionGroupBox.Height = (s as UserControl).Height + 80 + ButtonInputActionLabel.Height;
            };
        }

        public void SetConfigRefsDataView(DataView dv, string filterGuid)
        {
            displayLedDisplayPanel.SetConfigRefsDataView(dv, filterGuid);
        }

        public bool ConfigHasChanged()
        {
            return !originalConfig.Equals(config);
        }

        public void Init(ExecutionManager executionManager)
        {
            _execManager = executionManager;
            _initDisplayPanels();            
        }

        public void SetModules(List<ListItem> ModuleList)
        {
            displayModuleNameComboBox.Items.Clear();
            displayModuleNameComboBox.Items.Add(new ListItem() { Value = "-", Label = "" });
            displayModuleNameComboBox.Items.AddRange(ModuleList.ToArray());
            displayModuleNameComboBox.SelectedIndex = 0;
        }

        protected void _initDisplayPanels()
        {
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
            stepperPanel.OnStepperSelected += StepperPanel_OnStepperSelected;


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

        internal void syncFromConfig(OutputConfigItem cfg)
        {
            originalConfig = cfg.Clone() as OutputConfigItem;
            config = cfg;

            OutputTypeComboBox.SelectedIndex = (config.DisplayType == "InputAction") ? 1 : 0;

            if (OutputTypeComboBox.SelectedIndex == 0)
            {
                if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
                {
                    // TODO: provide error message
                    Log.Instance.log("_syncConfigToForm : Exception on selecting item in Display Type ComboBox", LogSeverity.Debug);
                }

                if (config.DisplaySerial != null && config.DisplaySerial != "")
                {
                    if (!ComboBoxHelper.SetSelectedItemByValue(displayModuleNameComboBox, config.DisplaySerial))
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
            }
            else
            {
                AnalogInputConfig analogInputConfig = new AnalogInputConfig();
                analogInputConfig.onChange = config.AnalogInputConfig?.onChange;                    ;
                analogPanel1.syncFromConfig(analogInputConfig);

                ButtonInputConfig buttonInputConfig = new ButtonInputConfig();
                buttonInputConfig.onPress = config.ButtonInputConfig?.onPress;
                buttonInputConfig.onRelease = config.ButtonInputConfig?.onRelease;
                buttonPanel1.syncFromConfig(buttonInputConfig);

                InputTypeButtonRadioButton.Checked = config.AnalogInputConfig?.onChange == null;
                InputTypeAnalogRadioButton.Checked = config.AnalogInputConfig?.onChange != null;
            }
        }

        private void StepperPanel_OnStepperSelected(object sender, EventArgs e)
        {
            // in case the module has changed
            // we have to sync those changes to properly
            // find the stepper and be able to
            // show manual calibration group box or not.
            syncToConfig();

            String stepperAddress = (sender as ComboBox).SelectedValue.ToString();
            String serial = SerialNumber.ExtractSerial(config.DisplaySerial);

            MobiFlightStepper stepper = _execManager.getMobiFlightModuleCache()
                                            .GetModuleBySerial(serial)
                                            .GetStepper(stepperAddress);
            stepperPanel.ShowManualCalibation(!stepper.HasAutoZero);
            // sorry for this hack...
        }

        void stepperPanel_OnSetZeroTriggered(object sender, EventArgs e)
        {
            syncToConfig();
            String serial = SerialNumber.ExtractSerial(config.DisplaySerial);
            _execManager.getMobiFlightModuleCache().resetStepper(serial, config.Stepper.Address);
        }

        internal void SetArcazeSettings(Dictionary<string, string> arcazeFirmware, Dictionary<string, ArcazeModuleSettings> moduleSettings)
        {
            this.arcazeFirmware = arcazeFirmware;
            this.moduleSettings = moduleSettings;
        }

        void stepperPanel_OnManualCalibrationTriggered(object sender, Panels.ManualCalibrationTriggeredEventArgs e)
        {
            syncToConfig();
            int steps = e.Steps;

            String serial = SerialNumber.ExtractSerial(config.DisplaySerial);

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

        internal void syncToConfig()
        {
            if (OutputTypeComboBox.SelectedIndex == 0) { 
                config.DisplayType = displayTypeComboBox.Text;
                config.DisplayTrigger = "normal";
                config.DisplaySerial = displayModuleNameComboBox.SelectedItem.ToString();

                // sync panels
                displayPinPanel.syncToConfig(config);

                displayLedDisplayPanel.syncToConfig(config);

                displayBcdPanel.syncToConfig(config);

                servoPanel.syncToConfig(config);

                stepperPanel.syncToConfig(config);

                displayLcdDisplayPanel.syncToConfig(config);

                displayShiftRegisterPanel.SyncToConfig(config);
            }
            else { 
                config.DisplayType = "InputAction";

                if (analogPanel1.Enabled)
                {
                    AnalogInputConfig tmpConfig = new AnalogInputConfig();
                    analogPanel1.ToConfig(tmpConfig);
                    config.AnalogInputConfig = tmpConfig;
                } else
                {
                    ButtonInputConfig tmpConfig = new ButtonInputConfig();
                    buttonPanel1.ToConfig(tmpConfig);
                    config.ButtonInputConfig = tmpConfig;
                }
            }
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
                                SettingsDialogRequested(module, new EventArgs());

                                // trigger reload of Type ComboBox
                                int CurrentIdx = displayModuleNameComboBox.SelectedIndex;
                                displayModuleNameComboBox.SelectedIndex = 0;
                                displayModuleNameComboBox.SelectedIndex = CurrentIdx;
                            }
                        }
                    }
                }

                // config can be null
                // because module information is set
                // before config is loaded
                if (config == null) return;


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
                                int Lines = (device as MobiFlightLcdDisplay).Lines;
                                lcdDisplays.Add(new ListItem() { Value = device.Name + "," + Cols + "," + Lines, Label = device.Name });
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
                for (int i = 0; i < (device as MobiFlightLedModule).SubModules; i++)
                {
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

        private void portComboBox_Validating(object sender, CancelEventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            if (!cb.Parent.Visible) return;
            if (null == cb.SelectedItem) return;
            if (cb.SelectedItem.ToString() == "-----")
            {
                e.Cancel = true;
                cb.Focus();
                displayError(cb, i18n._tr("Please_select_a_port"));
                DisplayPanelValidatingError?.Invoke(this, EventArgs.Empty);
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
                    
                    displayLedDisplayPanel.displayLedAddressComboBox.Focus();
                    displayError(displayLedDisplayPanel.displayLedAddressComboBox, i18n._tr("uiMessageConfigWizard_ProvideLedDisplayAddress"));
                    DisplayPanelValidatingError?.Invoke(this, EventArgs.Empty);
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

        private void displayPinTestButton_Click(object sender, EventArgs e)
        {
            TestModeStartRequested?.Invoke(this, EventArgs.Empty);
            displayPinTestStopButton.Enabled = true;
            displayPinTestButton.Enabled = false;
            displayTypeGroupBox.Enabled = false;
            groupBoxDisplaySettings.Enabled = false;
        }

        private void displayPinTestStopButton_Click(object sender, EventArgs e)
        {
            // check if running in test mode otherwise simply return
            if (!displayPinTestStopButton.Enabled) return;

            displayPinTestStopButton.Enabled = false;
            displayPinTestButton.Enabled = true;
            displayTypeGroupBox.Enabled = true;
            groupBoxDisplaySettings.Enabled = true;

            TestModeStopRequested?.Invoke(this, EventArgs.Empty);
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

        private void OutputTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayTypePanel.Visible = OutputTypeComboBox.SelectedIndex == 0;
            groupBoxDisplaySettings.Visible = OutputTypeComboBox.SelectedIndex == 0;
            testSettingsGroupBox.Visible = OutputTypeComboBox.SelectedIndex == 0;

            InputActionTypePanel.Visible = OutputTypeComboBox.SelectedIndex == 1;
            inputActionGroupBox.Visible = OutputTypeComboBox.SelectedIndex == 1;
        }

        private void InputTypeButtonRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            buttonPanel1.Enabled = InputTypeButtonRadioButton.Checked;
            buttonPanel1.Visible = InputTypeButtonRadioButton.Checked;
            ButtonInputActionLabel.Visible = InputTypeButtonRadioButton.Checked;

            analogPanel1.Enabled = InputTypeAnalogRadioButton.Checked;
            analogPanel1.Visible = InputTypeAnalogRadioButton.Checked;
            AnalogInputActionLabel.Visible = InputTypeAnalogRadioButton.Checked;
        }
    }
}
