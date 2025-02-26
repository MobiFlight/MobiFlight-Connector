using MobiFlight.Base;
using MobiFlight.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class ConfigWizard : Form
    {
        public event EventHandler SettingsDialogRequested;

        static int lastTabActive = 0;

        ExecutionManager _execManager = null;
        OutputConfigItem config = null;
        OutputConfigItem originalConfig = null;
        ErrorProvider errorProvider = new ErrorProvider();
        List<OutputConfigItem> outputConfigs = null;
        Timer TestTimer = new Timer();
        public OutputConfigItem Config { get { return config; } }

#if ARCAZE
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        Dictionary<string, ArcazeModuleSettings> moduleSettings;
#endif

        public ConfigWizard(ExecutionManager mainForm,
                             OutputConfigItem cfg,
#if ARCAZE
                             ArcazeCache arcazeCache,
                             Dictionary<string, ArcazeModuleSettings> moduleSettings,
#endif
                             List<OutputConfigItem> outputConfigs)
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
            this.outputConfigs = outputConfigs.ToArray().ToList();
            var list = this.outputConfigs.Where(c => c.GUID != cfg.GUID)
                                     .Select(c => new ListItem() { Label = c.Name, Value = c.GUID }).ToList();

            preconditionPanel.SetAvailableConfigs(list);
            preconditionPanel.SetAvailableVariables(mainForm.GetAvailableVariables());
            initConfigRefDropDowns(this.outputConfigs, cfg.GUID);

            // Append the row description to the window title if one was provided.
            if (!String.IsNullOrEmpty(cfg.Name))
            {
                this.Text = $"{this.Text} - {cfg.Name}";
            }
        }

        private void initConfigRefDropDowns(List<OutputConfigItem> outputConfigs, string filterGuid)
        {
            configRefPanel.SetConfigRefsDataView(outputConfigs, filterGuid);
            displayPanel1.SetConfigRefsDataView(outputConfigs, filterGuid);
        }

        public bool ConfigHasChanged()
        {
            return !originalConfig.Equals(config);
        }

        protected void Init(ExecutionManager executionManager, OutputConfigItem cfg)
        {
            this._execManager = executionManager;
            // create a clone so that we don't edit 
            // the original item
            config = cfg.Clone() as OutputConfigItem;

            // this is only stored to be able
            // to check for modifications
            originalConfig = cfg;

            InitializeComponent();

            ActivateCorrectTab(config);

            // DISPLAY PANEL
            displayPanel1.Init(_execManager);
            displayPanel1.DisplayPanelValidatingError += (sender, args) =>
            {
                tabControlFsuipc.SelectedTab = displayTabPage;
            };
            displayPanel1.SettingsDialogRequested += (sender, args) => { SettingsDialogRequested?.Invoke(sender, args); };

            // PRECONDITION PANEL
            preconditionPanel.Init();
            preconditionPanel.ErrorOnValidating += (sender, e) =>
            {
                tabControlFsuipc.SelectedTab = preconditionTabPage;
            };

            variablePanel1.SetVariableReferences(_execManager.GetAvailableVariables());

            // FSUIPC PANEL
            fsuipcConfigPanel.setMode(true);
            // fsuipcConfigPanel.syncFromConfig(cfg);

            // SIMCONNECT SIMVARS PANEL
            simConnectPanel1.HubHopPresetPanel.OnGetLVarListRequested += SimConnectPanel1_OnGetLVarListRequested;
            _execManager.GetSimConnectCache().LVarListUpdated += ConfigWizard_LVarListUpdated;

            fsuipcConfigPanel.ModifyTabLink += ConfigPanel_ModifyTabLink;
            fsuipcConfigPanel.PresetChanged += FsuipcConfigPanel_PresetChanged;
            simConnectPanel1.ModifyTabLink += ConfigPanel_ModifyTabLink;
            xplaneDataRefPanel1.ModifyTabLink += ConfigPanel_ModifyTabLink;
            variablePanel1.ModifyTabLink += ConfigPanel_ModifyTabLink;

            testValuePanel1.FromConfig(config);
            testValuePanel1.TestModeStart += TestValuePanel_TestModeStart;
            testValuePanel1.TestModeStop += TestValuePanel_TestModeEnd;
            testValuePanel1.TestValueChanged += ModifierPanel1_ModifierChanged;
            TestTimer.Interval = Properties.Settings.Default.TestTimerInterval;
            TestTimer.Tick += TestTimer_Tick;
            modifierPanel1.ModifierChanged += ModifierPanel1_ModifierChanged;
        }

        private void ModifierPanel1_ModifierChanged(object sender, EventArgs e)
        {
            testValuePanel1.ToConfig(config);
            modifierPanel1.toConfig(config);
        }

        private void TestTimer_Tick(object sender, EventArgs e)
        {
            TestTimer.Interval = Properties.Settings.Default.TestTimerInterval;
            var value = config.TestValue.Clone() as ConnectorValue;
            if (value == null) value = new ConnectorValue();

            try
            {
                var configRefs = CreateTestConfigRefs(config.ConfigRefs);
                // Apply all modifiers to the test value
                // so that the test value yields the final value
                config.Modifiers.Items.FindAll(x => x.Active).ForEach(y => value = y.Apply(value, configRefs));
            }
            catch (Exception ex)
            {
                // ShowError? Or don't do anything?
            }

            testValuePanel1.Result = value.ToString();

            _execManager.ExecuteTestOn(config, value);
        }

        private List<ConfigRefValue> CreateTestConfigRefs(ConfigRefList configRefs)
        {
            var result = new List<ConfigRefValue>();
            foreach (ConfigRef configRef in configRefs)
            {
                if (!configRef.Active) continue;
                result.Add(new ConfigRefValue() { ConfigRef = configRef, Value = configRef.TestValue });
            }
            return result;
        }

        private void TestValuePanel_TestModeEnd(object sender, EventArgs e)
        {
            _testModeStop(false);
        }

        private void TestValuePanel_TestModeStart(object sender, ConnectorValue value)
        {
            _syncFormToConfig();
            try
            {
                modifierPanel1.toConfig(config);
                TestTimer.Interval = 10;
                TestTimer.Start();
            }
            catch (Exception e)
            {
                Log.Instance.log($"Error starting test mode: {e.Message}", LogSeverity.Error);
            }
        }

        private void FsuipcConfigPanel_PresetChanged(object sender, IFsuipcConfigItem newPreset)
        {
            modifierPanel1.ReplaceModifiers((newPreset as IFsuipcConfigItem)?.Modifiers);
        }

        private void ConfigPanel_ModifyTabLink(object sender, EventArgs e)
        {
            tabControlFsuipc.SelectedTab = compareTabPage;
        }

        private void ActivateCorrectTab(OutputConfigItem cfg)
        {
            // by default always the first tab is activated.
            // if one opens the dialog for an existing config,
            // then we use the lastTabActive
            if (cfg?.ModuleSerial != null && cfg?.ModuleSerial != SerialNumber.NOT_SET)
            {
                tabControlFsuipc.SelectedIndex = lastTabActive;
            }
        }

        private void SimConnectPanel1_OnGetLVarListRequested(object sender, EventArgs e)
        {
            if (_execManager.GetSimConnectCache().IsConnected())
            {
                _execManager.GetSimConnectCache().RefreshLVarsList();
            }
        }

        private void ConfigWizard_LVarListUpdated(object sender, EventArgs e)
        {
            simConnectPanel1.HubHopPresetPanel.LVars = (sender as List<String>);
        }



#if ARCAZE
        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        /// <param name="arcazeCache"></param>
        public void initWithArcazeCache(ArcazeCache arcazeCache)
        {
            List<ListItem> PreconditionModuleList = new List<ListItem>();
            List<ListItem> DisplayModuleList = new List<ListItem>();


            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                arcazeFirmware[module.Serial] = module.Version;
                DisplayModuleList.Add(new ListItem()
                {
                    Value = module.Name + "/ " + module.Serial,
                    Label = $"{module.Name} ({module.Serial})"
                });

                PreconditionModuleList.Add(new ListItem()
                {
                    Value = module.Name + "/ " + module.Serial,
                    Label = $"{module.Name} ({module.Serial})"
                });
            }

            _AddMobiFlightModules(DisplayModuleList);
            _AddJoysticks(DisplayModuleList);
            _AddMidiBoards(DisplayModuleList);

            displayPanel1.SetArcazeSettings(arcazeFirmware, moduleSettings);
            displayPanel1.SetModules(DisplayModuleList);
            preconditionPanel.SetModules(PreconditionModuleList);
        }
#endif
#if MOBIFLIGHT
        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        public void initWithoutArcazeCache()
        {
            var DisplayModuleList = new List<ListItem>();

            _AddMobiFlightModules(DisplayModuleList);
            _AddJoysticks(DisplayModuleList);
            _AddMidiBoards(DisplayModuleList);

            displayPanel1.SetModules(DisplayModuleList);
        }
#endif

        protected void _AddMobiFlightModules(List<ListItem> DisplayModuleList)
        {
            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().GetModuleInfo())
            {
                DisplayModuleList.Add(new ListItem()
                {
                    Value = module.Name + "/ " + module.Serial,
                    Label = $"{module.Name} ({module.Port})"
                });

                // Not yet supported for pins
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }
        }

        protected void _AddJoysticks(List<ListItem> DisplayModuleList)
        {
            foreach (Joystick joystick in _execManager.GetJoystickManager().GetJoysticks())
            {
                if (joystick.GetAvailableOutputDevicesAsListItems().Count == 0) continue;

                DisplayModuleList.Add(new ListItem()
                {
                    Value = $"{joystick.Name} {SerialNumber.SerialSeparator}{joystick.Serial}",
                    Label = $"{joystick.Name}"
                });

                // Not yet supported for pins
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }
        }

        protected void _AddMidiBoards(List<ListItem> DisplayModuleList)
        {
            foreach (MidiBoard midiBoard in _execManager.GetMidiBoardManager().GetMidiBoards())
            {
                if (midiBoard.GetAvailableOutputDevices().Count == 0) continue;

                DisplayModuleList.Add(new ListItem()
                {
                    Value = $"{midiBoard.Name} {SerialNumber.SerialSeparator}{midiBoard.Serial}",
                    Label = $"{midiBoard.Name}"
                });

                // Not yet supported for pins
                // preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }
        }

        /// <summary>
        /// sync the values from config with the config wizard form
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool _syncConfigToForm(OutputConfigItem config)
        {
            if (config == null) throw new Exception(i18n._tr("uiException_ConfigItemNotFound"));

            _syncFsuipcTabFromConfig(config);

            displayPanel1.syncFromConfig(config);

            modifierPanel1.fromConfig(config);

            preconditionPanel.syncFromConfig(config);

            testValuePanel1.FromConfig(config);

            return true;
        }

        private void _syncFsuipcTabFromConfig(OutputConfigItem config)
        {
            OffsetTypeFsuipRadioButton.Checked = (config.Source is FsuipcSource);
            OffsetTypeSimConnectRadioButton.Checked = (config.Source is SimConnectSource);
            OffsetTypeVariableRadioButton.Checked = (config.Source is VariableSource);
            OffsetTypeXplaneRadioButton.Checked = (config.Source is XplaneSource);

            fsuipcConfigPanel.syncFromConfig(config);

            simConnectPanel1.syncFromConfig(config);
            variablePanel1.syncFromConfig(config);
            configRefPanel.syncFromConfig(config);
            xplaneDataRefPanel1.syncFromConfig(config);
        }

        /// <summary>
        /// sync current status of form values to config
        /// </summary>
        /// <returns></returns>
        protected bool _syncFormToConfig()
        {
            if (OffsetTypeFsuipRadioButton.Checked)
            {
                config.Source = new FsuipcSource();
                fsuipcConfigPanel.syncToConfig(config);
            } else
            if (OffsetTypeSimConnectRadioButton.Checked)
            {
                config.Source = new SimConnectSource();
                simConnectPanel1.syncToConfig(config);
            } else
            if (OffsetTypeVariableRadioButton.Checked)
            {
                config.Source = new VariableSource();
                variablePanel1.syncToConfig(config);
            } else
            if (OffsetTypeXplaneRadioButton.Checked)
            {
                config.Source = new XplaneSource();
                xplaneDataRefPanel1.syncToConfig(config);
            }

            configRefPanel.syncToConfig(config);
            modifierPanel1.toConfig(config);
            displayPanel1.syncToConfig();
            preconditionPanel.syncToConfig(config);
            testValuePanel1.ToConfig(config);

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _testModeStop(true);
            try
            {
                if (!ValidateChildren())
                {
                    Log.Instance.log("The dialog cannot be closed. There are invalid values on some tab.", LogSeverity.Error);
                    return;
                }
            }
            catch (System.InvalidOperationException ex)
            {
                Log.Instance.log(ex.Message, LogSeverity.Error);
            }
            _syncFormToConfig();
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _testModeStop(true);
            DialogResult = DialogResult.Cancel;
        }

        private void ConfigWizard_Load(object sender, EventArgs e)
        {
            _syncConfigToForm(config);
        }

        private void _validatingHexFields(object sender, CancelEventArgs e, int length)
        {
            try
            {
                string tmp = (sender as TextBox).Text.Replace("0x", "").ToUpper();
                (sender as TextBox).Text = "0x" + Int64.Parse(tmp, System.Globalization.NumberStyles.HexNumber).ToString("X" + length.ToString());
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                Log.Instance.log($"Parsing problem: {ex.Message}", LogSeverity.Debug);
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

        private void tabControlFsuipc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if running in test mode
            lastTabActive = (sender as TabControl).SelectedIndex;
        }

        private void OffsetTypeFsuipRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            FsuipcSettingsPanel.Visible = (sender as RadioButton) == OffsetTypeFsuipRadioButton;
            simConnectPanel1.Visible = (sender as RadioButton) == OffsetTypeSimConnectRadioButton;
            variablePanel1.Visible = (sender as RadioButton) == OffsetTypeVariableRadioButton;
            xplaneDataRefPanel1.Visible = (sender as RadioButton) == OffsetTypeXplaneRadioButton;
        }

        private void ConfigWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _execManager.GetSimConnectCache().LVarListUpdated -= ConfigWizard_LVarListUpdated;
            // actively trigger dispose
            // to be able to clean up hubHopPreset Instances
            // to prevent memory leak.
            simConnectPanel1.Dispose();
        }

        private void _testModeStop(bool DialogClose = false)
        {
            try
            {
                TestTimer.Stop();
                _execManager.ExecuteTestOff(config, DialogClose);
            }
            catch (Exception e)
            {
                Log.Instance.log($"Error stopping test mode: {e.Message}", LogSeverity.Error);
            }

        }
    }
}
