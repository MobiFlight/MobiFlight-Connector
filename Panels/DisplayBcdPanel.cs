using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.Panels
{
    public partial class DisplayBcdPanel : UserControl
    {
        public DisplayBcdPanel()
        {
            InitializeComponent();
        }

        public void SetPorts(List<ListItem> ports)
        {
            // displayBcdPortComboBox
            displayBcdStrobePortComboBox.DataSource = new List<ListItem>(ports);
            displayBcdStrobePortComboBox.DisplayMember = "Label";
            displayBcdStrobePortComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                displayBcdStrobePortComboBox.SelectedIndex = 0;

            displayBcdStrobePortComboBox.Enabled = ports.Count > 0;

            displayBcdPortComboBox.DataSource = new List<ListItem>(ports);
            displayBcdPortComboBox.DisplayMember = "Label";
            displayBcdPortComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                displayBcdPortComboBox.SelectedIndex = 0;

            displayBcdPortComboBox.Enabled = ports.Count > 0;
        }

        /**
         *                         displayBcdPanel.displayBcdStrobePinComboBox.Items.Add(v);
                        displayBcdPanel.displayBcdPin1ComboBox.Items.Add(v);
                        displayBcdPanel.displayBcdPin2ComboBox.Items.Add(v);
                        displayBcdPanel.displayBcdPin3ComboBox.Items.Add(v);
                        displayBcdPanel.displayBcdPin4ComboBox.Items.Add(v);
         * */

        public void SetPins(List<ListItem> pins)
        {
            displayBcdStrobePinComboBox.DataSource = new List<ListItem>(pins);
            displayBcdStrobePinComboBox.DisplayMember = "Label";
            displayBcdStrobePinComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdStrobePinComboBox.SelectedIndex = 0;

            displayBcdStrobePinComboBox.Visible = displayBcdStrobePinComboBox.Enabled = pins.Count > 0;

            displayBcdPin1ComboBox.DataSource = new List<ListItem>(pins);
            displayBcdPin1ComboBox.DisplayMember = "Label";
            displayBcdPin1ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin1ComboBox.SelectedIndex = 0;

            displayBcdPin1ComboBox.Visible = displayBcdPin1ComboBox.Enabled = pins.Count > 0;

            displayBcdPin2ComboBox.DataSource = new List<ListItem>(pins);
            displayBcdPin2ComboBox.DisplayMember = "Label";
            displayBcdPin2ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin2ComboBox.SelectedIndex = 0;

            displayBcdPin2ComboBox.Visible = displayBcdPin2ComboBox.Enabled = pins.Count > 0;

            displayBcdPin3ComboBox.DataSource = new List<ListItem>(pins);
            displayBcdPin3ComboBox.DisplayMember = "Label";
            displayBcdPin3ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin3ComboBox.SelectedIndex = 0;

            displayBcdPin3ComboBox.Visible = displayBcdPin3ComboBox.Enabled = pins.Count > 0;

            displayBcdPin4ComboBox.DataSource = new List<ListItem>(pins);
            displayBcdPin4ComboBox.DisplayMember = "Label";
            displayBcdPin4ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin4ComboBox.SelectedIndex = 0;

            displayBcdPin4ComboBox.Visible = displayBcdPin4ComboBox.Enabled = pins.Count > 0;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // preselect BCD4056
            for (int i = 0; i < config.BcdPins.Count(); i++)
            {
                if (config.BcdPins[i] != "")
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
                        if (!ComboBoxHelper.SetSelectedItem(Controls["displayBcdPin" + i + "ComboBox"] as ComboBox, tmpPin)) { /* TODO: provide error message */ }
                    }
                }
            }
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.BcdPins.Clear();
            config.BcdPins.Add(displayBcdStrobePortComboBox.Text + displayBcdStrobePinComboBox.Text);
            for (int i = 1; i <= 4; i++)
            {
                config.BcdPins.Add(
                    displayBcdStrobePortComboBox.Text +
                    (Controls["displayBcdPin" + i + "ComboBox"] as ComboBox).Text);
            }

            return config;
        }
    }
}
