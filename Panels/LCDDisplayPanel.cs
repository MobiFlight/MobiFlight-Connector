using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;
using MobiFlight.Panels.Group;

namespace MobiFlight.Panels
{
    public partial class LCDDisplayPanel : UserControl
    {
        DataView dv;

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

        public void SetConfigRefsDataView(DataView dv, String filterGuid)
        {
            this.dv = dv;
            dv.RowFilter = "guid <> '" + filterGuid + "'";
        }
        
        internal void syncFromConfig(OutputConfigItem config)
        {
            // preselect display stuff
            if (config.LcdDisplay.Address != null)
            {
                if (!ComboBoxHelper.SetSelectedItem(DisplayComboBox, config.LcdDisplay.Address.ToString()))
                {
                    // TODO: provide error message
                    Log.Instance.log("_syncConfigToForm : Exception on selecting item in LCD Address ComboBox", LogSeverity.Debug);
                }
            }

            lcdDisplayTextBox.Lines = config.LcdDisplay.Lines.ToArray();
            configRefItemPanel.Controls.Clear();

            foreach (ConfigRef configRef in config.ConfigRefs)
            {
                ConfigRefPanel p = new ConfigRefPanel();
                p.SetDataView(dv);
                p.syncFromConfig(configRef);

                p.Dock = DockStyle.Top;
                configRefItemPanel.Controls.Add(p);
            }

            string[] placeholder = { "#", "§", "&", "%" };

            while (configRefItemPanel.Controls.Count < 4)
            {
                ConfigRefPanel p = new ConfigRefPanel();
                p.SetDataView(dv);
                p.SetPlaceholder(placeholder[configRefItemPanel.Controls.Count]);

                p.Dock = DockStyle.Top;
                configRefItemPanel.Controls.Add(p);
            }
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

            config.ConfigRefs.Clear();

            // sync the config ref settings back to the config
            foreach (ConfigRefPanel p in configRefItemPanel.Controls.OfType<ConfigRefPanel>())
            {
                ConfigRef configRef = new ConfigRef();
                p.syncToConfig(configRef);
                config.ConfigRefs.Add(configRef);
            }

            return config;
        }

        private void DisplayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).SelectedValue == null) return;

            int Cols = int.Parse(((sender as ComboBox).SelectedValue.ToString()).Split(',').ElementAt(1));
            int Lines = int.Parse(((sender as ComboBox).SelectedValue.ToString()).Split(',').ElementAt(2));
            lcdDisplayTextBox.Width = 4 + (Cols * 8);
            lcdDisplayTextBox.Height = Lines * 16;
        }
    }
}
