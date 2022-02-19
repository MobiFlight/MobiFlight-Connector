using MobiFlight.HubHop;
using MobiFlight.InputConfig;
using MobiFlight.OutputConfig;
using MobiFlight.UI.Forms;
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

        private HubHopPanelMode _mode;
        public HubHopPanelMode Mode { 
            get { return _mode; } 
            set { 
                if (_mode == value) return;
                _mode = value;
                OnModeChanged(value);
            } 
        }

        private void OnModeChanged(HubHopPanelMode value)
        {
            LVarExamplePanel.Visible = value == HubHopPanelMode.Output;
            AVarExamplePanel.Visible = value == HubHopPanelMode.Output;
            ExampleLabel.Visible = value == HubHopPanelMode.Output;

            ShowExpertSettingsCheckBox.Checked = value == HubHopPanelMode.Input;

            if (value == HubHopPanelMode.Input)
            {
                FilterVendorPanel.Width = 100;
                AircraftFilterPanel.Width = 100;
                SystemFilterPanel.Width = 100;
                TextFilterPanel.Width = 100;
            }
        }

        public String PresetFile { get; set; }
        public String PresetFileUser { get; set; }
        Dictionary<String, List<SimVarPreset>> data = new Dictionary<string, List<SimVarPreset>>();

        protected List<String> lVars = new List<string>();

        SimConnectLvarsListForm LVarsListForm = new SimConnectLvarsListForm();

        Msfs2020HubhopPresetList PresetList = new Msfs2020HubhopPresetList();
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

            PresetComboBox.Enabled = false;

            _loadPresets();

            if (Properties.Settings.Default.SimVarTextBoxExpanded)
            {
                MaximizeSimVarNameTextBox();
            }

            SimVarNameTextBox.TextChanged += SimVarNameTextBox_TextChanged;
            //PresetComboBox.SelectedIndexChanged += PresetComboBox_SelectedIndexChanged;
            FilterTextBox.TextChanged += textBox1_TextChanged;
            //VendorComboBox.SelectedIndexChanged += OnFilter_SelectedIndexChanged;
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
                if (Mode==HubHopPanelMode.Input) hubhopType = HubHopType.Input | HubHopType.InputPotentiometer;
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
            config.SimConnectValue.VarType = SimConnectVarType.CODE;
            switch (config.SimConnectValue.VarType)
            {
                case SimConnectVarType.CODE:
                    config.SimConnectValue.Value = SimVarNameTextBox.Text;
                    break;
            }
            config.SimConnectValue.Value = SimVarNameTextBox.Text;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.MSFS2020CustomInputAction result = 
                new InputConfig.MSFS2020CustomInputAction()
                {
                    Command = SimVarNameTextBox.Text
                };
            return result;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // Restore the code
            if (config.SimConnectValue.Value != "") { 
                SimVarNameTextBox.TextChanged -= SimVarNameTextBox_TextChanged;
                SimVarNameTextBox.Text = config.SimConnectValue.Value;
                SimVarNameTextBox.TextChanged += SimVarNameTextBox_TextChanged;
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
            SimVarNameTextBox.Text = inputAction.Command;

            // Try to find the original preset and 
            // initialize comboboxes accordingly
            String OriginalCode = inputAction.Command;

            TryToSelectOriginalPresetFromCode(OriginalCode);
        }

        internal void syncFromConfig(MSFS2020EventIdInputAction inputAction)
        {
            Msfs2020EventPresetList deprecatedPresets = new Msfs2020EventPresetList();
            deprecatedPresets.Load();

            String OriginalCode = deprecatedPresets.FindCodeByEventId(inputAction.EventId);

            if (OriginalCode == null)
            {
                return;
            }

            // Restore the code
            SimVarNameTextBox.Text = OriginalCode;
            TryToSelectOriginalPresetFromCode(OriginalCode);
        }

        private void TryToSelectOriginalPresetFromCode(string Code)
        {
            if (Code == null || Code == "") return;

            String OriginalCode = Regex.Replace(Code, @":\d+", ":index");

            Msfs2020HubhopPreset OriginalPreset = 
                PresetList.FindByCode(Mode == HubHopPanelMode.Input ? HubHopType.Input | HubHopType.InputPotentiometer : HubHopType.Output, OriginalCode);

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
            VendorComboBox.SuspendLayout();
            AircraftComboBox.SuspendLayout();
            SystemComboBox.SuspendLayout();
            // ---
            ComboBoxHelper.SetSelectedItemByValue(VendorComboBox, preset.vendor);
            ComboBoxHelper.SetSelectedItemByValue(AircraftComboBox, preset.aircraft);
            ComboBoxHelper.SetSelectedItemByValue(SystemComboBox, preset.system);
            PresetComboBox.SelectedIndexChanged -= PresetComboBox_SelectedIndexChanged;
            PresetComboBox.SelectedValue = preset.id;
            DescriptionLabel.Text = preset?.description;
            PresetComboBox.SelectedIndexChanged += PresetComboBox_SelectedIndexChanged;
            // ---
            VendorComboBox.ResumeLayout();
            AircraftComboBox.ResumeLayout();
            SystemComboBox.ResumeLayout();
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
            SimVarNameTextBox.Text = selectedPreset?.code;

            DescriptionLabel.Enabled = selectedItem.id != "-";

            InitializeComboBoxesWithPreset(selectedPreset);
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
            FilterPresetList();
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
            if (Mode == HubHopPanelMode.Input) hubhopType = HubHopType.Input;

            FilteredPresetList.Items.AddRange(
                PresetList.Filtered(
                    hubhopType,
                    SelectedVendor,
                    SelectedAircraft,
                    SelectedSystem,
                    FilterText
                    )
            );

            /*
            String Code = SimVarNameTextBox.Text;
            FilteredPresetList.Items.Add(
                new Msfs2020HubhopPreset()
                {
                    id = "0",
                    label = "Customized Preset or Custom Code",
                    code = Code,
                    description = "This is a customized preset or custom code."
                }
            );
            */

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
            if (Mode == HubHopPanelMode.Input) hubhopType = HubHopType.Input;

            UpdateValues(VendorComboBox, FilteredPresetList.AllVendors(hubhopType).ToArray());
            UpdateValues(AircraftComboBox, FilteredPresetList.AllAircraft(hubhopType).ToArray());
            UpdateValues(SystemComboBox, FilteredPresetList.AllSystems(hubhopType).ToArray());
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
            }
            else
            {
                PresetComboBox.SelectedIndex = 0;
            }

            PresetComboBox.Enabled = (FilteredPresetList.Items.Count > 1);

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
            //TryToSelectOriginalPresetFromCode(SimVarNameTextBox.Text);
        }

        private void ResetFilter()
        {
            SuspendUpdateEvents();

            VendorComboBox.SelectedIndex = 0;
            AircraftComboBox.SelectedIndex = 0;
            SystemComboBox.SelectedIndex = 0;
            FilterTextBox.Text = "";

            FilterPresetList();
        }

        private void SuspendUpdateEvents()
        {
            VendorComboBox.SelectedIndexChanged -= OnFilter_SelectedIndexChanged;
            AircraftComboBox.SelectedIndexChanged -= OnFilter_SelectedIndexChanged;
            SystemComboBox.SelectedIndexChanged -= OnFilter_SelectedIndexChanged;
            FilterTextBox.TextChanged -= textBox1_TextChanged;
        }

        public class SimVarPreset
        {
            public String Code { get; set; }
            public String Label { get; set; }
            public String Description { get; set; }
        }
    }
}
