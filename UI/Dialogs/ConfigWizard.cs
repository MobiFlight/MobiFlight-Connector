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
using MobiFlight.Properties;
using MobiFlight.UI.Forms;
using MobiFlight.UI.Panels.Config;
using MobiFlight.UI.Panels.OutputWizard;
using Newtonsoft.Json.Linq;

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
        DataSet _dataSetConfig = null;
        Timer TestTimer = new Timer();
        String CurrentGuid = null;
        public OutputConfigItem Config { get { return config; } }

#if ARCAZE
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        Dictionary<string, ArcazeModuleSettings> moduleSettings;
#endif

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
            var list = dataSetConfig.GetConfigsWithGuidAndLabel(filterGuid);

            // store the current guid
            CurrentGuid = filterGuid;

            preconditionPanel.SetAvailableConfigs(list);
            preconditionPanel.SetAvailableVariables(mainForm.GetAvailableVariables());
            initConfigRefDropDowns(dataSetConfig, filterGuid);   
        }

        private void initConfigRefDropDowns(DataSet dataSetConfig, string filterGuid)
        {
            _dataSetConfig = dataSetConfig;
            DataRow[] rows = dataSetConfig.Tables["config"].Select("guid <> '" + filterGuid + "'");

            // this filters the current config
            DataView dv = new DataView(dataSetConfig.Tables["config"]);
            dv.RowFilter = "guid <> '" + filterGuid + "'";

            configRefPanel.SetConfigRefsDataView(dv, filterGuid);
            displayPanel1.SetConfigRefsDataView(dv, filterGuid);
            //displayShiftRegisterPanel.SetConfigRefsDataView(dv, filterGuid);
        }

        public bool ConfigHasChanged()
        {
            return !originalConfig.Equals(config);
        }

        protected void Init(ExecutionManager executionManager, OutputConfigItem cfg)
        {
            this._execManager = executionManager;
            config = cfg.Clone() as OutputConfigItem;

            // Until with have the preconditions completely refactored,
            // add an empty precondition in case the current cfg doesn't have one
            // we removed addEmptyNode but add an empty Precondition here
            if (cfg.Preconditions.Count == 0) 
                cfg.Preconditions.Add(new Precondition());

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
            fsuipcConfigPanel.ModifierChanged += FsuipcConfigPanel_ModifierChanged;
            simConnectPanel1.ModifyTabLink += ConfigPanel_ModifyTabLink;
            xplaneDataRefPanel1.ModifyTabLink += ConfigPanel_ModifyTabLink;
            variablePanel1.ModifyTabLink += ConfigPanel_ModifyTabLink;

            testValuePanel1.FromConfig(config);
            testValuePanel1.TestModeStart += TestValuePanel_TestModeStart;
            testValuePanel1.TestModeStop += TestValuePanel_TestModeEnd;
            testValuePanel1.TestValueChanged += ModifierPanel1_ModifierChanged;
            TestTimer.Interval = Settings.Default.TestTimerInterval;
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
            var value = config.TestValue.Clone() as ConnectorValue;
            try
            {
                if (value != null)
                    config.Modifiers.Items.FindAll(x => x.Active).ForEach(y => value = y.Apply(value, new List<ConfigRefValue>()));
            } catch (Exception ex)
            {
                // ShowError? Or don't do anything?
            }
            
            testValuePanel1.Result = value.ToString();

            _execManager.ExecuteTestOn(config, CurrentGuid, value);
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
                TestTimer.Start();
            }
            catch (Exception e)
            {
                Log.Instance.log($"Error starting test mode: {e.Message}", LogSeverity.Error);
            }
        }

        private void FsuipcConfigPanel_ModifierChanged(object sender, EventArgs e)
        {
            modifierPanel1.UpdateModifier(fsuipcConfigPanel.Modifier);
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
            if (cfg?.DisplaySerial != null && cfg?.DisplaySerial != SerialNumber.NOT_SET)
            {
                tabControlFsuipc.SelectedIndex = lastTabActive;
            }
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
            simConnectPanel1.HubHopPresetPanel.LVars = (sender as List<String>);
        }

                

#if ARCAZE
        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        /// <param name="arcazeCache"></param>
        public void initWithArcazeCache (ArcazeCache arcazeCache)
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
            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().getModuleInfo())
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
                if (joystick.GetAvailableOutputDevices().Count == 0) continue;

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
            OffsetTypeFsuipRadioButton.Checked = (config.SourceType == SourceType.FSUIPC);
            OffsetTypeSimConnectRadioButton.Checked = (config.SourceType == SourceType.SIMCONNECT);
            OffsetTypeVariableRadioButton.Checked = (config.SourceType == SourceType.VARIABLE);
            OffsetTypeXplaneRadioButton.Checked = (config.SourceType == SourceType.XPLANE);

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
            config.SourceType = SourceType.FSUIPC;
            if (OffsetTypeSimConnectRadioButton.Checked) config.SourceType = SourceType.SIMCONNECT;
            if (OffsetTypeVariableRadioButton.Checked) config.SourceType = SourceType.VARIABLE;
            if (OffsetTypeXplaneRadioButton.Checked) config.SourceType = SourceType.XPLANE;

            if (config.SourceType == SourceType.FSUIPC)
                fsuipcConfigPanel.syncToConfig(config);
            else if (config.SourceType == SourceType.SIMCONNECT)
                simConnectPanel1.syncToConfig(config);
            else if (config.SourceType == SourceType.VARIABLE)
                variablePanel1.syncToConfig(config);
            else if (config.SourceType == SourceType.XPLANE)
                xplaneDataRefPanel1.syncToConfig(config);

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
            try {
                if (!ValidateChildren())
                {
                    return;
                }
            } catch (System.InvalidOperationException ex)
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
