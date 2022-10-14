using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;
using MobiFlight.UI.Panels.Config;

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
        
        internal void syncFromConfig(OutputConfigItem config)
        {
            // preselect display stuff
            if (config.LcdDisplay.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(DisplayComboBox, config.LcdDisplay.Address.ToString()))
                {
                    // TODO: provide error message
                    Log.Instance.log($"{GetType().Name}:syncFromConfig : Exception on selecting item in LCD Address ComboBox", LogSeverity.Debug);
                }
            }
            if (config.LcdDisplay.Lines.Count > 0)
                lcdDisplayTextBox.Lines = config.LcdDisplay.Lines.ToArray();
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            // check if this is currently selected and properly initialized
            if (DisplayComboBox.SelectedValue == null) return config;

            config.LcdDisplay.Address = DisplayComboBox.SelectedValue.ToString().Split(',').ElementAt(0);

            config.LcdDisplay.Lines.Clear();
            foreach (String line in lcdDisplayTextBox.Lines)
            {
                config.LcdDisplay.Lines.Add(line);
            }
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
