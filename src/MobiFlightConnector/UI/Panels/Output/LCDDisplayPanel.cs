using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class LCDDisplayPanel : UserControl
    {
        DataView dv;
        int Cols = 16;
        int Lines = 2;
        static byte MAX_CONFIG_REFS = 6;
        static string[] CONFIG_REFS_PLACEHOLDER = { "#", "§", "&", "?", "@", "^", "%" };

        public LCDDisplayPanel()
        {
            InitializeComponent();
        }
        
        public void SetAddresses(List<ListItem> ports)
        {
            DisplayComboBox.DataSource = new List<ListItem>(ports);
            DisplayComboBox.DisplayMember = "Label";
            DisplayComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                DisplayComboBox.SelectedIndex = 0;

            DisplayComboBox.Enabled = ports.Count > 0;
        }

        public void DisableOutputDefinition()
        {
            label2.Visible = false;
            panel2.Visible = false;
            label3.Visible = false;
            lcdDisplayTextBox.Text = string.Empty;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            if (!(config.Device is LcdDisplay)) return;

            var lcdDisplay = config.Device as LcdDisplay;
            // preselect display stuff
            if (lcdDisplay.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(DisplayComboBox, lcdDisplay.Address))
                {
                    // TODO: provide error message
                    Log.Instance.log($"Exception on selecting item {lcdDisplay.Address} in LCD address ComboBox.", LogSeverity.Error);
                }
            }
            if (lcdDisplay.Lines.Count > 0)
                lcdDisplayTextBox.Lines = lcdDisplay.Lines.ToArray();
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            // check if this is currently selected and properly initialized
            if (DisplayComboBox.SelectedValue == null) return config;

            config.Device = new LcdDisplay()
            {
                Address = DisplayComboBox.SelectedValue.ToString().Split(',').ElementAt(0),
                Lines = new List<string>(lcdDisplayTextBox.Lines)
            };

            return config;
        }

        private void DisplayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).SelectedValue == null) return;

            Cols = int.Parse(((sender as ComboBox).SelectedValue.ToString()).Split(',').ElementAt(1));
            Lines = int.Parse(((sender as ComboBox).SelectedValue.ToString()).Split(',').ElementAt(2));
            lcdDisplayTextBox.Width = 4 + (Cols * 8);
            lcdDisplayTextBox.Height = Lines * 16;
        }

        private void lcdDisplayTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
