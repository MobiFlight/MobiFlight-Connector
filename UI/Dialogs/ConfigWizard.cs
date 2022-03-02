﻿using System;
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
using MobiFlight.UI.Panels.Config;
using MobiFlight.UI.Panels.OutputWizard;

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
            preconditionPanel.preparePreconditionPanel(dataSetConfig, filterGuid);
            initConfigRefDropDowns(dataSetConfig, filterGuid);   
            
            displayPanel1.TestModeStartRequested += (sender, args) => { _testModeStart();  };
            displayPanel1.TestModeStopRequested += (sender, args) => { _testModeStop(); };
            displayPanel1.DisplayPanelValidatingError += (sender, args) =>
            {
                tabControlFsuipc.SelectedTab = displayTabPage;
            };
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

            // DISPLAY PANEL
            displayPanel1.Init(_execManager);

            // PRECONDITION PANEL
            preconditionPanel.Init();
            preconditionPanel.ErrorOnValidating += (sender, e) =>
            {
                tabControlFsuipc.SelectedTab = preconditionTabPage;
            };

            variablePanel1.SetVariableReferences(_execManager.GetAvailableVariables());

            // FSUIPC PANEL
            fsuipcConfigPanel.setMode(true);
            fsuipcConfigPanel.syncFromConfig(cfg);

            // SIMCONNECT SIMVARS PANEL
            simConnectPanel1.HubHopPresetPanel.OnGetLVarListRequested += SimConnectPanel1_OnGetLVarListRequested;
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
            List<ListItem> DisplayModuleList = new List<ListItem>();
            
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

            displayPanel1.SetModules(DisplayModuleList);
        }
#endif
        /// <summary>
        /// sync the values from config with the config wizard form
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool _syncConfigToForm(OutputConfigItem config)
        {
            if (config == null) throw new Exception(i18n._tr("uiException_ConfigItemNotFound"));

            _syncFsuipcTabFromConfig(config);

            _syncComparisonTabFromConfig(config);
            
            displayPanel1.syncFromConfig(config);

            preconditionPanel.syncFromConfig(config);
            
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
            displayPanel1.syncToConfig();
            preconditionPanel.syncToConfig(config);

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

        private void tabControlFsuipc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if running in test mode
            lastTabActive = (sender as TabControl).SelectedIndex;
            _testModeStop();
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

        private void _testModeStart()
        {
            _syncFormToConfig();

            try
            {
                _execManager.ExecuteTestOn(config);
            }
            catch (Exception e)
            {
                Log.Instance.log($"Error Test Mode execution. ExecuteTestOn > {e.Message}", LogSeverity.Error);
            }
        }

        private void _testModeStop()
        {
            try
            {
                _execManager.ExecuteTestOff(config);
            }
            catch (Exception e)
            {
                Log.Instance.log($"Error Test Mode execution. ExecuteTestOff > {e.Message}", LogSeverity.Error);
            }

        }
    }
}
