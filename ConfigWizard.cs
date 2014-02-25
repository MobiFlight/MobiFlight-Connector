using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;

namespace ArcazeUSB
{
    public partial class ConfigWizard : Form
    {
        public event EventHandler PreconditionTreeNodeChanged;

        static int lastTabActive = 0;

        ExecutionManager _execManager = null;
        int displayPanelHeight = -1;
        List<Panel> displayPanels = new List<Panel>();
        ArcazeConfigItem config = null;
        ErrorProvider errorProvider = new ErrorProvider();
        Dictionary<String, ushort> arcazeFirmware = new Dictionary<String, ushort>();
        DataSet _dataSetConfig = null;

        Dictionary<string, ArcazeModuleSettings> moduleSettings;

        public ConfigWizard(ExecutionManager mainForm, 
                             ArcazeConfigItem cfg, 
                             ArcazeCache arcazeCache, 
                             Dictionary<string, ArcazeModuleSettings> moduleSettings, 
                             DataSet dataSetConfig, 
                             String filterGuid)
        {
            Init(mainForm, cfg);            
            initWithArcazeCache(arcazeCache);
            preparePreconditionPanel(dataSetConfig, filterGuid);
            this.moduleSettings = moduleSettings;
        }

        protected void Init(ExecutionManager mainForm, ArcazeConfigItem cfg)
        {
            this._execManager = mainForm;
            config = cfg;
            InitializeComponent();
            comparisonSettingsPanel.Enabled = false;
            
            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            if (cfg.FSUIPCOffset == ArcazeConfigItem.FSUIPCOffsetNull)
            {
                lastTabActive = 0;
            }
            tabControlFsuipc.SelectedIndex = lastTabActive;

            _initDisplayPanels();
            _initPreconditionPanel();
            _initFsuipcOffsetTypeComboBox();
            _loadPresets();
            fsuipcPresetComboBox.ResetText();
            // displayLedDisplayComboBox.Items.Clear(); 
        }

        private void _initPreconditionPanel()
        {
            preConditionTypeComboBox.Items.Clear();
            List<ListItem> preconTypes = new List<ListItem>() {
                new ListItem() { Value = "none",    Label = MainForm._tr("LabelPrecondition_None") },
                new ListItem() { Value = "config",  Label = MainForm._tr("LabelPrecondition_ConfigItem") },
                new ListItem() { Value = "pin",     Label = MainForm._tr("LabelPrecondition_ArcazePin") }
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

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(Properties.Settings.Default.PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(MainForm._tr("uiMessageConfigWizard_PresetsNotFound"), MainForm._tr("Hint"));             
            }
            else
            {

                try
                {
                    presetsDataSet.Clear();
                    presetsDataSet.ReadXml(Properties.Settings.Default.PresetFile);
                    DataRow[] rows = presetDataTable.Select("", "description");
                    fsuipcPresetComboBox.Items.Clear();

                    foreach (DataRow row in rows)
                    {
                       fsuipcPresetComboBox.Items.Add(row["description"]);
                    }
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(MainForm._tr("uiMessageConfigWizard_ErrorLoadingPresets"), MainForm._tr("Hint"));                    
                }
            }

            fsuipcPresetComboBox.Enabled = isLoaded;
            fsuipcPresetUseButton.Enabled = isLoaded;
        }

        protected void _initDisplayPanels () {
            // make all panels small and store the common height
            displayPanels.Clear();
            foreach (Control ctrl in groupBoxDisplaySettings.Controls)
            {
                if (ctrl.GetType() != typeof(Panel)) continue;
                Panel p = ctrl as Panel;
                if (p.Height > 0 && (p.Height > displayPanelHeight)) displayPanelHeight = p.Height;
                
                p.Height = 0;
                displayPanels.Add(p);
            } //foreach
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
        }

        /// <summary>
        /// sync the config wizard with the provided settings from arcaze cache such as available modules, ports, etc.
        /// </summary>
        /// <param name="arcazeCache"></param>
        public void initWithArcazeCache (ArcazeCache arcazeCache)
        {
            displayPortComboBox.Items.Clear();
            displayPinComboBox.Items.Clear();
            displayBcdStrobePortComboBox.Items.Clear();
            displayBcdPortComboBox.Items.Clear();
            displayBcdStrobePinComboBox.Items.Clear();
            displayBcdPin1ComboBox.Items.Clear();
            displayBcdPin2ComboBox.Items.Clear();
            displayBcdPin3ComboBox.Items.Clear();
            displayBcdPin4ComboBox.Items.Clear();
            preconditionPortComboBox.Items.Clear();
            preconditionPinComboBox.Items.Clear();

            foreach (String v in ArcazeModule.getPorts()) { 
                displayPortComboBox.Items.Add(v);
                displayBcdStrobePortComboBox.Items.Add(v);
                displayBcdPortComboBox.Items.Add(v);

                if (v == "B" || v == "E" || v == "H" || v == "K")
                {                    
                    displayPortComboBox.Items.Add("-----");
                    displayBcdStrobePortComboBox.Items.Add("-----");
                    displayBcdPortComboBox.Items.Add("-----");
                }

                if (v == "A" || v == "B")
                {
                    preconditionPortComboBox.Items.Add(v);
                }
            }

            foreach (String v in ArcazeModule.getPins())
            { 
                displayPinComboBox.Items.Add(v);
                displayBcdStrobePinComboBox.Items.Add(v);
                displayBcdPin1ComboBox.Items.Add(v);
                displayBcdPin2ComboBox.Items.Add(v);
                displayBcdPin3ComboBox.Items.Add(v);
                displayBcdPin4ComboBox.Items.Add(v);
                preconditionPinComboBox.Items.Add(v);
            }

            displayModuleNameComboBox.Items.Clear();
            preconditionPinSerialComboBox.Items.Clear();
            displayModuleNameComboBox.Items.Add("-");
            preconditionPinSerialComboBox.Items.Add("-");

            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                arcazeFirmware[module.Name + "/ " + module.Serial] = module.Version;
                displayModuleNameComboBox.Items.Add(module.Name + "/ " + module.Serial);
                preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }
#if MOBIFLIGHT
            foreach (IModuleInfo module in _execManager.getMobiFlightModuleCache().getModuleInfo())
            {
                displayModuleNameComboBox.Items.Add(module.Name + "/ " + module.Serial);
                preconditionPinSerialComboBox.Items.Add(module.Name + "/ " + module.Serial);
            }
#endif
            displayModuleNameComboBox.SelectedIndex = 0;
            preconditionPinSerialComboBox.SelectedIndex = 0;            
        }

        /// <summary>
        /// sync the values from config with the config wizard form
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected bool _syncConfigToForm(ArcazeConfigItem config)
        {
            if (config == null) throw new Exception(MainForm._tr("uiException_ConfigItemNotFound"));
            // first tab                        
            fsuipcOffsetTextBox.Text = "0x" + config.FSUIPCOffset.ToString("X4");
            
            // preselect fsuipc offset type
            try
            {
                fsuipcOffsetTypeComboBox.SelectedValue = config.FSUIPCOffsetType.ToString();
            }
            catch (Exception exc)
            {
                // TODO: provide error message
            }

            if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, config.FSUIPCSize.ToString()))
            {
                // TODO: provide error message
            }

            // mask
            fsuipcMaskTextBox.Text = "0x" + config.FSUIPCMask.ToString("X"+config.FSUIPCSize);

            // multiplier
            fsuipcMultiplyTextBox.Text = config.FSUIPCMultiplier.ToString();
            fsuipcBcdModeCheckBox.Checked = config.FSUIPCBcdMode;
            
            // second tab
            comparisonActiveCheckBox.Checked = config.ComparisonActive;
            comparisonValueTextBox.Text = config.ComparisonValue;

            if (!ComboBoxHelper.SetSelectedItem(comparisonOperandComboBox, config.ComparisonOperand))
            {
                // TODO: provide error message
            }
            comparisonIfValueTextBox.Text = config.ComparisonIfValue;
            comparisonElseValueTextBox.Text = config.ComparisonElseValue;         

            // third tab
            if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
            {
                // TODO: provide error message
            }

            if (config.DisplaySerial != null && config.DisplaySerial != "")
            {
                string serial = config.DisplaySerial;
                if (serial.Contains('/'))
                {
                    serial = serial.Split('/')[1].Trim();
                }
                if (!ComboBoxHelper.SetSelectedItemByPart(displayModuleNameComboBox, serial))
                {
                    // TODO: provide error message
                }
            }            

            if (config.DisplayPin != null && config.DisplayPin != "")
            {
                string port = config.DisplayPin.Substring(0, 1);
                string pin = config.DisplayPin.Substring(1);
            
                // preselect normal pin drop downs
                if (!ComboBoxHelper.SetSelectedItem(displayPortComboBox, port)) { /* TODO: provide error message */ }
                if (!ComboBoxHelper.SetSelectedItem(displayPinComboBox, pin)) { /* TODO: provide error message */ }

                int range = displayPinBrightnessTrackBar.Maximum - displayPinBrightnessTrackBar.Minimum;
                displayPinBrightnessTrackBar.Value = (int)((config.DisplayPinBrightness / (double)255) * (range)) + displayPinBrightnessTrackBar.Minimum;
            }

            // preselect BCD4056
            for (int i = 0; i < config.BcdPins.Count(); i++)
            {
                string tmpPort = config.BcdPins[i].Substring(0, 1);
                string tmpPin = config.BcdPins[i].Substring(1);

                if (i == 0)
                {
                    if (!ComboBoxHelper.SetSelectedItem(displayBcdStrobePortComboBox, tmpPort)) { /* TODO: provide error message */ }
                    if (!ComboBoxHelper.SetSelectedItem(displayBcdStrobePinComboBox, tmpPin)) { /* TODO: provide error message */ }
                }
                else
                {
                    if (!ComboBoxHelper.SetSelectedItem(displayBcdPortComboBox, tmpPort)) { /* TODO: provide error message */ }
                    if (!ComboBoxHelper.SetSelectedItem(displayBcdPanel.Controls["displayBcdPin" + i + "ComboBox"] as ComboBox, tmpPin)) { /* TODO: provide error message */ }                    
                }
            }
            
            // preselect display stuff
			if (!ComboBoxHelper.SetSelectedItem(displayLedAddressComboBox, config.DisplayLedAddress.ToString()))
            {
                // TODO: provide error message
            }

            if (!ComboBoxHelper.SetSelectedItem(displayLedConnectorComboBox, config.DisplayLedConnector.ToString()))
            {
                // TODO: provide error message
            }

            if (!ComboBoxHelper.SetSelectedItem(displayLedModuleSizeComboBox, config.DisplayLedModuleSize.ToString()))
            {
                // TODO: provide error message
            }
            
			displayLedPaddingCheckBox.Checked = config.DisplayLedPadding;
            foreach (string digit in config.DisplayLedDigits)
            {
                (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + digit + "Checkbox"] as CheckBox).Checked = true;
            }

            foreach (string digit in config.DisplayLedDecimalPoints)
            {
                (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + digit + "Checkbox"] as CheckBox).Checked = true;
            }

            preconditionListTreeView.Nodes.Clear();
            foreach (Precondition p in config.Preconditions)
            {
                TreeNode tmpNode = new TreeNode();
                tmpNode.Text = p.ToString();
                tmpNode.Tag = p;
                tmpNode.Checked = p.PreconditionActive;
                _updateNodeWithPrecondition(tmpNode, p);
                preconditionListTreeView.Nodes.Add(tmpNode);   
            }

            if (preconditionListTreeView.Nodes.Count == 0)
            {
                _addEmptyNodeToTreeView();
            }

            return true;
        }

        private void _addEmptyNodeToTreeView()
        {
            TreeNode tmpNode = new TreeNode();
            Precondition p = new Precondition();

            tmpNode.Text = p.ToString();
            tmpNode.Tag = p;
            tmpNode.Checked = p.PreconditionActive;            
            _updateNodeWithPrecondition(tmpNode, p);
            config.Preconditions.Add(p);
            preconditionListTreeView.Nodes.Add(tmpNode);
        }

        /*
        protected bool _setSelectedItem (ComboBox comboBox, string value) {
            if (comboBox.FindStringExact(value) != -1)
            {
                comboBox.SelectedIndex = comboBox.FindStringExact(value);
                return true;
            }
            return false;
        }        

        protected bool _setSelectedItemByPart (ComboBox comboBox, string value)
        {
            foreach (string item in comboBox.Items)
            {
                if (item.Contains(value))
                {
                    comboBox.SelectedIndex = comboBox.FindStringExact(item);
                    return true;
                }
            }

            return false;
        }
         * */

        /// <summary>
        /// sync current status of form values to config
        /// </summary>
        /// <returns></returns>
        protected bool _syncFormToConfig()
        {
            // fsuipc panel
            config.FSUIPCMask       = Int64.Parse(fsuipcMaskTextBox.Text.Replace("0x","").ToLower(),System.Globalization.NumberStyles.HexNumber);
            config.FSUIPCOffset     = Int32.Parse(fsuipcOffsetTextBox.Text.Replace("0x", "").ToLower(), System.Globalization.NumberStyles.HexNumber);            
            config.FSUIPCSize       = Byte.Parse(fsuipcSizeComboBox.Text);
            config.FSUIPCOffsetType = (FSUIPCOffsetType) Enum.Parse(typeof(FSUIPCOffsetType), ((ListItem)(fsuipcOffsetTypeComboBox.SelectedItem)).Value);
            config.FSUIPCMultiplier = Double.Parse(fsuipcMultiplyTextBox.Text);
            config.FSUIPCBcdMode    = fsuipcBcdModeCheckBox.Checked;

            // comparison panel
            config.ComparisonActive = comparisonActiveCheckBox.Checked;
            config.ComparisonValue = comparisonValueTextBox.Text;
            config.ComparisonOperand = comparisonOperandComboBox.Text;
            config.ComparisonIfValue = comparisonIfValueTextBox.Text;
            config.ComparisonElseValue = comparisonElseValueTextBox.Text;

            // display panel
            config.DisplayType = displayTypeComboBox.Text;
            config.DisplayTrigger = "normal";
            config.DisplaySerial = displayModuleNameComboBox.Text;
            config.DisplayPin = displayPortComboBox.Text + displayPinComboBox.Text;
            config.DisplayPinBrightness = (byte)(255 * ((displayPinBrightnessTrackBar.Value) / (double)(displayPinBrightnessTrackBar.Maximum)));

            config.DisplayLedAddress = displayLedAddressComboBox.Text;
            config.DisplayLedConnector = byte.Parse(displayLedConnectorComboBox.Text);
            config.DisplayLedPadding = displayLedPaddingCheckBox.Checked;
            config.DisplayLedModuleSize = byte.Parse(displayLedModuleSizeComboBox.Text);
            config.DisplayLedDigits.Clear();
            config.DisplayLedDecimalPoints.Clear();
            for (int i = 0; i < 8; i++)
            {
                CheckBox cb = (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.DisplayLedDigits.Add(i.ToString());
                } //if

                cb = (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.DisplayLedDecimalPoints.Add(i.ToString());
                } //if
            }

            config.BcdPins.Clear();
            config.BcdPins.Add(displayBcdStrobePortComboBox.Text + displayBcdStrobePinComboBox.Text);
            for (int i = 1; i <= 4; i++ )
            {
                config.BcdPins.Add(
                    displayBcdStrobePortComboBox.Text +
                    (displayBcdPanel.Controls["displayBcdPin" + i + "ComboBox"] as ComboBox).Text);
            }
            
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _testModeStop();
            if (!ValidateChildren())
            {              
                return;
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

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Panel p in displayPanels)
            {
                p.Enabled = false;
                p.Height = 0;
            } //foreach

            try
            {
                bool panelEnabled = true;
                // get the deviceinfo for the current arcaze
                string serial = displayModuleNameComboBox.SelectedItem.ToString();
                if (arcazeFirmware.ContainsKey(serial))
                {

                    switch ((sender as ComboBox).SelectedItem.ToString())
                    {
                        case "DisplayDriver":
                            panelEnabled = arcazeFirmware[serial] > 0x529;
                            break;

                        case "LedDriver2":
                            panelEnabled = arcazeFirmware[serial] > 0x554;
                            break;

                        case "LedDriver3":
                            panelEnabled = arcazeFirmware[serial] > 0x550;
                            break;
                    }
                }

                displayPanels[(sender as ComboBox).SelectedIndex].Enabled = panelEnabled;
                displayPanels[(sender as ComboBox).SelectedIndex].Height = displayPanelHeight;
            }
            catch (Exception)
            {
                MessageBox.Show(MainForm._tr("uiMessageNotImplementedYet"), 
                                MainForm._tr("Hint"), 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
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
            fsuipcOffsetTypeComboBox.SelectedIndexChanged += fsuipcOffsetTypeComboBox_SelectedIndexChanged;
        }

        private void fsuipcPresetUseButton_Click(object sender, EventArgs e)
        {
            if (fsuipcPresetComboBox.Text != "")
            {
                DataRow[] rows = presetDataTable.Select("description = '" + fsuipcPresetComboBox.Text+"'");
                if (rows.Length > 0)
                {
                    _syncConfigToForm(rows[0]["settings"] as ArcazeConfigItem);
                }
            }
        }

        private void _usePresetConfig(ArcazeConfigItem cfg)
        {
            
        }

        private void fsuipcSizeComboBox_TextChanged(object sender, EventArgs e)
        {
            // we always set the mask according to the set bytes
            fsuipcMaskTextBox.Text = "0x" + (
                                        new String ('F', 
                                                    UInt16.Parse((sender as ComboBox).Text)* 2
                                                   ));
        }

        private void fsuipcMultiplyTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                float.Parse((sender as TextBox).Text);
            }
            catch (Exception exc)
            {
                e.Cancel = true;
            }
        }

        private void fsuipcMaskTextBox_Validating(object sender, CancelEventArgs e)
        {
            _validatingHexFields(sender, e, int.Parse(fsuipcSizeComboBox.Text)*2);
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
                MessageBox.Show(MainForm._tr("uiMessageConfigWizard_ValidHexFormat"), MainForm._tr("Hint"));
            }
        }

        private void displayError(Control control, String message)
        {
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, MainForm._tr("Hint"));
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
                displayError(displayArcazeSerialComboBox, MainForm._tr("uiMessageConfigWizard_SelectArcaze"));                
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
                displayError(cb, MainForm._tr("Please_select_a_port"));
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
                    int.Parse(displayLedAddressComboBox.Text);
                    removeError(displayLedAddressComboBox);
                }
                catch (Exception exc)
                {
                    e.Cancel = true;
                    tabControlFsuipc.SelectedTab = displayTabPage;
                    displayLedAddressComboBox.Focus();
                    displayError(displayLedAddressComboBox, MainForm._tr("uiMessageConfigWizard_ProvideLedDisplayAddress"));
                }                
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value = Int16.Parse((sender as ComboBox).Text);
            for (int i = 0; i < 8; i++)
            {
                displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"].Visible = i < value;
                displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"].Visible = i < value;
                displayLedDisplayPanel.Controls["displayLedDisplayLabel" + i].Visible = i < value;

                // uncheck all invisible checkboxes to ensure correct mask
                if (i >= value)
                {
                    (displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "CheckBox"] as CheckBox).Checked = false;
                    (displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "CheckBox"] as CheckBox).Checked = false;
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
                displayError(preconditionRefValueTextBox, MainForm._tr("uiMessageConfigWizard_SelectComparison"));
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

            if (preconditionPinSerialComboBox.Text.Trim() == "-")
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                preconditionPinSerialComboBox.Focus();
                displayError(preconditionPinSerialComboBox, MainForm._tr("uiMessageConfigWizard_SelectArcaze"));
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

            if (preconditionPinComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionPinComboBox, MainForm._tr("Please_select_a_pin."));
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

            if (preconditionPortComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                tabControlFsuipc.SelectedTab = preconditionTabPage;
                displayError(preconditionPortComboBox, MainForm._tr("Please_select_a_port"));
            }
            else
            {
                removeError(preconditionPortComboBox);
            }
        }

        private void displayArcazeSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check which extension type is available to current serial
            ComboBox cb = (sender as ComboBox);

            try {
                // disable test button
                // in case that no display is selected                
                String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());

                displayTypeComboBox.Enabled = groupBoxDisplaySettings.Enabled = testSettingsGroupBox.Enabled = (serial != "");
                
                
                displayPinBrightnessPanel.Visible = (moduleSettings[serial].type == SimpleSolutions.Usb.ArcazeCommand.ExtModuleType.LedDriver3);
                displayPinBrightnessPanel.Enabled = (displayPinBrightnessPanel.Visible && (cb.SelectedIndex > 1)); 
            }
            catch(Exception ex) {
                displayPinBrightnessPanel.Visible = false;
                displayPinBrightnessPanel.Enabled = false; 
            }
        }

        private void displayPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            // enable setting only for ports higher than B
            displayPinBrightnessPanel.Enabled = cb.SelectedIndex > 1;
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
            _execManager.executeTestOn(config);
        }

        private void _testModeStop()
        {
            // check if running in test mode otherwise simply return
            if (!displayPinTestStopButton.Enabled) return;

            displayPinTestStopButton.Enabled = false;
            displayPinTestButton.Enabled = true;
            displayTypeGroupBox.Enabled = true;
            groupBoxDisplaySettings.Enabled = true;
            _execManager.executeTestOff(config);
        }

        private void tabControlFsuipc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if running in test mode
            lastTabActive = (sender as TabControl).SelectedIndex;
            _testModeStop();
        }

        private void maskEditorButton_Click(object sender, EventArgs e)
        {
            BitMaskEditorForm bme = new BitMaskEditorForm(
                                        Byte.Parse(fsuipcSizeComboBox.Text), 
                                        UInt64.Parse(fsuipcMaskTextBox.Text.Replace("0x","").ToLower(),
                                                     System.Globalization.NumberStyles.HexNumber));
            bme.StartPosition = FormStartPosition.CenterParent;
            if (bme.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fsuipcMaskTextBox.Text = "0x" + bme.Result.ToString("X" + (Byte.Parse(fsuipcSizeComboBox.Text)*2));
            }
        }

        private void fsuipcOffsetTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateByteSizeComboBox();
        }

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

        private void _initFsuipcOffsetTypeComboBox()
        {
            List<ListItem> offsetTypes = new List<ListItem>() {
                new ListItem() { Value = FSUIPCOffsetType.Integer.ToString(),       Label = "Int" },
                /*new ListItem() { Value = FSUIPCOffsetType.UnsignedInt.ToString(),   Label = "UInt" },*/
                new ListItem() { Value = FSUIPCOffsetType.Float.ToString(),         Label = "Float" },
                new ListItem() { Value = FSUIPCOffsetType.String.ToString(),        Label = "String" }
            };

            fsuipcOffsetTypeComboBox.DataSource = offsetTypes;
            fsuipcOffsetTypeComboBox.DisplayMember = "Label";
            fsuipcOffsetTypeComboBox.ValueMember = "Value";
            fsuipcOffsetTypeComboBox.SelectedIndex = 0;
        }

        private void preconditionListTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            preconditionListTreeView.SelectedNode = e.Node;
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

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
                    }

                    ComboBoxHelper.SetSelectedItem(preconditionRefOperandComboBox, config.PreconditionOperand);
                    preconditionRefValueTextBox.Text = config.PreconditionValue;
                    break;

                case "pin":
                    ArcazeIoBasic io = new ArcazeIoBasic(config.PreconditionPin);
                    preconditionPortComboBox.SelectedIndex = io.Port;
                    preconditionPinComboBox.SelectedIndex = io.Pin;
                    ComboBoxHelper.SetSelectedItemByPart(preconditionPinSerialComboBox, config.PreconditionSerial);
                    preconditionPinValueComboBox.SelectedValue = config.PreconditionValue;
                    break;
            }  
        }

        private void preconditionApplyButton_Click(object sender, EventArgs e)
        {
            // sync the selected node with the current settings from the panels
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
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
    }
}
