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
using MobiFlight.UI.Panels;
using MobiFlight.UI.Panels.Input;

namespace MobiFlight.UI.Dialogs
{
    public partial class InputConfigWizard : Form
    {
        public event EventHandler PreconditionTreeNodeChanged;
        public event EventHandler SettingsDialogRequested;

        static int lastTabActive = 0;

        ExecutionManager _execManager = null;
        int displayPanelHeight = -1;
        List<UserControl> displayPanels = new List<UserControl>();
        
        InputConfigItem config = null;
        InputConfigItem originalConfig = null;

        ErrorProvider errorProvider = new ErrorProvider();
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        DataSet _dataSetConfig = null;
#if ARCAZE
        Dictionary<string, ArcazeModuleSettings> moduleSettings;
#endif

        UI.Panels.DisplayPinPanel displayPinPanel = new UI.Panels.DisplayPinPanel();
        UI.Panels.DisplayBcdPanel displayBcdPanel = new UI.Panels.DisplayBcdPanel();
        UI.Panels.DisplayLedDisplayPanel displayLedDisplayPanel = new UI.Panels.DisplayLedDisplayPanel();
        UI.Panels.DisplayNothingSelectedPanel displayNothingSelectedPanel = new UI.Panels.DisplayNothingSelectedPanel();
        UI.Panels.ServoPanel servoPanel = new UI.Panels.ServoPanel();

        bool IsShown = false;

        public InputConfigWizard(ExecutionManager mainForm, 
                             InputConfigItem cfg,
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
            preconditionPanel.preparePreconditionPanel(dataSetConfig, filterGuid);
            initConfigRefDropDowns(dataSetConfig, filterGuid);
            _loadPresets();
        }

        private void initConfigRefDropDowns(DataSet dataSetConfig, string filterGuid)
        {
            _dataSetConfig = dataSetConfig;
            DataRow[] rows = dataSetConfig.Tables["config"].Select("guid <> '" + filterGuid + "'");

            // this filters the current config
            DataView dv = new DataView(dataSetConfig.Tables["config"]);
            dv.RowFilter = "guid <> '" + filterGuid + "'";

            configRefPanel.SetConfigRefsDataView(dv, filterGuid);
            displayLedDisplayPanel.SetConfigRefsDataView(dv, filterGuid);
            //displayShiftRegisterPanel.SetConfigRefsDataView(dv, filterGuid);
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
            config = cfg;

            // Until with have the preconditions completely refactored,
            // add an empty precondition in case the current cfg doesn't have one
            // we removed addEmptyNode but add an empty Precondition here
            if (cfg.Preconditions.Count == 0)
                cfg.Preconditions.Add(new Precondition());

            originalConfig = config.Clone() as InputConfigItem;

            InitializeComponent();

            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            //if (cfg.FSUIPCOffset == InputConfigItem.FSUIPCOffsetNull)
            //{
            //    lastTabActive = 0;
            //}
            tabControlFsuipc.SelectedIndex = lastTabActive;

            // PRECONDITION PANEL
            preconditionPanel.Init();
            preconditionPanel.ErrorOnValidating += (sender, e) =>
            {
                tabControlFsuipc.SelectedTab = preconditionTabPage;
            };
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
        public void initWithArcazeCache (ArcazeCache arcazeCache)
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

                PreconditionModuleList.Add(new ListItem() { 
                    Value = $"{module.Name}/ {module.Serial}",
                    Label = $"{module.Name} ({module.Serial})"
                });
            }

            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{module.Name}/ {module.Serial}",
                    Label = $"{module.Name} ({module.Port})"
                });
            }

            foreach (Joystick joystick in _execManager.GetJoystickManager().GetJoysticks())
            {
                if (joystick.GetAvailableDevices().Count > 0)
                    inputModuleNameComboBox.Items.Add(new ListItem()
                    {
                        Value = $"{joystick.Name} / {joystick.Serial}",
                        Label = $"{joystick.Name}"
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

            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{module.Name}/ {module.Serial}",
                    Label = $"{module.Name} ({module.Port})"
                });
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }

            foreach (Joystick joystick in _execManager.GetJoystickManager().GetJoysticks())
            {
                inputModuleNameComboBox.Items.Add(new ListItem()
                {
                    Value = $"{joystick.Name} / {joystick.Serial}",
                    Label = $"{joystick.Name}"
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
            if (!ComboBoxHelper.SetSelectedItem(inputTypeComboBox, config.Name))
            {
                // TODO: provide error message
                Log.Instance.log($"{GetType().Name}:_syncConfigToForm : Exception on selecting item in Display Type ComboBox", LogSeverity.Debug);
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
            config.Name = inputTypeComboBox.Text;

            configRefPanel.syncToConfig(config);

            preconditionPanel.syncToConfig(config);

            if (config.ModuleSerial == "-") return true;

            DeviceType currentInputType = determineCurrentDeviceType(SerialNumber.ExtractSerial(config.ModuleSerial));

            if (groupBoxInputSettings.Controls.Count == 0) return false;

            switch (currentInputType)
            {
                case DeviceType.Button:
                    config.Type = InputConfigItem.TYPE_BUTTON;
                    if (config.button == null) config.button = new InputConfig.ButtonInputConfig();
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as ButtonPanel).ToConfig(config.button);
                    break;

                case DeviceType.Encoder:
                    config.Type = InputConfigItem.TYPE_ENCODER;
                    if (config.encoder == null) config.encoder = new InputConfig.EncoderInputConfig();
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as EncoderPanel).ToConfig(config.encoder);
                    break;

                case DeviceType.InputShiftRegister:
                    config.Type = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER;
                    if (config.inputShiftRegister == null) config.inputShiftRegister = new InputConfig.InputShiftRegisterConfig();
                    config.inputShiftRegister.ExtPin = (int)inputPinDropDown.SelectedItem;
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as InputShiftRegisterPanel).ToConfig(config.inputShiftRegister);
                    break;

                case DeviceType.InputMultiplexer:
                    config.Type = InputConfigItem.TYPE_INPUT_MULTIPLEXER;
                    if (config.inputMultiplexer == null) config.inputMultiplexer = new InputConfig.InputMultiplexerConfig();
                    config.inputMultiplexer.DataPin = (int)inputPinDropDown.SelectedItem;
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as InputMultiplexerPanel).ToConfig(config.inputMultiplexer);
                    break;

                case DeviceType.AnalogInput:
                    config.Type = InputConfigItem.TYPE_ANALOG;
                    if (config.analog == null) config.analog = new InputConfig.AnalogInputConfig();
                    if (groupBoxInputSettings.Controls[0] != null)
                        (groupBoxInputSettings.Controls[0] as AnalogPanel).ToConfig(config.analog);
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
                // disable test button
                // in case that no display is selected                
                String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());

                inputTypeComboBox.Enabled = groupBoxInputSettings.Enabled = (serial != "");
                // serial is empty if no module is selected (on init of form)
                //if (serial == "") return;                

                // update the available types depending on the 
                // type of module
                
                inputTypeComboBox.Items.Clear();
                inputTypeComboBox.ValueMember = "Value";
                inputTypeComboBox.DisplayMember = "Label";

                if (!Joystick.IsJoystickSerial(serial))
                {
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

                    foreach (Config.BaseDevice device in module.GetConnectedInputDevices())
                    {
                        switch (device.Type)
                        {
                            case DeviceType.Button:
                            case DeviceType.AnalogInput:
                            case DeviceType.Encoder:
                            case DeviceType.InputShiftRegister:
                            case DeviceType.InputMultiplexer:
                                inputTypeComboBox.Items.Add(new ListItem<Config.BaseDevice>() { Label = device.Name, Value = device });
                                break;
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
                // Add all Joysticks
                else { 
                    Joystick joystick = _execManager.GetJoystickManager().GetJoystickBySerial(serial);
                    inputTypeComboBox.Items.AddRange(joystick.GetAvailableDevices().ToArray());
                }
                
                // third tab
                if (!ComboBoxHelper.SetSelectedItem(inputTypeComboBox, config.Name))
                {
                    // TODO: provide error message
                    Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Problem setting Display Type ComboBox", LogSeverity.Debug);
                }

            }
            catch (Exception ex)
            {
                Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Some Exception occurred" + ex.Message, LogSeverity.Debug);
            }
        }

        private DeviceType determineCurrentDeviceType(String serial)
        {
            DeviceType currentInputType = DeviceType.Button;

            if (!Joystick.IsJoystickSerial(serial)) { 
                MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

                // find the correct input type based on the name
                foreach (Config.BaseDevice device in module.GetConnectedInputDevices())
                {
                    if ((inputTypeComboBox.SelectedItem as ListItem<Config.BaseDevice>) == null) 
                        break;
                    if (device.Name != ((inputTypeComboBox.SelectedItem as ListItem<Config.BaseDevice>).Value as Config.BaseDevice)?.Name) continue;

                    currentInputType = device.Type;
                    break;
                }
            } else
            {
                // We have a joystick 
                // which right now is only buttons
                if (inputTypeComboBox.SelectedItem.ToString().Contains(Joystick.AxisPrefix))
                    currentInputType = DeviceType.AnalogInput;
            }

            return currentInputType;
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
                        panel = new Panels.Input.ButtonPanel();
                        (panel as Panels.Input.ButtonPanel).SetVariableReferences(_execManager.GetAvailableVariables());
                        (panel as Panels.Input.ButtonPanel).syncFromConfig(config.button);
                        break;

                    case DeviceType.Encoder:
                        panel = new Panels.Input.EncoderPanel();
                        (panel as Panels.Input.EncoderPanel).SetVariableReferences(_execManager.GetAvailableVariables());
                        (panel as Panels.Input.EncoderPanel).syncFromConfig(config.encoder);
                        break;

                    case DeviceType.InputShiftRegister:
                        Config.InputShiftRegister selectedInputShifter = (inputTypeComboBox.SelectedItem as ListItem<Config.BaseDevice>).Value as Config.InputShiftRegister;
                        panel = new Panels.Input.InputShiftRegisterPanel();
                        (panel as Panels.Input.InputShiftRegisterPanel).syncFromConfig(config.inputShiftRegister);
                        PopulateInputPinDropdown(Convert.ToInt32(selectedInputShifter.NumModules), config.inputShiftRegister?.ExtPin);
                        inputPinDropDown.Visible = true;
                        break;

                    case DeviceType.InputMultiplexer:
                        Config.InputMultiplexer selectedInputMultiplexer = (inputTypeComboBox.SelectedItem as ListItem<Config.BaseDevice>).Value as Config.InputMultiplexer;
                        panel = new Panels.Input.InputMultiplexerPanel();
                        (panel as Panels.Input.InputMultiplexerPanel).syncFromConfig(config.inputMultiplexer);
                        PopulateInputPinDropdown(Convert.ToInt32(selectedInputMultiplexer.NumBytes), config.inputMultiplexer?.DataPin);
                        inputPinDropDown.Visible = true;
                        break;

                    case DeviceType.AnalogInput:
                        panel = new Panels.Input.AnalogPanel();
                        (panel as Panels.Input.AnalogPanel).SetVariableReferences(_execManager.GetAvailableVariables());
                        (panel as Panels.Input.AnalogPanel).syncFromConfig(config.analog);
                        break;
                }

                if (panel != null)
                {
                    panel.Padding = new Padding(2, 0, 2, 0);
                    groupBoxInputSettings.Controls.Add(panel);
                    panel.Dock = DockStyle.Fill;
                }
            }
            catch (Exception exc)
            {
                Log.Instance.log("InputConfigWizard.inputTypeComboBox_SelectedIndexChanged: EXC " + exc.Message, LogSeverity.Debug);
                MessageBox.Show(i18n._tr("uiMessageNotImplementedYet"), 
                                i18n._tr("Hint"), 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
        }

        void displayLedAddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = inputModuleNameComboBox;                
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
            if (inputTypeComboBox.Text == ArcazeLedDigit.TYPE)                
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
                
        private void tabControlFsuipc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if running in test mode
            lastTabActive = (sender as TabControl).SelectedIndex;
        }

        private void InputConfigWizard_Shown(object sender, EventArgs e)
        {
            IsShown = true;
        }

        private void InputConfigWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            groupBoxInputSettings.Dispose();
        }
    }
}
