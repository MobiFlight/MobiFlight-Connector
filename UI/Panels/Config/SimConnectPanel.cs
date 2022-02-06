using MobiFlight.OutputConfig;
using MobiFlight.SimConnectMSFS;
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

    public partial class SimConnectPanel : UserControl
    {
        public event EventHandler OnGetLVarListRequested;
        private event EventHandler OnLVarsSet;

        public String PresetFile { get; set; }
        public String PresetFileUser { get; set; }
        Dictionary<String, List<SimVarPreset>> data = new Dictionary<string, List<SimVarPreset>>();

        protected List<String> lVars = new List<string>();

        SimConnectLvarsListForm LVarsListForm = new SimConnectLvarsListForm();

        Msfs2020HubhopPresetList PresetList = new Msfs2020HubhopPresetList();
        Msfs2020HubhopPresetList FilteredPresetList = new Msfs2020HubhopPresetList();
        private Msfs2020HubhopPreset SelectedPreset = null;

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

        public SimConnectPanel()
        {
            InitializeComponent();

            ExpertSettingsPanel.Visible = false;

            PresetFile = Properties.Settings.Default.PresetFileMSFS2020SimVars;

            // New Json File
            PresetFile = @"Presets\msfs2020_presets.json";

            // Not sure if we want to keep the user file?
            // Would have to be JSON too...
            PresetFileUser = Properties.Settings.Default.PresetFileMSFS2020SimVarsUser;

            transformOptionsGroup1.setMode(true);
            transformOptionsGroup1.ShowSubStringPanel(false);

            PresetComboBox.Enabled = false;

            _loadPresets();
            OnLVarsSet += SimConnectPanel_OnLVarsSet;

            if (Properties.Settings.Default.SimVarTextBoxExpanded)
            {
                MaximizeSimVarNameTextBox();
            }
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
                SimVarNameTextBox.Text = "(L:"+LVarsListForm.SelectedVariable+")";
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
                SystemComboBox.SelectedIndexChanged -= GroupComboBox_SelectedIndexChanged;
                VendorComboBox.SelectedIndexChanged -= GroupComboBox_SelectedIndexChanged;
                AircraftComboBox.SelectedIndexChanged -= GroupComboBox_SelectedIndexChanged;

                try
                {
                    PresetList.Load(PresetFile);
                    FilteredPresetList.Items = PresetList.Filtered("Output", null, null, null, null);

                    VendorComboBox.Items.Clear();
                    VendorComboBox.Items.Add(i18n._tr("uiSimConnectPanelComboBoxPresetSelectGroup"));
                    VendorComboBox.Items.AddRange(PresetList.AllVendors("Output").ToArray());
                    VendorComboBox.SelectedIndex = 0;


                    AircraftComboBox.Items.Clear();
                    AircraftComboBox.Items.Add(i18n._tr("uiSimConnectPanelComboBoxPresetSelectGroup"));
                    AircraftComboBox.Items.AddRange(PresetList.AllAircraft("Output").ToArray());
                    AircraftComboBox.SelectedIndex = 0;


                    // System Combobox
                    SystemComboBox.Items.Clear();
                    SystemComboBox.Items.Add(i18n._tr("uiSimConnectPanelComboBoxPresetSelectGroup"));
                    SystemComboBox.Items.AddRange(PresetList.AllSystems("Output").ToArray());
                    SystemComboBox.SelectedIndex = 0;

                    // Presets ComboBox
                    PresetComboBox.ValueMember = "id";
                    PresetComboBox.DisplayMember = "label";
                    PresetComboBox.DataSource = FilteredPresetList.Items;
                    PresetComboBox.Enabled = true;
                    PresetComboBox.SelectedIndex = 0;
                    PresetComboBox_SelectedIndexChanged(PresetComboBox, EventArgs.Empty);
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
                }

                SystemComboBox.SelectedIndexChanged += GroupComboBox_SelectedIndexChanged;
                VendorComboBox.SelectedIndexChanged += GroupComboBox_SelectedIndexChanged;
                AircraftComboBox.SelectedIndexChanged += GroupComboBox_SelectedIndexChanged;
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
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // Restore the code
            SimVarNameTextBox.Text = config.SimConnectValue.Value;

            // Sync the transform panel
            transformOptionsGroup1.syncFromConfig(config);

            // Try to find the original preset and 
            // initialize comboboxes accordingly

            String OriginalCode = config.SimConnectValue.Value;
            String OriginalId = config.SimConnectValue.Value;

            if (OriginalCode == null) return;

            OriginalCode = Regex.Replace(OriginalCode, @":\d+", ":index");

            if (!PresetList.Items.Exists(x => x.code == OriginalCode))
            {
                ShowExpertSerttingsCheckBox.Checked = true;
                Msfs2020HubhopPreset CustomPreset = new Msfs2020HubhopPreset() { 
                    id = "0", 
                    label = "Customized Preset loaded", 
                    code = OriginalCode,
                    description = "This is a customized preset."
                };
                FilteredPresetList.Items.Add(CustomPreset);
                FilteredPresetListChanged();
                // We have found the original preset
                InitializeComboBoxesWithPreset(CustomPreset);
                return;
            }

            // We have found the original preset
            InitializeComboBoxesWithPreset(PresetList.Items.Find(x => x.code == OriginalCode));
        }

        private void InitializeComboBoxesWithPreset(Msfs2020HubhopPreset preset)
        {
            VendorComboBox.SelectedIndexChanged -= GroupComboBox_SelectedIndexChanged;
            AircraftComboBox.SelectedIndexChanged -= GroupComboBox_SelectedIndexChanged;
            SystemComboBox.SelectedIndexChanged -= GroupComboBox_SelectedIndexChanged;
            //PresetComboBox.SelectedIndexChanged -= PresetComboBox_SelectedIndexChanged;

            ComboBoxHelper.SetSelectedItemByValue(VendorComboBox, preset.vendor);
            ComboBoxHelper.SetSelectedItemByValue(AircraftComboBox, preset.aircraft);
            ComboBoxHelper.SetSelectedItemByValue(SystemComboBox, preset.system);
            PresetComboBox.SelectedValue = preset.id;

            VendorComboBox.SelectedIndexChanged += GroupComboBox_SelectedIndexChanged;
            AircraftComboBox.SelectedIndexChanged += GroupComboBox_SelectedIndexChanged;
            SystemComboBox.SelectedIndexChanged += GroupComboBox_SelectedIndexChanged;
            //PresetComboBox.SelectedIndexChanged += PresetComboBox_SelectedIndexChanged;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Simulation_Variables.htm");
        }

        private void GroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            return;
        }

        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).SelectedValue == null) return;
            String id = (sender as ComboBox).SelectedValue as String;

            Msfs2020HubhopPreset selectedPreset = FilteredPresetList.Items.Find(x => x.id == id);
            if (selectedPreset == null) return;
            DescriptionLabel.Text = selectedPreset?.description;
            SimVarNameTextBox.Text = selectedPreset?.code;
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
            if (FilterTextBox.Text != "" && FilterTextBox.Text.Length > 2) FilterText = FilterTextBox.Text;

            FilteredPresetList.Items.Clear();
            FilteredPresetList.Items.AddRange(
                PresetList.Filtered(
                    "Output",
                    SelectedVendor,
                    SelectedAircraft,
                    SelectedSystem,
                    FilterText
                    )
            );
            FilteredPresetListChanged();
        }

        private void FilteredPresetListChanged()
        {
            UpdateValues(VendorComboBox, FilteredPresetList.AllVendors("Output").ToArray());
            UpdateValues(AircraftComboBox, FilteredPresetList.AllAircraft("Output").ToArray());
            UpdateValues(SystemComboBox, FilteredPresetList.AllSystems("Output").ToArray());
            UpdatePresetComboBoxValues();
            MatchLabel.Text = $"{FilteredPresetList.Items.Count} presets found";
        }

        private void UpdatePresetComboBoxValues()
        {
            String SelectedValue = null;
            PresetComboBox.SelectedIndexChanged -= PresetComboBox_SelectedIndexChanged;
            if (PresetComboBox.SelectedIndex > 0)
            {
                SelectedValue = PresetComboBox.SelectedValue as String;
            }
            PresetComboBox.DataSource = null;
            PresetComboBox.ValueMember = "id";
            PresetComboBox.DisplayMember = "label";
            PresetComboBox.DataSource = FilteredPresetList.Items;

            if (SelectedValue != null)
            {
                PresetComboBox.SelectedValue = SelectedValue;
            }
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
            cb.Items.Add(i18n._tr("- show all -"));
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
            ExpertSettingsPanel.Visible = ShowExpertSerttingsCheckBox.Checked;
        }

        private void ResetFilterButton_Click(object sender, EventArgs e)
        {
            ResetFilter();
        }

        private void ResetFilter()
        {
            VendorComboBox.SelectedIndex = 0;
            AircraftComboBox.SelectedIndex = 0;
            SystemComboBox.SelectedIndex = 0;
            FilterTextBox.Text = "";
        }
    }

    public class SimVarPreset
    {
        public String Code { get; set; }
        public String Label { get; set; }
        public String Description { get; set; }
    }
}
