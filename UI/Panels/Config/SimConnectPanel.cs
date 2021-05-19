using MobiFlight.OutputConfig;
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

            _loadPresets();
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
                    string[] lines = System.IO.File.ReadAllLines(PresetFile);
                    string GroupKey = "Default";
                    data.Add(GroupKey, new List<SimVarPreset>());

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("//")) continue;
                        var cols = line.Split('#');
                        if (cols.Count() == 2 && "GROUP" == cols[1])
                        {
                            GroupKey = cols[0];
                            if (data.ContainsKey(GroupKey)) continue;

                            data.Add(GroupKey, new List<SimVarPreset>());
                        }
                        else if (cols.Count() == 3)
                        {
                            data[GroupKey].Add(new SimVarPreset() { Label = cols[0], Code = cols[1], Description = cols[2] });
                        }
                    }

                    if (data["Default"].Count == 0) data.Remove("Default");


                    if (System.IO.File.Exists(PresetFileUser))
                    {
                        Log.Instance.log("SimConnectPanel.cs: User events found.", LogSeverity.Debug);
                        lines = System.IO.File.ReadAllLines(PresetFileUser);
                        GroupKey = "User";
                        data.Add(GroupKey, new List<SimVarPreset>());


                        foreach (string line in lines)
                        {
                            if (line.StartsWith("//")) continue;
                            var cols = line.Split('#');
                            if (cols.Count() == 2 && "GROUP" == cols[1])
                            {
                                GroupKey = "User: " + cols[0];
                                if (data.ContainsKey(GroupKey)) continue;

                                data.Add(GroupKey, new List<SimVarPreset>());
                            }
                            else if (cols.Count() == 3)
                            {
                                data[GroupKey].Add(new SimVarPreset() { Label = cols[0], Code = cols[1], Description = cols[2] });
                            }
                        }

                        if (data["User"].Count == 0) data.Remove("User");
                    }
                    else
                    {
                        Log.Instance.log("SimConnectPanel.cs: No user event found.", LogSeverity.Debug);
                    }

                    GroupComboBox.Items.Clear();
                    EventIdComboBox.Items.Clear();

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
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_Simulation_Variables.htm");
        }

        private void DeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selectedDevice = (sender as ComboBox).SelectedItem as String;
            if (!data.ContainsKey(selectedDevice)) return;

            EventIdComboBox.DataSource = data[selectedDevice];
            EventIdComboBox.ValueMember = "Code";
            EventIdComboBox.DisplayMember = "Label";

            EventIdComboBox.SelectedIndex = 0;
        }

        private void EventIdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SimVarNameTextBox.Text = (sender as ComboBox).SelectedValue.ToString();
        }
    }

    public class SimVarPreset
    {
        public String Code { get; set; }
        public String Label { get; set; }
        public String Description { get; set; }
    }
}
