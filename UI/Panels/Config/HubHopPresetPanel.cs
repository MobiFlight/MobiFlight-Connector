using MobiFlight.Base;
using MobiFlight.HubHop;
using MobiFlight.InputConfig;
using MobiFlight.OutputConfig;
using MobiFlight.UI.Forms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public enum HubHopPanelMode {
        Output,
        Input
    }

    public partial class HubHopPresetPanel : UserControl
    {
        public const byte MINIMUM_SEARCH_STRING_LENGTH = 1;
        public event EventHandler OnGetLVarListRequested;
        private event EventHandler OnLVarsSet;
        private Timer searchDebounceTimer;

        private HubHopPanelMode _mode;
        public HubHopPanelMode Mode { 
            get { return _mode; } 
            set { 
                if (_mode == value) return;
                _mode = value;
                OnModeChanged(value);
            } 
        }

        private FlightSimType _flightSimType;
        public FlightSimType FlightSimType
        {
            get { return _flightSimType; }
            set
            {
                if (_flightSimType == value) return;
                _flightSimType = value;
                OnFlightSimTypeChanged(value);
            }
        }

        private void OnModeChanged(HubHopPanelMode value)
        {
            UpdateElementsBasedOnPanelMode(value, FlightSimType);
        }

        private void OnFlightSimTypeChanged(FlightSimType value)
        {
            UpdateElementsBasedOnPanelMode(Mode, value);
        }

        private void UpdateElementsBasedOnPanelMode(HubHopPanelMode panelMode, FlightSimType simType)
        {
            LVarExamplePanel.Visible = panelMode == HubHopPanelMode.Output && FlightSimType == FlightSimType.MSFS2020;
            AVarExamplePanel.Visible = panelMode == HubHopPanelMode.Output && FlightSimType == FlightSimType.MSFS2020;
            ExampleLabel.Visible = panelMode == HubHopPanelMode.Output && FlightSimType == FlightSimType.MSFS2020;
            ValuePanel.Visible = panelMode == HubHopPanelMode.Input && FlightSimType == FlightSimType.XPLANE;
            HintLabelPresetCodeLabel.Visible = FlightSimType == FlightSimType.MSFS2020;

            if (panelMode == HubHopPanelMode.Input)
            {
                FilterVendorPanel.Width = 100;
                AircraftFilterPanel.Width = 100;
                SystemFilterPanel.Width = 100;
                TextFilterPanel.Width = 100;

                CodeActionPanel.Visible = FlightSimType == FlightSimType.XPLANE;
            }

            Msfs2020Panel.Visible = simType == FlightSimType.MSFS2020;
            CodeActionPanel.Visible = simType == FlightSimType.XPLANE && Mode == HubHopPanelMode.Input;

            if (simType == FlightSimType.MSFS2020)
            {
                PresetList = Msfs2020HubhopPresetListSingleton.Instance;
            }
            
            if (simType == FlightSimType.XPLANE)
            {
                PresetList = XplaneHubhopPresetListSingleton.Instance;
                // XPLANE initialization
                List<ListItem> listItems = new List<ListItem>();
                listItems.Add(new ListItem() { Value = XplaneInputAction.INPUT_TYPE_DATAREF, Label = XplaneInputAction.INPUT_TYPE_DATAREF });
                listItems.Add(new ListItem() { Value = XplaneInputAction.INPUT_TYPE_COMMAND, Label = XplaneInputAction.INPUT_TYPE_COMMAND });

                CodeTypeComboBox.DataSource = listItems;
                CodeTypeComboBox.ValueMember = "Value";
                CodeTypeComboBox.DisplayMember = "Label";
            }
        }

        public String PresetFile { get; set; }
        public String PresetFileUser { get; set; }
        
        protected List<String> lVars = new List<string>();

        SimConnectLvarsListForm LVarsListForm = new SimConnectLvarsListForm();

        Msfs2020HubhopPresetList PresetList = null;
        Msfs2020HubhopPresetList FilteredPresetList = new Msfs2020HubhopPresetList();

        public List<String> LVars
        {
            get { return lVars; }
            set
            {
                if (lVars.Count == 0 && value.Count == 0) return;

                lVars = value;
                OnLVarsSet?.Invoke(lVars, new EventArgs());
            }
        }
        

        public HubHopPresetPanel()
        {
            InitializeComponent();


            PresetList = Msfs2020HubhopPresetListSingleton.Instance;

            Disposed += HubHopPresetPanel_Disposed;

            linkLabel1.LinkClicked += this.linkLabel1_LinkClicked;
            ExpandButton.Click += this.ExpandButton_Click;
            ShowExpertSettingsCheckBox.CheckedChanged += this.ShowExpertSerttingsCheckBox_CheckedChanged;            
            ResetButton.Click += this.ResetFilterButton_Click;
            OnLVarsSet += SimConnectPanel_OnLVarsSet;
            ExpertSettingsPanel.Visible = false;

            PresetFile = Properties.Settings.Default.PresetFileMSFS2020SimVars;
            // New Json File
            PresetFile = @"Presets\msfs2020_hubhop_presets.json";
            
            // Not sure if we want to keep the user file?
            // Would have to be JSON too...
            PresetFileUser = Properties.Settings.Default.PresetFileMSFS2020SimVarsUser;

            PresetPanel.Enabled = false;

            if (Properties.Settings.Default.SimVarTextBoxExpanded)
            {
                MaximizeSimVarNameTextBox();
            }

            SimVarNameTextBox.TextChanged += SimVarNameTextBox_TextChanged;
            FilterTextBox.TextChanged += textBox1_TextChanged;

            CodeTypeComboBox.SelectedValueChanged += (sender, e) =>
            {
                ValuePanel.Visible = Mode == HubHopPanelMode.Input && FlightSimType == FlightSimType.XPLANE && (CodeTypeComboBox.SelectedValue.ToString() == XplaneInputAction.INPUT_TYPE_DATAREF);
            };
        }

        private void HubHopPresetPanel_Disposed(object sender, EventArgs e)
        {
            // Explicitly setting the HubHopPresets to null actually 
            // helps with the Garbage Collection    
            FilteredPresetList.Clear();
        }

        public void LoadPresets()
        {
            _loadPresets();
        }

        private void SimConnectPanel_OnLVarsSet(object sender, EventArgs e)
        {
            LVarListButton.Enabled = true;
            if (LVarsListForm.Visible) return;

            LVarsListForm.SetLVarsList(LVars);
            LVarsListForm.StartPosition = FormStartPosition.CenterParent;
            LVarsListForm.BringToFront();
            if (LVarsListForm.ShowDialog() == DialogResult.OK)
            {
                SimVarNameTextBox.Text = "(L:" + LVarsListForm.SelectedVariable + ")";
            }
        }

        private void _loadPresets()
        {
            bool isLoaded = true;

            if (!System.IO.File.Exists(PresetFile))
            {
                isLoaded = false;
                MessageBox.Show(i18n._tr("uiMessageConfigWizard_PresetsNotFound"), i18n._tr("Hint"));
            }
            else
            {
                HubHopType hubhopType = HubHopType.Output;
                if (Mode==HubHopPanelMode.Input) hubhopType = HubHopType.AllInputs;
                try
                {
                    PresetList.Load(PresetFile);
                    FilterPresetList();
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
                }
            }
            SystemComboBox.Enabled = isLoaded;
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            config.XplaneDataRef = new xplane.XplaneDataRef();
            config.SimConnectValue = new SimConnectValue();

            if (FlightSimType==FlightSimType.MSFS2020)
            {
                config.SimConnectValue.VarType = SimConnectVarType.CODE;

                Msfs2020HubhopPreset selectedPreset = (PresetComboBox.Items[PresetComboBox.SelectedIndex] as Msfs2020HubhopPreset);

                config.SimConnectValue.UUID = selectedPreset?.id;
                config.SimConnectValue.Value = SimVarNameTextBox.Text.ToLF();
            } else if (FlightSimType == FlightSimType.XPLANE)
            {
                config.XplaneDataRef.Path = SimVarNameTextBox.Text;
            }
        }

        internal InputConfig.InputAction ToConfig()
        {
            if (FlightSimType==FlightSimType.XPLANE) 
                return ToXplaneConfig();

            return ToMsfsConfig();
        }

        internal InputConfig.MSFS2020CustomInputAction ToMsfsConfig()
        {
            Msfs2020HubhopPreset selectedPreset = (PresetComboBox.Items[PresetComboBox.SelectedIndex] as Msfs2020HubhopPreset);

            MobiFlight.InputConfig.MSFS2020CustomInputAction result =
                new InputConfig.MSFS2020CustomInputAction()
                {
                    PresetId = selectedPreset?.id,
                    Command = SimVarNameTextBox.Text.ToLF()
                };
            return result;
        }

        internal InputConfig.XplaneInputAction ToXplaneConfig()
        {
            MobiFlight.InputConfig.XplaneInputAction result = new InputConfig.XplaneInputAction();
            result.InputType = CodeTypeComboBox.SelectedValue.ToString();
            result.Path = SimVarNameTextBox.Text;
            result.Expression = ValueTextBox.Text;
            return result;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            if (FlightSimType == FlightSimType.MSFS2020)
                syncFromConfigMSFS(config);

            if (FlightSimType == FlightSimType.XPLANE)
                syncFromConfigXplane(config);
        }

        internal void syncFromConfigXplane(OutputConfigItem config)
        {
            var VariableValue = config.XplaneDataRef.Path;

            // Restore the code
            if (VariableValue != "")
            {
                SimVarNameTextBox.TextChanged -= SimVarNameTextBox_TextChanged;
                SimVarNameTextBox.Text = VariableValue;
                SimVarNameTextBox.TextChanged += SimVarNameTextBox_TextChanged;
            }

            // Try to find the original preset and 
            // initialize comboboxes accordingly
            String OriginalCode = VariableValue;
            TryToSelectOriginalPresetFromCode(OriginalCode);
        }

        internal void syncFromConfigMSFS(OutputConfigItem config)
        {
            var VariableValue = config.SimConnectValue.Value;

            // Restore the code
            if (config.SimConnectValue.Value != "") { 
                SimVarNameTextBox.TextChanged -= SimVarNameTextBox_TextChanged;
                SimVarNameTextBox.Text = config.SimConnectValue.Value.ToCRLF();
                SimVarNameTextBox.TextChanged += SimVarNameTextBox_TextChanged;
            }

            if (config.SimConnectValue.UUID!=null) {
                // Try to find the preset by UUID
                Msfs2020HubhopPreset OriginalPreset =
                    PresetList.FindByUUID(Mode == HubHopPanelMode.Input ? HubHopType.AllInputs : HubHopType.Output, config.SimConnectValue.UUID);
                if (OriginalPreset != null)
                {
                    String Code = Regex.Replace(config.SimConnectValue.Value, @":\d+", ":index");
                    
                    // Code was modified by user after
                    // originally using a preset as template
                    ShowExpertSettingsCheckBox.Checked = Code != OriginalPreset.code;

                    // We have found the original preset
                    InitializeComboBoxesWithPreset(OriginalPreset);
                    return;
                }
            }

            // Try to find the original preset and 
            // initialize comboboxes accordingly
            String OriginalCode = config.SimConnectValue.Value;
            TryToSelectOriginalPresetFromCode(OriginalCode);
        }

        internal void syncFromConfig(InputConfig.MSFS2020CustomInputAction inputAction)
        {
            if (inputAction == null || inputAction.Command == "") return;

            // Restore the code
            SimVarNameTextBox.Text = inputAction.Command.ToCRLF();

            if (inputAction.PresetId != null)
            {
                // Try to find the preset by UUID
                Msfs2020HubhopPreset OriginalPreset =
                    PresetList.FindByUUID(Mode == HubHopPanelMode.Input ? HubHopType.AllInputs : HubHopType.Output, inputAction.PresetId);
                if (OriginalPreset != null)
                {
                    String Code = Regex.Replace(inputAction.Command, @":\d+", ":index");

                    // Code was modified by user after
                    // originally using a preset as template
                    ShowExpertSettingsCheckBox.Checked = Code != OriginalPreset.code;

                    // We have found the original preset
                    InitializeComboBoxesWithPreset(OriginalPreset);
                    return;
                }
            }

            // Try to find the original preset and 
            // initialize comboboxes accordingly
            String OriginalCode = inputAction.Command;

            TryToSelectOriginalPresetFromCode(OriginalCode);
        }

        internal void syncFromConfig(MSFS2020EventIdInputAction inputAction)
        {
            if (inputAction == null) return;

            Msfs2020EventPresetList deprecatedPresets = new Msfs2020EventPresetList();
            deprecatedPresets.Load();

            String OriginalCode = deprecatedPresets.FindCodeByEventId(inputAction.EventId);

            if (OriginalCode == null)
            {
                return;
            }

            // Restore the code
            SimVarNameTextBox.Text = OriginalCode.ToCRLF();
            TryToSelectOriginalPresetFromCode(OriginalCode);
        }

        private void TryToSelectOriginalPresetFromCode(string Code)
        {
            if (Code == null || Code == "") return;

            String OriginalCode = Regex.Replace(Code, @":\d+", ":index");

            Msfs2020HubhopPreset OriginalPreset = 
                PresetList.FindByCode(Mode == HubHopPanelMode.Input ? HubHopType.AllInputs : HubHopType.Output, OriginalCode);

            if (OriginalPreset==null)
            {
                ShowExpertSettingsCheckBox.Checked = true;
                Msfs2020HubhopPreset CustomPreset = new Msfs2020HubhopPreset()
                {
                    id = "0",
                    label = "Customized Preset or Custom Code",
                    code = Code,
                    description = "This is a customized preset or custom code."
                };
                FilteredPresetList.Items.Add(CustomPreset);
                FilteredPresetListChanged();
                // We have found the original preset
                InitializeComboBoxesWithPreset(CustomPreset);
                return;
            }

            // We have found the original preset
            InitializeComboBoxesWithPreset(OriginalPreset);
        }

        private void InitializeComboBoxesWithPreset(Msfs2020HubhopPreset preset)
        {
            ComboBoxHelper.SetSelectedItemByValue(VendorComboBox, preset.vendor);
            ComboBoxHelper.SetSelectedItemByValue(AircraftComboBox, preset.aircraft);
            ComboBoxHelper.SetSelectedItemByValue(SystemComboBox, preset.system);
            PresetComboBox.SelectedIndexChanged -= PresetComboBox_SelectedIndexChanged;
            PresetComboBox.SelectedValue = preset.id;
            DescriptionLabel.Text = preset?.description;
            PresetComboBox.SelectedIndexChanged += PresetComboBox_SelectedIndexChanged;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Simulation_Variables.htm");
        }

        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null) return;
            Msfs2020HubhopPreset selectedItem = (sender as ComboBox).SelectedItem as Msfs2020HubhopPreset;

            Msfs2020HubhopPreset selectedPreset = FilteredPresetList.Items.Find(x => x.id == selectedItem.id);
            if (selectedPreset == null) return;
            DescriptionLabel.Text = selectedPreset?.description;
            SimVarNameTextBox.Text = selectedPreset?.code?.ToCRLF();
            
            if (FlightSimType==FlightSimType.XPLANE)
            {
                try
                {
                    CodeTypeComboBox.SelectedValue = selectedPreset.codeType.ToString();
                }
                catch (Exception)
                {
                    CodeTypeComboBox.SelectedValue = "DataRef";
                }
            }

            DescriptionLabel.Enabled = selectedItem.id != "-";
        }

        private void SimVarNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text.Contains(":index"))
            {
                SimConnectPanelIndexSelectForm form = new SimConnectPanelIndexSelectForm();
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    (sender as TextBox).Text = (sender as TextBox).Text.Replace(":index", ":" + form.IndexValue);
                }
            }
        }

        private void GetLVarsListButton_Click(object sender, EventArgs e)
        {
            if (LVarsListForm.Visible) return;

            OnGetLVarListRequested?.Invoke(sender, e);
            LVarListButton.Enabled = false;
        }

        private void ExpandButton_Click(object sender, EventArgs e)
        {
            if (SimVarNameTextBoxIsMaximized())
            {
                MinimizeSimVarNameTextBox();
            }
            else
            {
                MaximizeSimVarNameTextBox();
            }

        }

        private bool SimVarNameTextBoxIsMaximized()
        {
            return SimVarNameTextBox.Height > 21;
        }

        private void MaximizeSimVarNameTextBox()
        {
            ExpandButton.Text = "-";
            SimVarNameTextBox.Height = 88;
            SimVarNameTextBox.ScrollBars = ScrollBars.Both;
            Properties.Settings.Default.SimVarTextBoxExpanded = true;
        }

        private void MinimizeSimVarNameTextBox()
        {
            ExpandButton.Text = "+";
            SimVarNameTextBox.Height = 21;
            SimVarNameTextBox.ScrollBars = ScrollBars.None;
            Properties.Settings.Default.SimVarTextBoxExpanded = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterPresetListDelayedByMs(300);
        }

        private void FilterPresetListDelayedByMs(int miliseconds)
        {
            searchDebounceTimer?.Dispose();
            searchDebounceTimer = new Timer { Interval = miliseconds };
            searchDebounceTimer.Tick += (s, _) =>
            {
                searchDebounceTimer.Stop();
                searchDebounceTimer.Dispose();

                FilterPresetList();
            };
            searchDebounceTimer.Start();
        }

        private void FilterPresetList()
        {
            String SelectedVendor = null;
            String SelectedAircraft = null;
            String SelectedSystem = null;
            String FilterText = null;

            if (VendorComboBox.SelectedIndex > 0) SelectedVendor = VendorComboBox.SelectedItem.ToString();
            if (AircraftComboBox.SelectedIndex > 0) SelectedAircraft = AircraftComboBox.SelectedItem.ToString();
            if (SystemComboBox.SelectedIndex > 0) SelectedSystem = SystemComboBox.SelectedItem.ToString();
            if (FilterTextBox.Text != "" && FilterTextBox.Text.Length >= MINIMUM_SEARCH_STRING_LENGTH) FilterText = FilterTextBox.Text;

            FilteredPresetList.Items.Clear();
            FilteredPresetList.Items.Add(new Msfs2020HubhopPreset()
            {
                label = "- Select Preset -",
                id = "-",
                code = "",
                description = "No Preset selected."
            });

            HubHopType hubhopType = HubHopType.Output;
            if (Mode == HubHopPanelMode.Input) hubhopType = HubHopType.AllInputs;

            FilteredPresetList.Items.AddRange(
                PresetList.Filtered(
                    hubhopType,
                    SelectedVendor,
                    SelectedAircraft,
                    SelectedSystem,
                    FilterText
                    )
            );
            
            // Substract 1 because of the static "select preset"-label
            int MatchesFound = FilteredPresetList.Items.Count - 1;
            MatchLabel.Text = String.Format(
                                    i18n._tr("uiMessagesSimConnectPanelMatchesFound"),
                                    MatchesFound);

            FilteredPresetListChanged();
        }

        private void FilteredPresetListChanged()
        {
            HubHopType hubhopType = HubHopType.Output;
            if (Mode == HubHopPanelMode.Input) hubhopType = HubHopType.AllInputs;

            // update categories when we have hits
            // otherwise keep categories from last filter
            if (FilteredPresetList.Items.Count > 1)
            {
                UpdateValues(VendorComboBox, FilteredPresetList.AllVendors(hubhopType).ToArray());
                UpdateValues(AircraftComboBox, FilteredPresetList.AllAircraft(hubhopType).ToArray());
                UpdateValues(SystemComboBox, FilteredPresetList.AllSystems(hubhopType).ToArray());
            }

            UpdatePresetComboBoxValues();
        }

        private void UpdatePresetComboBoxValues()
        {
            String SelectedValue = null;
            Msfs2020HubhopPreset selectedPreset = null;

            PresetComboBox.SelectedIndexChanged -= PresetComboBox_SelectedIndexChanged;
            if (PresetComboBox.SelectedIndex > 0)
            {
                selectedPreset = (PresetComboBox.Items[PresetComboBox.SelectedIndex] as Msfs2020HubhopPreset);
                SelectedValue = selectedPreset.id;
                if (selectedPreset.id == "0")
                {
                    FilteredPresetList.Items.Add(selectedPreset);
                }
            }

            PresetComboBox.DataSource = null;
            PresetComboBox.DataSource = FilteredPresetList.Items;
            PresetComboBox.ValueMember = "id";
            PresetComboBox.DisplayMember = "label";

            if (SelectedValue != null)
            {                
                PresetComboBox.SelectedValue = SelectedValue;

                // we didn't find the preset within the current
                // list
                if (PresetComboBox.SelectedValue == null)
                PresetComboBox.SelectedIndex = 0;
            }
            else
            {
                PresetComboBox.SelectedIndex = 0;
            }

            PresetPanel.Enabled = (FilteredPresetList.Items.Count > 1);

            PresetComboBox.SelectedIndexChanged += PresetComboBox_SelectedIndexChanged;
        }

        private void UpdateValues(ComboBox cb, String[] valueList)
        {
            String SelectedValue = null;
            cb.SelectedIndexChanged -= OnFilter_SelectedIndexChanged;
            if (cb.SelectedIndex > 0)
            {
                SelectedValue = cb.SelectedItem.ToString();
            }

            cb.Items.Clear();
            cb.Items.Add(i18n._tr("uiMessagesSimConnectPanelShowAll"));
            cb.SelectedIndex = 0;
            cb.Items.AddRange(valueList);

            if (SelectedValue != null)
                ComboBoxHelper.SetSelectedItem(cb, SelectedValue);

            cb.SelectedIndexChanged += OnFilter_SelectedIndexChanged;
        }

        private void OnFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterPresetList();
        }

        private void ShowExpertSerttingsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ExpertSettingsPanel.Visible = ShowExpertSettingsCheckBox.Checked;
        }

        private void ResetFilterButton_Click(object sender, EventArgs e)
        {
            ResetFilter();
        }

        private void ResetFilter()
        {
            SuspendUpdateEvents();
            VendorComboBox.SelectedIndex = 0;
            AircraftComboBox.SelectedIndex = 0;
            SystemComboBox.SelectedIndex = 0;
            FilterTextBox.Text = "";
            FilterPresetList();

            ResumeUpdateEvents();
        }

        private void SuspendUpdateEvents()
        {
            VendorComboBox.SelectedIndexChanged -= OnFilter_SelectedIndexChanged;
            AircraftComboBox.SelectedIndexChanged -= OnFilter_SelectedIndexChanged;
            SystemComboBox.SelectedIndexChanged -= OnFilter_SelectedIndexChanged;
            FilterTextBox.TextChanged -= textBox1_TextChanged;
        }

        private void ResumeUpdateEvents()
        {
            // make sure to remove potentially
            // registered events
            SuspendUpdateEvents();

            // And now really register
            // all required events
            VendorComboBox.SelectedIndexChanged += OnFilter_SelectedIndexChanged;
            AircraftComboBox.SelectedIndexChanged += OnFilter_SelectedIndexChanged;
            SystemComboBox.SelectedIndexChanged += OnFilter_SelectedIndexChanged;
            FilterTextBox.TextChanged += textBox1_TextChanged;
        }

        public void syncFromConfig(XplaneInputAction inputAction)
        {
            if (inputAction == null) inputAction = new InputConfig.XplaneInputAction();

            try
            {
                CodeTypeComboBox.SelectedValue = inputAction.InputType;
            }
            catch (Exception)
            {
                CodeTypeComboBox.SelectedValue = "DataRef";
            }

            // Restore the code
            SimVarNameTextBox.Text = inputAction.Path;
            ValueTextBox.Text = inputAction.Expression;

            // Try to find the original preset and 
            // initialize comboboxes accordingly
            String OriginalCode = inputAction.Path;

            TryToSelectOriginalPresetFromCode(OriginalCode);
        }
    }
}
