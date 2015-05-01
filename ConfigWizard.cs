using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;

namespace MobiFlight
{
    public partial class ConfigWizard : Form
    {
        public event EventHandler PreconditionTreeNodeChanged;

        static int lastTabActive = 0;

        ExecutionManager _execManager = null;
        int displayPanelHeight = -1;
        List<UserControl> displayPanels = new List<UserControl>();
        OutputConfigItem config = null;
        ErrorProvider errorProvider = new ErrorProvider();
        Dictionary<String, String> arcazeFirmware = new Dictionary<String, String>();
        DataSet _dataSetConfig = null;

        Dictionary<string, ArcazeModuleSettings> moduleSettings;

        Panels.DisplayPinPanel displayPinPanel = new Panels.DisplayPinPanel();
        Panels.DisplayBcdPanel displayBcdPanel = new Panels.DisplayBcdPanel();
        Panels.DisplayLedDisplayPanel displayLedDisplayPanel = new Panels.DisplayLedDisplayPanel();
        Panels.DisplayNothingSelectedPanel displayNothingSelectedPanel = new Panels.DisplayNothingSelectedPanel();
        Panels.ServoPanel servoPanel = new Panels.ServoPanel();

        public ConfigWizard(ExecutionManager mainForm, 
                             OutputConfigItem cfg, 
                             ArcazeCache arcazeCache, 
                             Dictionary<string, ArcazeModuleSettings> moduleSettings, 
                             DataSet dataSetConfig, 
                             String filterGuid)
        {
            this.moduleSettings = moduleSettings;
            Init(mainForm, cfg);            
            initWithArcazeCache(arcazeCache);
            preparePreconditionPanel(dataSetConfig, filterGuid);            
        }

        protected void Init(ExecutionManager mainForm, OutputConfigItem cfg)
        {
            this._execManager = mainForm;
            config = cfg;
            InitializeComponent();
            comparisonSettingsPanel.Enabled = false;
            
            // if one opens the dialog for a new config
            // ensure that always the first tab is shown
            if (cfg.FSUIPCOffset == OutputConfigItem.FSUIPCOffsetNull)
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

            displayPanels.Clear();
            displayPanelHeight = 0;
            displayPanels.Add(displayPinPanel);
            displayPanels.Add(displayBcdPanel);
            displayPanels.Add(displayLedDisplayPanel);
            displayPanels.Add(displayNothingSelectedPanel);
            displayPanels.Add(servoPanel);

            foreach (UserControl p in displayPanels)
            {
                if (p.Height > 0 && (p.Height > displayPanelHeight)) displayPanelHeight = p.Height;                
                p.Height = 0;
            } //foreach

            displayNothingSelectedPanel.Height = displayPanelHeight;
            displayNothingSelectedPanel.Enabled = true;
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
        protected bool _syncConfigToForm(OutputConfigItem config)
        {
            string serial = null;
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
                Log.Instance.log("_syncConfigToForm : Exception on FSUIPCOffsetType.ToString", LogSeverity.Debug);
            }

            if (!ComboBoxHelper.SetSelectedItem(fsuipcSizeComboBox, config.FSUIPCSize.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in ComboBox", LogSeverity.Debug);
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
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Comparison ComboBox", LogSeverity.Debug);
            }
            comparisonIfValueTextBox.Text = config.ComparisonIfValue;
            comparisonElseValueTextBox.Text = config.ComparisonElseValue;
            
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

            // third tab
            if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Display Type ComboBox", LogSeverity.Debug);
            }
                    
            if (config.DisplayPin != null && config.DisplayPin != "")
            {
                string port = "";
                string pin = config.DisplayPin;

                if (serial != null && serial.IndexOf("SN") != 0)
                {
                    port = config.DisplayPin.Substring(0, 1);
                    pin = config.DisplayPin.Substring(1);
                }
            
                // preselect normal pin drop downs
                if (!ComboBoxHelper.SetSelectedItem(displayPinPanel.displayPortComboBox, port)) { /* TODO: provide error message */ }
                if (!ComboBoxHelper.SetSelectedItem(displayPinPanel.displayPinComboBox, pin)) { /* TODO: provide error message */ }

                int range = displayPinPanel.displayPinBrightnessTrackBar.Maximum - displayPinPanel.displayPinBrightnessTrackBar.Minimum;
                displayPinPanel.displayPinBrightnessTrackBar.Value = (int)((config.DisplayPinBrightness / (double)255) * (range)) + displayPinPanel.displayPinBrightnessTrackBar.Minimum;
            }

            // preselect BCD4056
            for (int i = 0; i < config.BcdPins.Count(); i++)
            {
                if (config.BcdPins[i] != "")
                {
                    string tmpPort = config.BcdPins[i].Substring(0, 1);
                    string tmpPin = config.BcdPins[i].Substring(1);

                    if (i == 0)
                    {
                        if (!ComboBoxHelper.SetSelectedItem(displayBcdPanel.displayBcdStrobePortComboBox, tmpPort)) { /* TODO: provide error message */ }
                        if (!ComboBoxHelper.SetSelectedItem(displayBcdPanel.displayBcdStrobePinComboBox, tmpPin)) { /* TODO: provide error message */ }
                    }
                    else
                    {
                        if (!ComboBoxHelper.SetSelectedItem(displayBcdPanel.displayBcdPortComboBox, tmpPort)) { /* TODO: provide error message */ }
                        if (!ComboBoxHelper.SetSelectedItem(displayBcdPanel.Controls["displayBcdPin" + i + "ComboBox"] as ComboBox, tmpPin)) { /* TODO: provide error message */ }
                    }
                }
            }
            
            // preselect display stuff
			if (!ComboBoxHelper.SetSelectedItem(displayLedDisplayPanel.displayLedAddressComboBox, config.DisplayLedAddress.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Led Address ComboBox", LogSeverity.Debug);
            }

            if (!ComboBoxHelper.SetSelectedItem(displayLedDisplayPanel.displayLedConnectorComboBox, config.DisplayLedConnector.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Led Connector ComboBox", LogSeverity.Debug);
            }

            if (!ComboBoxHelper.SetSelectedItem(displayLedDisplayPanel.displayLedModuleSizeComboBox, config.DisplayLedModuleSize.ToString()))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Led Module Size ComboBox", LogSeverity.Debug);
            }

            displayLedDisplayPanel.displayLedPaddingCheckBox.Checked = config.DisplayLedPadding;
            displayLedDisplayPanel.SetPaddingChar(config.DisplayLedPaddingChar);

            foreach (string digit in config.DisplayLedDigits)
            {
                (displayLedDisplayPanel.displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + digit + "Checkbox"] as CheckBox).Checked = true;
            }

            foreach (string digit in config.DisplayLedDecimalPoints)
            {
                (displayLedDisplayPanel.displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + digit + "Checkbox"] as CheckBox).Checked = true;
            }


            if (!ComboBoxHelper.SetSelectedItem(servoPanel.servoAddressesComboBox, config.ServoAddress))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Servo Address ComboBox", LogSeverity.Debug);
            }
            
            if (config.ServoMin != null) servoPanel.minValueTextBox.Text = config.ServoMin;
            if (config.ServoMax != null) servoPanel.maxValueTextBox.Text = config.ServoMax;
            if (config.ServoMaxRotationPercent!= null) servoPanel.maxRotationPercentNumericUpDown.Text = config.ServoMaxRotationPercent;

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
            config.DisplayPin = displayPinPanel.displayPortComboBox.Text + displayPinPanel.displayPinComboBox.Text;
            config.DisplayPinBrightness = (byte)(255 * ((displayPinPanel.displayPinBrightnessTrackBar.Value) / (double)(displayPinPanel.displayPinBrightnessTrackBar.Maximum)));

            config.DisplayLedAddress = displayLedDisplayPanel.displayLedAddressComboBox.Text;
            config.DisplayLedPadding = displayLedDisplayPanel.displayLedPaddingCheckBox.Checked;
            config.DisplayLedPaddingChar = displayLedDisplayPanel.GetPaddingChar();
            try
            {
                config.DisplayLedConnector = byte.Parse(displayLedDisplayPanel.displayLedConnectorComboBox.Text);
                config.DisplayLedModuleSize = byte.Parse(displayLedDisplayPanel.displayLedModuleSizeComboBox.Text);
            }
            catch (FormatException e)
            {
                Log.Instance.log("_syncFormToConfig : Parsing values", LogSeverity.Debug);
            }
            config.DisplayLedDigits.Clear();
            config.DisplayLedDecimalPoints.Clear();
            for (int i = 0; i < 8; i++)
            {
                CheckBox cb = (displayLedDisplayPanel.displayLedDigitFlowLayoutPanel.Controls["displayLedDigit" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.DisplayLedDigits.Add(i.ToString());
                } //if

                cb = (displayLedDisplayPanel.displayLedDecimalPointFlowLayoutPanel.Controls["displayLedDecimalPoint" + i + "Checkbox"] as CheckBox);
                if (cb.Checked)
                {
                    config.DisplayLedDecimalPoints.Add(i.ToString());
                } //if
            }

            config.BcdPins.Clear();
            config.BcdPins.Add(displayBcdPanel.displayBcdStrobePortComboBox.Text + displayBcdPanel.displayBcdStrobePinComboBox.Text);
            for (int i = 1; i <= 4; i++ )
            {
                config.BcdPins.Add(
                    displayBcdPanel.displayBcdStrobePortComboBox.Text +
                    (displayBcdPanel.Controls["displayBcdPin" + i + "ComboBox"] as ComboBox).Text);
            }

            config.ServoAddress = servoPanel.servoAddressesComboBox.Text;
            config.ServoMin = servoPanel.minValueTextBox.Text;
            config.ServoMax = servoPanel.maxValueTextBox.Text;
            config.ServoMaxRotationPercent = servoPanel.maxRotationPercentNumericUpDown.Text;
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

        private void displayArcazeSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check which extension type is available to current serial
            ComboBox cb = (sender as ComboBox);

            try
            {
                // disable test button
                // in case that no display is selected                
                String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());

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
                    displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                }
                else
                {
                    displayTypeComboBox.Items.Clear();
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);
                    foreach (DeviceType devType in module.GetConnectedOutputDeviceTypes())
                    {
                        switch (devType)
                        {
                            case DeviceType.LedModule:
                                displayTypeComboBox.Items.Add(ArcazeLedDigit.TYPE);
                                break;

                            case DeviceType.Output:
                                displayTypeComboBox.Items.Add("Pin");
                                displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                                break;

                            case DeviceType.Servo:
                                displayTypeComboBox.Items.Add(DeviceType.Servo.ToString("F"));
                                break;

                            case DeviceType.Stepper:
                                displayTypeComboBox.Items.Add(DeviceType.Stepper.ToString("F"));
                                break;
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
            foreach (UserControl p in displayPanels)
            {
                p.Enabled = false;
                p.Height = 0;
            } //foreach

            try
            {
                bool panelEnabled = true;
                // get the deviceinfo for the current arcaze
                ComboBox cb = displayModuleNameComboBox;                
                String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());

                // we remove the callback method to ensure, that it is not added more than once
                displayLedDisplayPanel.displayLedAddressComboBox.SelectedIndexChanged -= displayLedAddressComboBox_SelectedIndexChanged;

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
                else if (serial.IndexOf("SN") == 0)
                {
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);

                    displayPinPanel.displayPinBrightnessPanel.Visible = true;
                    displayPinPanel.displayPinBrightnessPanel.Enabled = (displayPinPanel.displayPinBrightnessPanel.Visible && (cb.SelectedIndex > 1));

                    List<ListItem> outputs = new List<ListItem>();
                    List<ListItem> ledSegments = new List<ListItem>();
                    List<ListItem> servos = new List<ListItem>();
                    List<ListItem> stepper = new List<ListItem>();

                    foreach (IConnectedDevice device in module.GetConnectedDevices())
                    {
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
                        }                        
                    }
                    displayPinPanel.WideStyle = true;
                    displayPinPanel.SetPorts(new List<ListItem>());
                    displayPinPanel.SetPins(outputs);

                    displayLedDisplayPanel.WideStyle = true;
                    displayLedDisplayPanel.displayLedAddressComboBox.SelectedIndexChanged += new EventHandler(displayLedAddressComboBox_SelectedIndexChanged);
                    displayLedDisplayPanel.SetAddresses(ledSegments);

                    servoPanel.SetAdresses(servos);
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
            }
            catch (Exception)
            {
                MessageBox.Show(MainForm._tr("uiMessageNotImplementedYet"), 
                                MainForm._tr("Hint"), 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
        }

        void displayLedAddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = displayModuleNameComboBox;                
            String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());
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
                    _syncConfigToForm(rows[0]["settings"] as OutputConfigItem);
                }
            }
        }

        private void _usePresetConfig(OutputConfigItem cfg)
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
                removeError(sender as Control);
            }
            catch (Exception exc)
            {
                Log.Instance.log("fsuipcMultiplyTextBox_Validating : Parsing problem, " + exc.Message, LogSeverity.Debug);
                displayError(sender as Control, MainForm._tr("uiMessageFsuipcConfigPanelMultiplyWrongFormat"));
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
                Log.Instance.log("_validatingHexFields : Parsing problem, " + exc.Message, LogSeverity.Debug);
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
                    int.Parse(displayLedDisplayPanel.displayLedAddressComboBox.Text);
                    removeError(displayLedDisplayPanel.displayLedAddressComboBox);
                }
                catch (Exception exc)
                {
                    Log.Instance.log("displayLedDisplayComboBox_Validating : Parsing problem, " + exc.Message, LogSeverity.Debug);
                    e.Cancel = true;
                    tabControlFsuipc.SelectedTab = displayTabPage;
                    displayLedDisplayPanel.displayLedAddressComboBox.Focus();
                    displayError(displayLedDisplayPanel.displayLedAddressComboBox, MainForm._tr("uiMessageConfigWizard_ProvideLedDisplayAddress"));
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
                displayError(preconditionPinComboBox, MainForm._tr("Please_select_a_pin"));
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
            String serial = ArcazeModuleSettings.ExtractSerial(cb.SelectedItem.ToString());
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
    }
}
