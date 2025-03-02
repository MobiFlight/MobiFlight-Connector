using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Settings.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.OutputWizard
{
    public partial class DisplayPanel : UserControl
    {
        public event EventHandler<EventArgs> DisplayPanelValidatingError;

#if ARCAZE
        Dictionary<String, string> arcazeFirmware = new Dictionary<String, String>();
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
        Panels.DisplayLedDisplayPanel displayLedDisplayPanel = new Panels.DisplayLedDisplayPanel();
        Panels.DisplayNothingSelectedPanel displayNothingSelectedPanel = new Panels.DisplayNothingSelectedPanel();
        Panels.LCDDisplayPanel displayLcdDisplayPanel = new Panels.LCDDisplayPanel();
        Panels.ServoPanel servoPanel = new Panels.ServoPanel();
        Panels.StepperPanel stepperPanel = new Panels.StepperPanel();
        Panels.DisplayShiftRegisterPanel displayShiftRegisterPanel = new Panels.DisplayShiftRegisterPanel();
        Panels.CustomDevicePanel customDevicePanel = new Panels.CustomDevicePanel();


        public DisplayPanel()
        {
            InitializeComponent();
        }

        public void SetConfigRefsDataView(List<OutputConfigItem> outputConfigs, string filterGuid)
        {
            displayLedDisplayPanel.SetConfigRefsDataView(outputConfigs, filterGuid);
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

            // Pre select the first module if there is only one in the list.
            if (displayModuleNameComboBox.Items.Count == 2)
                displayModuleNameComboBox.SelectedIndex = 1;
            else
                displayModuleNameComboBox.SelectedIndex = 0;
        }

        protected void _initDisplayPanels()
        {
            displayPanels = new List<UserControl>() {
                displayPinPanel,
                displayLedDisplayPanel,
                displayNothingSelectedPanel,
                servoPanel,
                stepperPanel,
                displayShiftRegisterPanel,
                displayLcdDisplayPanel,
                customDevicePanel
            };

            displayPanelHeight = 0;
            displayLcdDisplayPanel.AutoSize = false;

            displayPanels.ForEach(control => {
                groupBoxDisplaySettings.Controls.Add(control);
                control.Dock = DockStyle.Top;

                if (control.Height > 0 && (control.Height > displayPanelHeight)) displayPanelHeight = control.Height;
                control.Height = 0;
            });

            displayNothingSelectedPanel.Height = displayPanelHeight;
            displayNothingSelectedPanel.Enabled = true;

            stepperPanel.OnManualCalibrationTriggered += stepperPanel_OnManualCalibrationTriggered;
            stepperPanel.OnSetZeroTriggered += stepperPanel_OnSetZeroTriggered;
            stepperPanel.OnStepperSelected += StepperPanel_OnStepperSelected;
            // set the default profile for steppers
            stepperPanel.setStepperProfile(MFStepperPanel.Profiles.Find(p => p.Value.id == 0).Value);
        }

        internal void syncFromConfig(OutputConfigItem cfg)
        {
            originalConfig = cfg.Clone() as OutputConfigItem;
            config = cfg;

            OutputTypeComboBox.SelectedIndex = (config.DeviceType == "InputAction") ? 1 : 0;

            if (OutputTypeIsDisplay())
            {
                if (config.ModuleSerial != null && config.ModuleSerial != "")
                {
                    if (!ComboBoxHelper.SetSelectedItemByValue(displayModuleNameComboBox, config.ModuleSerial))
                    {
                        Log.Instance.log($"Trying to show config but {config.ModuleSerial} currently not connected.", LogSeverity.Error);
                    }
                }

                if (config.DeviceType != null && !ComboBoxHelper.SetSelectedItemByValue(displayTypeComboBox, config.DeviceType))
                {
                    Log.Instance.log($"Trying to show config but display type {config.DeviceType} not present.", LogSeverity.Error);
                }

                switch (config.DeviceType)
                {
                    case MobiFlightOutput.TYPE:
                        displayPinPanel.syncFromConfig(config);
                        break;

                    case MobiFlightStepper.TYPE:
                        stepperPanel.syncFromConfig(config);
                        break;

                    case MobiFlightLedModule.TYPE:
                        displayLedDisplayPanel.syncFromConfig(config);
                        break;

                    case MobiFlightServo.TYPE:
                        servoPanel.syncFromConfig(config);
                        break;

                    case MobiFlightLcdDisplay.TYPE:
                        displayLcdDisplayPanel.syncFromConfig(config);
                        break;

                    case MobiFlightShiftRegister.TYPE:
                        displayShiftRegisterPanel.SyncFromConfig(config);
                        break;

                    case MobiFlightCustomDevice.TYPE:
                        customDevicePanel.syncFromConfig(config);
                        break;
                }
            }
            else
            {
                analogPanel1.syncFromConfig(config.AnalogInputConfig);
                buttonPanel1.syncFromConfig(config.ButtonInputConfig);

                InputTypeButtonRadioButton.Checked = config.AnalogInputConfig?.onChange == null;
                InputTypeAnalogRadioButton.Checked = config.AnalogInputConfig?.onChange != null;
            }
        }

        private bool OutputTypeIsDisplay()
        {
            return OutputTypeComboBox.SelectedIndex == 0;
        }

        private bool OutputTypeIsInputAction()
        {
            return OutputTypeComboBox.SelectedIndex == 1;
        }

        internal void SetArcazeSettings(Dictionary<string, string> arcazeFirmware, Dictionary<string, ArcazeModuleSettings> moduleSettings)
        {
            this.arcazeFirmware = arcazeFirmware;
            this.moduleSettings = moduleSettings;
        }
        internal void syncToConfig()
        {
            if (OutputTypeIsDisplay())
            {
                if (displayTypeComboBox.SelectedItem == null) return;

                config.DeviceType = (displayTypeComboBox.SelectedItem as ListItem).Value;
                config.ModuleSerial = displayModuleNameComboBox.SelectedItem.ToString();

                if ((displayTypeComboBox.SelectedItem as ListItem).Value == "-") return;

                switch (config.DeviceType)
                {
                    case MobiFlightOutput.TYPE:
                        displayPinPanel.syncToConfig(config);
                        break;

                    case MobiFlightStepper.TYPE:
                        // it is not nice but we haev to check what kind of stepper the stepper is
                        // to show or not show the manual calibration piece.
                        stepperPanel.syncToConfig(config);
                        break;

                    case MobiFlightLedModule.TYPE:
                        displayLedDisplayPanel.syncToConfig(config);
                        break;

                    case MobiFlightServo.TYPE:
                        servoPanel.syncToConfig(config);
                        break;

                    case MobiFlightLcdDisplay.TYPE:
                        displayLcdDisplayPanel.syncToConfig(config);
                        break;

                    case MobiFlightShiftRegister.TYPE:
                        displayShiftRegisterPanel.syncToConfig(config);
                        break;

                    case MobiFlightCustomDevice.TYPE:
                        customDevicePanel.syncToConfig(config);
                        break;
                }
            }
            else if (OutputTypeIsInputAction())
            {
                config.DeviceType = "InputAction";

                if (analogPanel1.Enabled)
                {
                    AnalogInputConfig tmpConfig = new AnalogInputConfig();
                    analogPanel1.ToConfig(tmpConfig);
                    config.AnalogInputConfig = tmpConfig;
                    config.ButtonInputConfig = null;
                }
                else
                {
                    ButtonInputConfig tmpConfig = new ButtonInputConfig();
                    buttonPanel1.ToConfig(tmpConfig);
                    config.ButtonInputConfig = tmpConfig;
                    config.AnalogInputConfig = null;
                }
            }
        }

        private void displaySerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check which extension type is available to current serial
            ComboBox cb = (sender as ComboBox);
            List<ListItem> deviceTypeOptions = new List<ListItem>();

            displayTypeComboBox.DataSource = null;
            displayTypeComboBox.Items.Clear();

            // disable test button
            // in case that no display is selected
            String rawSerial = cb.SelectedItem.ToString();
            String serial = SerialNumber.ExtractSerial(rawSerial);

            try
            {

                // serial is empty if no module is selected (e.g. on init of form)
                // but we add all available devices to be able to display the 
                // config even if the module is not currently connected
                // e.g., when a user shares a config with somebody else.
                if (serial == "")
                {
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightOutput.TYPE, Label = "LED / Output" });
                    deviceTypeOptions.Add(new ListItem() { Value = ArcazeLedDigit.TYPE, Label = ArcazeLedDigit.TYPE });
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightServo.TYPE, Label = MobiFlightServo.TYPE });
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightStepper.TYPE, Label = MobiFlightStepper.TYPE });
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightLcdDisplay.TYPE, Label = MobiFlightLcdDisplay.TYPE });
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightShiftRegister.TYPE, Label = MobiFlightShiftRegister.TYPE });
                }
                else if (serial.IndexOf(Joystick.SerialPrefix) == 0)
                {
                    Joystick joystick = _execManager.GetJoystickManager().GetJoystickBySerial(serial);
                    if (joystick != null)
                    {
                        foreach (var deviceType in joystick.GetConnectedOutputDeviceTypes())
                        {
                            switch (deviceType)
                            {
                                case DeviceType.Output:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightOutput.TYPE, Label = "LED / Output" });
                                    break;
                                case DeviceType.LcdDisplay:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightLcdDisplay.TYPE, Label = MobiFlightLcdDisplay.TYPE });
                                    break;
                            }
                        }
                    }
                }
                else if (serial.IndexOf(MidiBoard.SerialPrefix) == 0)
                {
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightOutput.TYPE, Label = "LED / Output" });
                }
                // update the available types depending on the 
                // type of module
                else if (serial.IndexOf("SN") != 0)
                {
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightOutput.TYPE, Label = "LED / Output" });
                    deviceTypeOptions.Add(new ListItem() { Value = ArcazeLedDigit.TYPE, Label = ArcazeLedDigit.TYPE });
                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightShiftRegister.TYPE, Label = MobiFlightShiftRegister.TYPE });
                }
                // update the available types depending on the 
                // type of module
                else
                {
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache()
                                                          .GetModuleBySerial(serial);

                    if (module != null)
                    {
                        foreach (DeviceType devType in module.GetConnectedOutputDeviceTypes())
                        {
                            switch (devType)
                            {
                                case DeviceType.LedModule:
                                    deviceTypeOptions.Add(new ListItem() { Value = ArcazeLedDigit.TYPE, Label = ArcazeLedDigit.TYPE });
                                    break;

                                case DeviceType.Output:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightOutput.TYPE, Label = "LED / Output" });
                                    //displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                                    break;

                                case DeviceType.Servo:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightServo.TYPE, Label = MobiFlightServo.TYPE });
                                    break;

                                case DeviceType.Stepper:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightStepper.TYPE, Label = MobiFlightStepper.TYPE });
                                    break;

                                case DeviceType.LcdDisplay:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightLcdDisplay.TYPE, Label = MobiFlightLcdDisplay.TYPE });
                                    break;

                                case DeviceType.CustomDevice:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightCustomDevice.TYPE, Label = MobiFlightCustomDevice.TYPE });
                                    break;

                                case DeviceType.ShiftRegister:
                                    deviceTypeOptions.Add(new ListItem() { Value = MobiFlightShiftRegister.TYPE, Label = MobiFlightShiftRegister.TYPE });
                                    break;
                            }
                        }
                    }

                    if (deviceTypeOptions.Count == 0 && this.Visible)
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

                deviceTypeOptions.Insert(0, new ListItem() { Value = "-", Label = "-" });

                displayTypeComboBox.DataSource = deviceTypeOptions;
                displayTypeComboBox.ValueMember = "Value";
                displayTypeComboBox.DisplayMember = "Label";

                // config can be null
                // because module information is set
                // before config is loaded
                if (config == null) return;
                config.ModuleSerial = rawSerial;

                // third tab
                if (config.DeviceType != null &&
                    !ComboBoxHelper.SetSelectedItemByValue(displayTypeComboBox, config.DeviceType))
                {
                    Log.Instance.log($"Trying to show config but display type {config.DeviceType} not present.", LogSeverity.Error);
                }
            }
            catch (Exception ex)
            {
                displayPinPanel.displayPinBrightnessPanel.Visible = false;
                displayPinPanel.displayPinBrightnessPanel.Enabled = false;
                Log.Instance.log(ex.Message, LogSeverity.Error);
            }
        }

        private void displayTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAllDisplayPanels();

            try
            {
                /*if (((sender as ComboBox).SelectedItem as ListItem) == null) return;
                if (((sender as ComboBox).SelectedItem as ListItem).Value == "-") { return;  }*/

                bool panelEnabled = true;
                ComboBox cb = displayModuleNameComboBox;
                String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
#if ARCAZE
                if (arcazeFirmware.ContainsKey(serial))
                {
                    panelEnabled = InitializeArcazeDisplays(cb, serial);
                }
                else
#endif
                if (SerialNumber.IsMobiFlightSerial(serial))
                {
                    panelEnabled = InitializeMobiFlightDisplays(cb, serial);
                }
                else if (SerialNumber.IsJoystickSerial(serial))
                {
                    panelEnabled = InitializeJoystickDisplays(cb, serial);
                }
                else if (SerialNumber.IsMidiBoardSerial(serial))
                {
                    panelEnabled = InitializeMidiBoardDisplays(cb, serial);
                }

                ShowActiveDisplayPanel(sender, serial, panelEnabled);
            }
            catch (Exception ex)
            {
                Log.Instance.log(ex.Message, LogSeverity.Error);
                MessageBox.Show(i18n._tr("uiMessageNotImplementedYet"),
                                i18n._tr("Hint"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private bool InitializeJoystickDisplays(ComboBox cb, string serial)
        {
            Joystick joystick = _execManager.GetJoystickManager().GetJoystickBySerial(serial);

            displayPinPanel.SetModule(null);
            displayPinPanel.displayPinBrightnessPanel.Visible = false;
            displayPinPanel.displayPinBrightnessPanel.Enabled = false;

            List<ListItem> outputs = new List<ListItem>();
            List<ListItem> lcdDisplays = new List<ListItem>();

            foreach (var deviceType in joystick.GetConnectedOutputDeviceTypes())
            {
                switch (deviceType)
                {
                    case DeviceType.Output:
                        foreach (var device in joystick.GetAvailableOutputDevicesAsListItems())
                            outputs.Add(new ListItem() { Value = device.Label, Label = device.Label });
                        break;
                    case DeviceType.LcdDisplay:
                        foreach (var device in joystick.GetAvailableLcdDevices())
                        {
                            int Cols = (device as MobiFlight.Config.LcdDisplay).Cols;
                            int Lines = (device as MobiFlight.Config.LcdDisplay).Lines;
                            lcdDisplays.Add(new ListItem() { Value = device.Name + "," + Cols + "," + Lines, Label = device.Name });
                        }
                        break;
                }
            }

            displayPinPanel.WideStyle = true;
            displayPinPanel.EnablePWMSelect(false);
            displayPinPanel.SetPorts(new List<ListItem>());
            displayPinPanel.SetPins(outputs);

            displayLcdDisplayPanel.DisableOutputDefinition();
            displayLcdDisplayPanel.SetAddresses(lcdDisplays);

            return true;
        }

        private bool InitializeMidiBoardDisplays(ComboBox cb, string serial)
        {
            MidiBoard midiBoard = _execManager.GetMidiBoardManager().GetMidiBoardBySerial(serial);

            displayPinPanel.SetModule(null);
            displayPinPanel.displayPinBrightnessPanel.Visible = false;
            displayPinPanel.displayPinBrightnessPanel.Enabled = false;

            List<ListItem> outputs = new List<ListItem>();
            foreach (var device in midiBoard.GetAvailableOutputDevices())
                outputs.Add(new ListItem() { Value = device.Name, Label = device.Label });

            displayPinPanel.WideStyle = true;
            displayPinPanel.EnablePWMSelect(false);
            displayPinPanel.SetPorts(new List<ListItem>());
            displayPinPanel.SetPins(outputs);

            return true;
        }

        private void ShowActiveDisplayPanel(object sender, string serial, bool panelEnabled)
        {
            var SelectedItemValue = ((sender as ComboBox).SelectedItem as ListItem)?.Value;
            if (SelectedItemValue == null) return;

            DeviceNotAvailableWarningLabel.Visible = serial == "" && SelectedItemValue != "-" && config != null;

            if (SelectedItemValue == MobiFlightOutput.TYPE)
            {
                displayPinPanel.Enabled = panelEnabled;
                displayPinPanel.Height = displayPanelHeight;
            }

            else if (SelectedItemValue == ArcazeLedDigit.TYPE)
            {
                displayLedDisplayPanel.Enabled = panelEnabled;
                displayLedDisplayPanel.Height = displayPanelHeight;
            }

            else if (SelectedItemValue == DeviceType.Servo.ToString("F"))
            {
                servoPanel.Enabled = panelEnabled;
                servoPanel.Height = displayPanelHeight;
            }

            else if (SelectedItemValue == DeviceType.Stepper.ToString("F"))
            {
                stepperPanel.Enabled = panelEnabled;
                stepperPanel.Height = displayPanelHeight;
            }

            else if (SelectedItemValue == DeviceType.LcdDisplay.ToString("F"))
            {
                displayLcdDisplayPanel.Enabled = panelEnabled;
                displayLcdDisplayPanel.AutoSize = true;
                displayLcdDisplayPanel.Height = displayPanelHeight;
            }

            else if (SelectedItemValue == DeviceType.ShiftRegister.ToString("F"))
            {
                displayShiftRegisterPanel.Enabled = panelEnabled;
                displayShiftRegisterPanel.Height = displayPanelHeight;
            }
            else if (SelectedItemValue == DeviceType.CustomDevice.ToString("F"))
            {
                customDevicePanel.Enabled = panelEnabled;
                customDevicePanel.AutoSize = true;
                customDevicePanel.Height = displayPanelHeight;
            }
            else
            {
                displayNothingSelectedPanel.Enabled = true;
                displayNothingSelectedPanel.Height = displayPanelHeight;
            }
        }

        private bool InitializeMobiFlightDisplays(ComboBox cb, string serial)
        {
            bool panelEnabled = true;
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
            List<ListItem<MobiFlightCustomDevice>> customDevices = new List<ListItem<MobiFlightCustomDevice>>();


            if (module != null)
            {
                foreach (IConnectedDevice device in module.GetConnectedDevices())
                {
                    switch (device.TypeDeprecated)
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

                        case DeviceType.CustomDevice:
                            customDevices.Add(new ListItem<MobiFlightCustomDevice>()
                            {
                                Value = device as MobiFlightCustomDevice,
                                Label = device.Name
                            });
                            break;
                    }
                }
            }
            displayPinPanel.WideStyle = true;
            displayPinPanel.SetPorts(new List<ListItem>());
            displayPinPanel.SetPins(outputs);

            displayLedDisplayPanel.WideStyle = true;

            // we remove the callback method to ensure, that it is not added more than once
            displayLedDisplayPanel.OnLedAddressChanged -= displayLedAddressComboBox_SelectedIndexChanged;
            displayLedDisplayPanel.OnLedAddressChanged += new EventHandler(displayLedAddressComboBox_SelectedIndexChanged);
            displayLedDisplayPanel.SetAddresses(ledSegments);

            servoPanel.SetAdresses(servos);

            stepperPanel.SetAddresses(stepper);

            displayShiftRegisterPanel.shiftRegistersComboBox.SelectedIndexChanged -= shiftRegistersComboBox_selectedIndexChanged;
            displayShiftRegisterPanel.shiftRegistersComboBox.SelectedIndexChanged += new EventHandler(shiftRegistersComboBox_selectedIndexChanged);
            displayShiftRegisterPanel.SetAddresses(shiftRegisters);

            displayLcdDisplayPanel.SetAddresses(lcdDisplays);

            customDevicePanel.SetCustomDeviceNames(customDevices);

            return panelEnabled;
        }

        private bool InitializeArcazeDisplays(ComboBox cb, string serial)
        {
            bool panelEnabled = true;

            switch (cb.SelectedItem.ToString())
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
            
            List<ListItem> pins = new List<ListItem>();
            foreach (String v in ArcazeModule.getPins())
            {
                pins.Add(new ListItem() { Label = v, Value = v });
                //preconditionPinComboBox.Items.Add(v);
            }

            displayPinPanel.SetPins(pins);
            displayPinPanel.WideStyle = false;

            List<ListItem> addr = new List<ListItem>();
            List<ListItem> connectors = new List<ListItem>();
            foreach (string v in ArcazeModule.getDisplayAddresses()) addr.Add(new ListItem() { Label = v, Value = v });
            foreach (string v in ArcazeModule.getDisplayConnectors()) connectors.Add(new ListItem() { Label = v, Value = v });
            displayLedDisplayPanel.WideStyle = false;
            displayLedDisplayPanel.SetAddresses(addr);
            displayLedDisplayPanel.SetConnectors(connectors);

            return panelEnabled;
        }

        private void HideAllDisplayPanels()
        {
            foreach (UserControl p in displayPanels)
            {
                p.Enabled = false;
                p.AutoSize = false;
                p.Height = 0;
            }
        }

        void displayLedAddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = displayModuleNameComboBox;
            String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
            MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);


            // Build list of chained modules and list of selectable sizes
            var chained = new List<ListItem>();
            var entries = new List<ListItem>();
            if (module != null)
            {
                foreach (IConnectedDevice device in module.GetConnectedDevices())
                {
                    if (device.TypeDeprecated != DeviceType.LedModule) continue;
                    if (device.Name != ((sender as ComboBox).SelectedItem as ListItem).Value) continue;
                    // Found the device we sought
                    MobiFlightLedModule dev = device as MobiFlightLedModule;
                    for (int i = 0; i < dev.SubModules; i++)
                    {
                        chained.Add(new ListItem() { Label = (i + 1).ToString(), Value = (i + 1).ToString() });
                    }
                    var maxdigits = 8;

                    if (dev.ModelType == MobiFlight.Config.LedModule.MODEL_TYPE_TM1637_4DIGIT) { maxdigits = 4; }
                    else
                    if (dev.ModelType == MobiFlight.Config.LedModule.MODEL_TYPE_TM1637_6DIGIT) { maxdigits = 6; }

                    for (int i = 2; i < maxdigits; i++)
                    {
                        entries.Add(new ListItem() { Label = (i + 1).ToString(), Value = (i + 1).ToString() });
                    }
                }
            }
            displayLedDisplayPanel.SetConnectors(chained);
            displayLedDisplayPanel.SetSizeDigits(entries);
        }

        #region Stepper related functions
        private void StepperPanel_OnStepperSelected(object sender, StepperConfigChangedEventArgs e)
        {
            // we have a fresh config, nothing to do.
            if (config?.ModuleSerial == null) return;

            String stepperAddress = e.StepperAddress;
            String serial = SerialNumber.ExtractSerial(config.ModuleSerial);

            try
            {
                MobiFlightModule module = _execManager.getMobiFlightModuleCache()
                                                      .GetModuleBySerial(serial);

                if (module == null)
                {
                    // we want to execute the catch block below
                    // GetModuleBySerial used to throw this exception
                    // see #1157
                    throw new IndexOutOfRangeException();
                }

                MobiFlightStepper stepper = module.GetStepper(stepperAddress);

                stepperPanel.setStepperProfile(stepper.Profile);
                stepperPanel.ShowManualCalibration(!stepper.HasAutoZero);
            }
            catch (IndexOutOfRangeException ex)
            {
                // the module with that serial is currently not connected
                // so we cannot lookup anything sensible
                Log.Instance.log($"Trying to show stepper config but module {config.ModuleSerial} is not connected. Using default profile.", LogSeverity.Error);
                stepperPanel.setStepperProfile(MFStepperPanel.Profiles.Find(p => p.Value.id == 0).Value);
            }
        }

        void stepperPanel_OnSetZeroTriggered(object sender, StepperConfigChangedEventArgs e)
        {
            String serial = SerialNumber.ExtractSerial(config.ModuleSerial);
            _execManager.getMobiFlightModuleCache().ResetStepper(serial, (sender as String));
        }

        void stepperPanel_OnManualCalibrationTriggered(object sender, Panels.ManualCalibrationTriggeredEventArgs e)
        {
            // TODO: remove this sync to config
            // to prevent overriding accidentaly something
            syncToConfig();

            string serial = SerialNumber.ExtractSerial(config.ModuleSerial);

            MobiFlightModule module = _execManager.getMobiFlightModuleCache()
                                                    .GetModuleBySerial(serial);

            if (module == null) return;

            var configStepper = config.Device as OutputConfig.Stepper;
            MobiFlightStepper stepper = module.GetStepper(configStepper.Address);

            int CurrentValue = stepper.Position();
            int NextValue = (CurrentValue + e.Steps);

            _execManager.getMobiFlightModuleCache().SetStepper(
                serial,
                configStepper.Address,
                (NextValue).ToString(),
                configStepper.OutputRev,
                configStepper.OutputRev,
                configStepper.CompassMode
            );
        }

        #endregion

        private void shiftRegistersComboBox_selectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = displayModuleNameComboBox;
            String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
            MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);
            bool pwmSupport = false;

            int numModules = 0;

            if (module != null)
            {
                foreach (IConnectedDevice device in module.GetConnectedDevices())
                {
                    if (device.TypeDeprecated != DeviceType.ShiftRegister) continue;
                    if (device.Name != ((sender as ComboBox).SelectedItem as ListItem).Value) continue;
                    numModules = (device as MobiFlightShiftRegister).NumberOfShifters;
                }
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
                catch (Exception ex)
                {
                    Log.Instance.log($"Parsing problem: {ex.Message}", LogSeverity.Error);
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
            DisplayTypePanel.Visible = OutputTypeIsDisplay();
            groupBoxDisplaySettings.Visible = OutputTypeIsDisplay();

            InputActionTypePanel.Visible = OutputTypeIsInputAction();
            inputActionGroupBox.Visible = OutputTypeIsInputAction();

            if (OutputTypeIsInputAction())
            {
                InputTypeButtonRadioButton.Checked = config.AnalogInputConfig?.onChange == null;
                InputTypeAnalogRadioButton.Checked = config.AnalogInputConfig?.onChange != null;
            }
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
