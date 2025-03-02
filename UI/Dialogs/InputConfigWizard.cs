using MobiFlight.Base;
using MobiFlight.Config;
using MobiFlight.UI.Panels.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class InputConfigWizard : Form
    {
        struct ButtonStyle
        {
            public Color BackColor;
            public Color ForeColor;
            public Color BorderColor;
        }

        public event EventHandler PreconditionTreeNodeChanged;
        public event EventHandler SettingsDialogRequested;

        protected bool ScanningForInput = false;

        static int lastTabActive = 0;

        ExecutionManager _execManager = null;
        int displayPanelHeight = -1;
        List<UserControl> displayPanels = new List<UserControl>();

        InputConfigItem config = null;
        public InputConfigItem Config { get { return config; } }

        InputConfigItem originalConfig = null;

        ErrorProvider errorProvider = new ErrorProvider();
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        List<OutputConfigItem> outputConfigItems = null;
#if ARCAZE
        Dictionary<string, ArcazeModuleSettings> moduleSettings;
#endif
        
        ButtonStyle ScanForInputButtonDefaultStyle;
        Dictionary<string, int> ScanForInputThreshold = new Dictionary<string, int>();

        bool IsShown = false;

        public InputConfigWizard(ExecutionManager mainForm,
                             InputConfigItem cfg,
#if ARCAZE
                             ArcazeCache arcazeCache,
                             Dictionary<string, ArcazeModuleSettings> moduleSettings,
#endif
                             List<OutputConfigItem> outputConfigItems)
        {
            Init(mainForm, cfg);
#if ARCAZE
            this.moduleSettings = moduleSettings;
            initWithArcazeCache(arcazeCache);
#else
            initWithoutArcazeCache();
#endif
            // copy this so that no filtering will 
            // impact the list of displayed items
            // https://github.com/MobiFlight/MobiFlight-Connector/issues/1447
            this.outputConfigItems = outputConfigItems.ToArray().ToList();

            var list = outputConfigItems.Where(c => c.GUID != cfg.GUID)
                                     .Select(c => new ListItem() { Label = c.Name, Value = c.GUID }) as List<ListItem>;

            preconditionPanel.SetAvailableConfigs(list);
            preconditionPanel.SetAvailableVariables(mainForm.GetAvailableVariables());
            initConfigRefDropDowns(this.outputConfigItems, cfg.GUID);
            _loadPresets();

            // remember the default style of the button
            ScanForInputButtonDefaultStyle.BackColor = ScanForInputButton.BackColor;
            ScanForInputButtonDefaultStyle.ForeColor = ScanForInputButton.ForeColor;
            ScanForInputButtonDefaultStyle.BorderColor = ScanForInputButton.FlatAppearance.BorderColor;

            // Append the row description to the window title if one was provided.
            if (!String.IsNullOrEmpty(cfg.Name))
            {
                this.Text = $"{this.Text} - {cfg.Name}";
            }
        }

        private void initConfigRefDropDowns(List<OutputConfigItem> dataSetConfig, string filterGuid)
        {
            configRefPanel.SetConfigRefsDataView(dataSetConfig, filterGuid);
        }

        private void ConfigWizard_Load(object sender, EventArgs e)
        {
            _syncConfigToForm(config);
        }

        public bool ConfigHasChanged()
        {
            return !originalConfig.Equals(config);
        }

        protected void Init(ExecutionManager mainForm, InputConfigItem cfg)
        {
            this._execManager = mainForm;
            // create a clone so that we don't edit 
            // the original item
            config = cfg.Clone() as InputConfigItem;

            // this is only stored to be able
            // to check for modifications
            originalConfig = cfg;

            InitializeComponent();

            ActivateCorrectTab(config);

            // PRECONDITION PANEL
            preconditionPanel.Init();
            preconditionPanel.ErrorOnValidating += (sender, e) =>
            {
                tabControlFsuipc.SelectedTab = preconditionTabPage;
            };
        }

        private void ActivateCorrectTab(InputConfigItem cfg)
        {
            // by default always the first tab is activated
            // if one opens the dialog for an existing config
            // we use the lastTabActive
            if (cfg?.ModuleSerial != null && cfg?.ModuleSerial != SerialNumber.NOT_SET)
            {
                tabControlFsuipc.SelectedIndex = lastTabActive;
            }
        }

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(Properties.Settings.Default.PresetFileOutputs))
            {
                isLoaded = false;
                MessageBox.Show(i18n._tr("uiMessageConfigWizard_PresetsNotFound"), i18n._tr("Hint"));
            }
            else
            {

                try
                {
                    presetsDataSet.Clear();
                    presetsDataSet.ReadXml(Properties.Settings.Default.PresetFileOutputs);
                    DataRow[] rows = presetDataTable.Select("", "description");
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
                }
            }
        }

#if ARCAZE
        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        /// <param name="arcazeCache"></param>
        public void initWithArcazeCache(ArcazeCache arcazeCache)
        {
            List<ListItem> PreconditionModuleList = new List<ListItem>();

            inputModuleNameComboBox.Items.Clear();
            inputModuleNameComboBox.Items.Add(new ListItem() { Value = "-", Label = "-" });
            inputModuleNameComboBox.SelectedIndex = 0;
            inputModuleNameComboBox.DisplayMember = "Label";
            inputModuleNameComboBox.ValueMember = "Value";

            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                arcazeFirmware[module.Serial] = module.Version;

                PreconditionModuleList.Add(new ListItem()
                {
                    Value = $"{module.Name}{SerialNumber.SerialSeparator}{module.Serial}",
                    Label = $"{module.Name} ({module.Serial})"
                });
            }

            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().GetModuleInfo())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{module.Name}{SerialNumber.SerialSeparator}{module.Serial}",
                    Label = $"{module.Name} ({module.Port})"
                });
            }

            foreach (Joystick joystick in _execManager.GetJoystickManager().GetJoysticks())
            {
                if (joystick.GetAvailableDevicesAsListItems().Count > 0)
                    inputModuleNameComboBox.Items.Add(new ListItem()
                    {
                        Value = $"{joystick.Name} {SerialNumber.SerialSeparator}{joystick.Serial}",
                        Label = $"{joystick.Name}"
                    });
            }

            foreach (MidiBoard midiBoard in _execManager.GetMidiBoardManager().GetMidiBoards())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{midiBoard.Name} {SerialNumber.SerialSeparator}{midiBoard.Serial}",
                    Label = $"{midiBoard.Name}"
                });
            }

            preconditionPanel.SetModules(PreconditionModuleList);
        }
#endif

        public void initWithoutArcazeCache()
        {
            List<ListItem> PreconditionModuleList = new List<ListItem>();
            // update the display box with
            // modules
            inputModuleNameComboBox.Items.Clear();
            inputModuleNameComboBox.Items.Add(new ListItem() { Value = "-", Label = "-" });
            inputModuleNameComboBox.SelectedIndex = 0;
            inputModuleNameComboBox.DisplayMember = "Label";
            inputModuleNameComboBox.ValueMember = "Value";

            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().GetModuleInfo())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{module.Name}{SerialNumber.SerialSeparator}{module.Serial}",
                    Label = $"{module.Name}{SerialNumber.SerialSeparator}({module.Port})"
                });
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }

            foreach (Joystick joystick in _execManager.GetJoystickManager().GetJoysticks())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{joystick.Name} {SerialNumber.SerialSeparator}{joystick.Serial}",
                    Label = $"{joystick.Name}"
                });
            }

            foreach (MidiBoard midiBoard in _execManager.GetMidiBoardManager().GetMidiBoards())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{midiBoard.Name} {SerialNumber.SerialSeparator}{midiBoard.Serial}",
                    Label = $"{midiBoard.Name}"
                });
            }

            preconditionPanel.SetModules(PreconditionModuleList);
        }

        /// <summary>
        /// sync the values from config with the config wizard form
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool _syncConfigToForm(InputConfigItem config)
        {
            string serial = null;
            if (config == null) throw new Exception(i18n._tr("uiException_ConfigItemNotFound"));
            // first tab                        
            serial = SerialNumber.ExtractSerial(config.ModuleSerial);
            if (serial != "")
            {
                if (!ComboBoxHelper.SetSelectedItemByValue(inputModuleNameComboBox, config.ModuleSerial))
                {
                    // TODO: provide error message
                }
            }

            // second tab
            if (config.DeviceName != null && !ComboBoxHelper.SetSelectedDeviceByDeviceName(inputTypeComboBox, config.DeviceName))
            {
                // TODO: provide error message
                Log.Instance.log($"Exception on selecting item in input type ComboBox. {config.DeviceName}", LogSeverity.Error);
            }

            preconditionPanel.syncFromConfig(config);

            configRefPanel.syncFromConfig(config);

            return true;
        }

        private void PopulateInputPinDropdown(int numModules, int? selectedPin)
        {
            // Originally added for Input shift registers
            // Also used for digital input multiplexers, usually with numModules=2 (CD4067) or 1 (CD4051)
            // The selected input in the dropdown is the shift register details, which includes the
            // number of connected modules. That gets multiplied by 8 pins per module to get the total
            // number of available pins to populate.
            int totalPins = numModules * 8;

            inputPinDropDown.Items.Clear();
            for (int i = 0; i < totalPins; i++)
            {
                inputPinDropDown.Items.Add(i);
            }

            inputPinDropDown.SelectedItem = selectedPin ?? 0;
        }

        /// <summary>
        /// sync current status of form values to config
        /// </summary>
        /// <returns></returns>
        protected bool _syncFormToConfig()
        {
            config.ModuleSerial = inputModuleNameComboBox.SelectedItem.ToString();

            configRefPanel.syncToConfig(config);

            preconditionPanel.syncToConfig(config);

            if (config.ModuleSerial == "-") return true;

            IBaseDevice device = ((ListItem<IBaseDevice>)inputTypeComboBox.SelectedItem).Value;
            if (device.Label != InputConfigItem.TYPE_NOTSET)
                config.DeviceName = device.Name;

            DeviceType currentInputType = determineCurrentDeviceType(SerialNumber.ExtractSerial(config.ModuleSerial));

            //if (groupBoxInputSettings.Controls.Count == 0) return false;

            switch (currentInputType)
            {
                case DeviceType.Button:
                    config.DeviceType = InputConfigItem.TYPE_BUTTON;
                    if (config.button == null) config.button = new InputConfig.ButtonInputConfig();
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as ButtonPanel).ToConfig(config.button);
                    break;

                case DeviceType.Encoder:
                    config.DeviceType = InputConfigItem.TYPE_ENCODER;
                    if (config.encoder == null) config.encoder = new InputConfig.EncoderInputConfig();
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as EncoderPanel).ToConfig(config.encoder);
                    break;

                case DeviceType.InputShiftRegister:
                    config.DeviceType = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER;
                    if (config.inputShiftRegister == null) config.inputShiftRegister = new InputConfig.InputShiftRegisterConfig();
                    config.inputShiftRegister.ExtPin = (int)inputPinDropDown.SelectedItem;
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as ButtonPanel).ToConfig(config.inputShiftRegister);
                    break;

                case DeviceType.InputMultiplexer:
                    config.DeviceType = InputConfigItem.TYPE_INPUT_MULTIPLEXER;
                    if (config.inputMultiplexer == null) config.inputMultiplexer = new InputConfig.InputMultiplexerConfig();
                    config.inputMultiplexer.DataPin = (int)inputPinDropDown.SelectedItem;
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as ButtonPanel).ToConfig(config.inputMultiplexer);
                    break;

                case DeviceType.AnalogInput:
                    config.DeviceType = InputConfigItem.TYPE_ANALOG;
                    if (config.analog == null) config.analog = new InputConfig.AnalogInputConfig();
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as AnalogPanel).ToConfig(config.analog);
                    break;

                case DeviceType.NotSet:
                    config.DeviceType = InputConfigItem.TYPE_NOTSET;
                    config.DeviceName = InputConfigItem.TYPE_NOTSET;
                    break;
            }

            return true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                return;
            }
            _syncFormToConfig();
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void ModuleSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide the input shifter / dig. input mux pin dropdown whenever the module changes. 
            // It will be made visible again in inputTypeComboBox_SelectedIndexChanged() 
            // when the user selects an input type.
            inputPinDropDown.Visible = false;

            // check which extension type is available to current serial
            ComboBox cb = (sender as ComboBox);
            try
            {
                String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());

                inputTypeComboBox.Items.Clear();
                inputTypeComboBox.ValueMember = "Value";
                inputTypeComboBox.DisplayMember = "Label";

                IBaseDevice emptyDevice = new BaseDevice() { Name = InputConfigItem.TYPE_NOTSET };
                inputTypeComboBox.Items.Add(new ListItem<IBaseDevice>() { Label = InputConfigItem.TYPE_NOTSET, Value = emptyDevice });
                inputTypeComboBox.SelectedIndex = 0;
                inputTypeComboBox.Enabled = true;

                if (string.IsNullOrEmpty(serial))
                {
                    BaseDevice device = null;

                    if (config.button != null)
                    {
                        device = new Config.Button();
                    }
                    else if (config.encoder != null)
                    {
                        device = new Encoder();
                    }
                    else if (config.analog != null)
                    {
                        device = new AnalogInput();
                    }
                    else if (config.inputShiftRegister != null)
                    {
                        device = new InputShiftRegister();
                    }
                    else if (config.inputMultiplexer != null)
                    {
                        device = new InputMultiplexer();
                    }

                    if (device != null)
                    {
                        device.Name = config.DeviceName;
                        inputTypeComboBox.Items.Add(new ListItem<IBaseDevice>() { Label = device.Label, Value = device });
                    }
                    inputTypeComboBox.Enabled = false;
                }
                // Add all Joysticks
                else if (Joystick.IsJoystickSerial(serial))
                {
                    Joystick joystick = _execManager.GetJoystickManager().GetJoystickBySerial(serial);
                    inputTypeComboBox.Items.AddRange(joystick.GetAvailableDevicesAsListItems().ToArray());
                }
                // Add all MidiBoards
                else if (MidiBoard.IsMidiBoardSerial(serial))
                {
                    MidiBoard midiBoard = _execManager.GetMidiBoardManager().GetMidiBoardBySerial(serial);
                    var devices = midiBoard.GetAvailableDevices();
                    foreach (var device in devices)
                    {
                        inputTypeComboBox.Items.Add(new ListItem<IBaseDevice>() { Label = device.Label, Value = device });
                    }
                }
                else
                {
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

                    if (module != null)
                    {
                        foreach (Config.BaseDevice device in module.GetConnectedInputDevices())
                        {
                            switch (device.Type)
                            {
                                case DeviceType.Button:
                                case DeviceType.AnalogInput:
                                case DeviceType.Encoder:
                                case DeviceType.InputShiftRegister:
                                case DeviceType.InputMultiplexer:
                                    inputTypeComboBox.Items.Add(new ListItem<IBaseDevice>() { Label = device.Name, Value = device });
                                    break;
                            }
                        }
                    }

                    if (inputTypeComboBox.Items.Count == 0 && this.IsShown)
                    {
                        if (MessageBox.Show(
                                i18n._tr("uiMessageSelectedModuleDoesNotContainAnyInputDevices"),
                                i18n._tr("Hint"),
                                MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes
                            )
                        {
                            if (SettingsDialogRequested != null)
                            {
                                SettingsDialogRequested(module, new EventArgs());

                                // trigger reload of Type ComboBox
                                int CurrentIdx = inputModuleNameComboBox.SelectedIndex;
                                inputModuleNameComboBox.SelectedIndex = 0;
                                inputModuleNameComboBox.SelectedIndex = CurrentIdx;
                            }
                        }
                    }
                }

                // third tab
                if (config.DeviceName != null && !ComboBoxHelper.SetSelectedDeviceByDeviceName(inputTypeComboBox, config.DeviceName))
                {
                    Log.Instance.log($"Problem setting input type ComboBox. {config.DeviceName}", LogSeverity.Error);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log(ex.Message, LogSeverity.Error);
            }
        }

        private DeviceType determineCurrentDeviceType(String serial)
        {
            IBaseDevice device = (inputTypeComboBox.SelectedItem as ListItem<IBaseDevice>)?.Value;
            if (device != null && !string.IsNullOrEmpty(device?.Name))
                return device.Type;
            else
                return DeviceType.NotSet;
        }

        private void inputTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Control panel = null;
            groupBoxInputSettings.Controls.Clear();
            inputPinDropDown.Visible = false;

            try
            {
                bool panelEnabled = true;
                // get the deviceinfo for the current arcaze
                ComboBox cb = inputModuleNameComboBox;
                String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());

                // we remove the callback method to ensure, that it is not added more than once
                // displayLedDisplayPanel.displayLedAddressComboBox.SelectedIndexChanged -= displayLedAddressComboBox_SelectedIndexChanged;

                DeviceType currentInputType = determineCurrentDeviceType(serial);

                switch (currentInputType)
                {
                    case DeviceType.Button:
                        panel = new Panels.Input.ButtonPanel()
                        {
                            Enabled = (serial != "")
                        };
                        (panel as Panels.Input.ButtonPanel).SetVariableReferences(_execManager.GetAvailableVariables());
                        (panel as Panels.Input.ButtonPanel).syncFromConfig(config.button);
                        break;

                    case DeviceType.Encoder:
                        panel = new Panels.Input.EncoderPanel()
                        {
                            Enabled = (serial != "")
                        };
                        (panel as Panels.Input.EncoderPanel).SetVariableReferences(_execManager.GetAvailableVariables());
                        (panel as Panels.Input.EncoderPanel).syncFromConfig(config.encoder);
                        break;

                    case DeviceType.InputShiftRegister:
                        Config.InputShiftRegister selectedInputShifter = (inputTypeComboBox.SelectedItem as ListItem<Config.IBaseDevice>).Value as Config.InputShiftRegister;
                        panel = new Panels.Input.ButtonPanel()
                        {
                            Enabled = (serial != "")
                        };
                        (panel as Panels.Input.ButtonPanel).syncFromConfig(config.inputShiftRegister);
                        PopulateInputPinDropdown(Convert.ToInt32(selectedInputShifter.NumModules), config.inputShiftRegister?.ExtPin);
                        inputPinDropDown.Visible = true;
                        break;

                    case DeviceType.InputMultiplexer:
                        Config.InputMultiplexer selectedInputMultiplexer = (inputTypeComboBox.SelectedItem as ListItem<Config.IBaseDevice>).Value as Config.InputMultiplexer;
                        panel = new Panels.Input.ButtonPanel()
                        {
                            Enabled = (serial != "")
                        };
                        (panel as Panels.Input.ButtonPanel).syncFromConfig(config.inputMultiplexer);
                        PopulateInputPinDropdown(Convert.ToInt32(selectedInputMultiplexer.NumBytes), config.inputMultiplexer?.DataPin);
                        inputPinDropDown.Visible = true;
                        break;

                    case DeviceType.AnalogInput:
                        panel = new Panels.Input.AnalogPanel()
                        {
                            Enabled = (serial != "")
                        };
                        (panel as Panels.Input.AnalogPanel).SetVariableReferences(_execManager.GetAvailableVariables());
                        (panel as Panels.Input.AnalogPanel).syncFromConfig(config.analog);
                        break;
                }

                DeviceNotAvailableWarningLabel.Visible = (serial == "") && currentInputType != DeviceType.NotSet;

                if (panel != null)
                {
                    panel.Padding = new Padding(2, 0, 2, 0);
                    groupBoxInputSettings.Controls.Add(panel);
                    panel.Dock = DockStyle.Fill;
                }
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

        private void tabControlFsuipc_SelectedIndexChanged(object sender, EventArgs e)
        {
            lastTabActive = (sender as TabControl).SelectedIndex;
        }

        private void InputConfigWizard_Shown(object sender, EventArgs e)
        {
            IsShown = true;
        }

        private void InputConfigWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            DeactivateScanForInputMode();
            groupBoxInputSettings.Dispose();
        }

        private void ScanForInputButton_Click(object sender, EventArgs e)
        {
            if (!ScanningForInput)
            {
                ActivateScanForInputMode();
            }
            else
            {
                DeactivateScanForInputMode();
            }
        }

        private void ActivateScanForInputMode()
        {
            ScanningForInput = true;
            ScanForInputButton.BackColor = Color.FromArgb(55, 110, 220);
            ScanForInputButton.ForeColor = Color.White;
            ScanForInputButton.FlatAppearance.BorderColor = Color.FromArgb(55, 110, 220);
            ScanForInputButton.Text = "Scanning...";
            _execManager.getMobiFlightModuleCache().OnButtonPressed += ScanforInput_OnButtonPressed;
            _execManager.GetJoystickManager().OnButtonPressed += ScanforInput_OnButtonPressed;
            _execManager.GetMidiBoardManager().OnButtonPressed += ScanforInput_OnButtonPressed;
        }

        private void DeactivateScanForInputMode()
        {
            ScanningForInput = false;
            _execManager.getMobiFlightModuleCache().OnButtonPressed -= ScanforInput_OnButtonPressed;
            _execManager.GetJoystickManager().OnButtonPressed -= ScanforInput_OnButtonPressed;
            _execManager.GetMidiBoardManager().OnButtonPressed -= ScanforInput_OnButtonPressed;

            ScanForInputButton.BackColor = ScanForInputButtonDefaultStyle.BackColor;
            ScanForInputButton.ForeColor = ScanForInputButtonDefaultStyle.ForeColor;
            ScanForInputButton.FlatAppearance.BorderColor = ScanForInputButtonDefaultStyle.BorderColor;
            ScanForInputButton.Text = "Scan for input";

            ScanForInputThreshold.Clear();
            // remove focus from button
            // to make it look like before clicking on it
            ScanForInputButton.Parent.Focus();
        }

        // required for correct thread-safe entry
        // into the ScanforInput_OnButtonPressed method
        delegate void ScanforInput_OnButtonPressedCallback(object sender, InputEventArgs e);

        private void ScanforInput_OnButtonPressed(object sender, InputEventArgs e)
        {
            if (!InputThresholdIsExceeded(e)) return;

            // For buttons, only the "positive" PRESS events matter
            if (e.Type == DeviceType.Button && e.Value != (int)MobiFlightButton.InputEvent.PRESS)
            {
                return;
            }

            if (inputModuleNameComboBox.InvokeRequired)
            {
                inputModuleNameComboBox.BeginInvoke(new ScanforInput_OnButtonPressedCallback(ScanforInput_OnButtonPressed), new object[] { sender, e });
                return;
            }

            var module = inputModuleNameComboBox.Items.Cast<ListItem>().Where(i => i.Value.ToString().Contains(e.Serial)).First();

            if (module == null) { return; }
            inputModuleNameComboBox.SelectedItem = module;

            // try to set the device
            if (SerialNumber.IsJoystickSerial(e.Serial))
            {
                ComboBoxHelper.SetSelectedItem(inputTypeComboBox, e.DeviceLabel);
            }
            else if (SerialNumber.IsMidiBoardSerial(e.Serial))
            {
                // Add item to device list if not yet there
                if (!inputTypeComboBox.Items.OfType<ListItem<IBaseDevice>>().Any(i => i.Value.Name == e.DeviceId))
                {
                    MidiBoardDevice mbd = new MidiBoardDevice();
                    mbd.Label = e.DeviceLabel;
                    mbd.Name = e.DeviceId;
                    mbd.Type = DeviceType.Button;
                    inputTypeComboBox.Items.Add(new ListItem<IBaseDevice> { Label = mbd.Label, Value = mbd });
                }
                ComboBoxHelper.SetSelectedItem(inputTypeComboBox, e.DeviceLabel);
            }
            else
            {
                ComboBoxHelper.SetSelectedItem(inputTypeComboBox, e.DeviceId);
                // if multiplexer or inputshiftregister set the sub item too
                if (e.Type == DeviceType.InputMultiplexer || e.Type == DeviceType.InputShiftRegister)
                {
                    ComboBoxHelper.SetSelectedItem(inputPinDropDown, e.ExtPin.ToString());
                }
            }


            DeactivateScanForInputMode();
        }

        private bool InputThresholdIsExceeded(InputEventArgs e)
        {
            const int JoystickThreshold = 2000;
            const int AnalogInputThreshold = 20;

            if ((SerialNumber.IsJoystickSerial(e.Serial) &&
                e.DeviceId.Contains(Joystick.AxisPrefix)) || e.Type == DeviceType.AnalogInput)
            {
                if (ScanForInputThreshold.ContainsKey(e.Serial + e.DeviceId))
                {
                    if (Math.Abs(e.Value - ScanForInputThreshold[e.Serial + e.DeviceId]) < (SerialNumber.IsJoystickSerial(e.Serial) ? JoystickThreshold : AnalogInputThreshold))
                    {
                        return false;
                    }
                }
                else
                {
                    ScanForInputThreshold[e.Serial + e.DeviceId] = e.Value;
                    return false;
                }
            }

            return true;
        }
    }
}
