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
    public partial class DisplayPinPanel : UserControl
    {
        public bool WideStyle = false;

        public DisplayPinPanel()
        {
            InitializeComponent();
            displayPortComboBox.SelectedIndexChanged += displayPortComboBox_SelectedIndexChanged;
        }

        public void SetSelectedPort(string value)
        {
            displayPortComboBox.SelectedValue = value;
        }

        public void SetSelectedPin(string value)
        {
            displayPinComboBox.SelectedValue = value;
        }

        public void SetPorts(List<ListItem> ports)
        {
            displayPortComboBox.DataSource = new List<ListItem>(ports);
            displayPortComboBox.DisplayMember = "Label";
            displayPortComboBox.ValueMember = "Value";
            if (ports.Count>0)
                displayPortComboBox.SelectedIndex = 0;

            displayPortComboBox.Visible = displayPortComboBox.Enabled = ports.Count > 0;
            
        }

        public void SetPins(List<ListItem> pins)
        {
            displayPinComboBox.DataSource = new List<ListItem>(pins);
            displayPinComboBox.DisplayMember = "Label";
            displayPinComboBox.ValueMember = "Value";

            if (pins.Count>0)
                displayPinComboBox.SelectedIndex = 0;

            displayPinComboBox.Enabled = pins.Count > 0;
            displayPinComboBox.Width = WideStyle ? displayPinComboBox.MaximumSize.Width : displayPinComboBox.MinimumSize.Width;
        }

        private void displayPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            
            // enable setting only for ports higher than B
            // and set selected item to previous item if divider has been chosen
            // to ensure correct values
            if ((cb.SelectedItem as ListItem).Value == "-----")
            {
                cb.SelectedIndex -= 1;
                return;
            }

            displayPinBrightnessPanel.Enabled = cb.SelectedIndex > 2;
        }
    }
}
