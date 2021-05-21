﻿using MobiFlight.OutputConfig;
using MobiFlight.UI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{


    public partial class SimConnectPanel : UserControl
    {
        public String PresetFile { get; set; }
        public String PresetFileUser { get; set; }
        Dictionary<String, List<SimVarPreset>> data = new Dictionary<string, List<SimVarPreset>>();

        public SimConnectPanel()
        {
            InitializeComponent();

            PresetFile = Properties.Settings.Default.PresetFileMSFS2020SimVars;
            PresetFileUser = Properties.Settings.Default.PresetFileMSFS2020SimVarsUser;

            transformOptionsGroup1.setMode(true);
            transformOptionsGroup1.ShowSubStringPanel(false);

            PresetComboBox.Enabled = false;

            _loadPresets();
        }

        private void _loadPresetData(String file, String DefaultGroupKey, String Prefix)
        {
            string[] lines = System.IO.File.ReadAllLines(PresetFile);
            String GroupKey = DefaultGroupKey;
            data.Add(DefaultGroupKey, new List<SimVarPreset>());

            foreach (string line in lines)
            {
                if (line.StartsWith("//")) continue;
                var cols = line.Split('#');
                if (cols.Count() == 2 && "GROUP" == cols[1])
                {
                    GroupKey = Prefix + cols[0];
                    if (data.ContainsKey(GroupKey)) continue;

                    data.Add(GroupKey, new List<SimVarPreset>());
                }
                else if (cols.Count() == 3)
                {
                    data[GroupKey].Add(new SimVarPreset() { Label = cols[0], Code = cols[1], Description = cols[2] });
                }
            }

            if (data[DefaultGroupKey].Count == 0) data.Remove(DefaultGroupKey);
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

                try
                {
                    
                    _loadPresetData(PresetFile, "Default", "");


                    if (System.IO.File.Exists(PresetFileUser))
                    {
                        Log.Instance.log("SimConnectPanel.cs: User events found.", LogSeverity.Debug);
                        _loadPresetData(PresetFileUser, "User", "User: ");
                    }
                    else
                    {
                        Log.Instance.log("SimConnectPanel.cs: No user event found.", LogSeverity.Debug);
                    }

                    GroupComboBox.Items.Clear();
                    PresetComboBox.Items.Clear();
                    GroupComboBox.Items.Add(i18n._tr("uiSimConnectPanelComboBoxPresetSelectGroup"));
                    GroupComboBox.SelectedIndex = 0;

                    PresetComboBox.Items.Add(i18n._tr("uiSimConnectPanelComboBoxPresetSelect"));
                    PresetComboBox.SelectedIndex = 0;

                    foreach (String key in data.Keys)
                    {
                        GroupComboBox.Items.Add(key);
                    }
                }
                catch (Exception e)
                {
                    isLoaded = false;
                    MessageBox.Show(i18n._tr("uiMessageConfigWizard_ErrorLoadingPresets"), i18n._tr("Hint"));
                }
            }
            GroupComboBox.Enabled = isLoaded;
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
            SimVarNameTextBox.Text = config.SimConnectValue.Value;
            transformOptionsGroup1.syncFromConfig(config);

            // try to find the "command"
            foreach (String key in data.Keys)
            {
                if (!data[key].Exists(x=>x.Code==config.SimConnectValue.Value)) continue;

                GroupComboBox.SelectedIndexChanged -= GroupComboBox_SelectedIndexChanged;
                GroupComboBox.Text = key;

                PresetComboBox.DataSource = data[key];
                PresetComboBox.ValueMember = "Code";
                PresetComboBox.DisplayMember = "Label";

                GroupComboBox.SelectedIndexChanged += GroupComboBox_SelectedIndexChanged;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_Simulation_Variables.htm");
        }

        private void GroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selectedDevice = (sender as ComboBox).SelectedItem as String;
            if (!data.ContainsKey(selectedDevice)) return;

            // this happens on first select
            if (GroupComboBox.Items[0] as String == i18n._tr("uiSimConnectPanelComboBoxPresetSelectGroup"))
            {
                GroupComboBox.Items.RemoveAt(0);
            }

            PresetComboBox.DataSource = data[selectedDevice];
            PresetComboBox.ValueMember = "Code";
            PresetComboBox.DisplayMember = "Label";

            PresetComboBox.Enabled = true;
        }

        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).SelectedValue == null) return;

            SimVarNameTextBox.Text = (sender as ComboBox).SelectedValue.ToString();
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
    }

    public class SimVarPreset
    {
        public String Code { get; set; }
        public String Label { get; set; }
        public String Description { get; set; }
    }
}
