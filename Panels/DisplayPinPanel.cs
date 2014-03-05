using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArcazeUSB.Panels
{
    public partial class DisplayPinPanel : UserControl
    {
        public bool WideStyle = false;

        public DisplayPinPanel()
        {
            InitializeComponent();
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
            displayPortComboBox.DataSource = ports;
            displayPortComboBox.DisplayMember = "Label";
            displayPortComboBox.ValueMember = "Value";
            if (ports.Count>0)
                displayPortComboBox.SelectedIndex = 0;

            displayPortComboBox.Enabled = ports.Count > 0;
            displayPortComboBox.Width = WideStyle ? displayPortComboBox.MaximumSize.Width : displayPortComboBox.MinimumSize.Width;
        }

        public void SetPins(List<ListItem> pins)
        {
            displayPinComboBox.DataSource = pins;
            displayPinComboBox.DisplayMember = "Label";
            displayPinComboBox.ValueMember = "Value";

            if (pins.Count>0)
                displayPinComboBox.SelectedIndex = 0;

            displayPinComboBox.Visible = displayPinComboBox.Enabled = pins.Count > 0;
        }
    }
}
