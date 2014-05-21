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
    public partial class DisplayLedDisplayPanel : UserControl
    {
        public bool WideStyle = false;

        public DisplayLedDisplayPanel()
        {
            InitializeComponent();
        }

        public void SetAddresses(List<ListItem> ports)
        {
            displayLedAddressComboBox.DataSource = new List<ListItem>(ports);
            displayLedAddressComboBox.DisplayMember = "Label";
            displayLedAddressComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                displayLedAddressComboBox.SelectedIndex = 0;

            displayLedAddressComboBox.Enabled = ports.Count > 0;
            displayLedAddressComboBox.Width = WideStyle ? displayLedAddressComboBox.MaximumSize.Width : displayLedAddressComboBox.MinimumSize.Width;
        }

        public void SetConnectors(List<ListItem> pins)
        {
            displayLedConnectorComboBox.DataSource = new List<ListItem>(pins);
            displayLedConnectorComboBox.DisplayMember = "Label";
            displayLedConnectorComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayLedConnectorComboBox.SelectedIndex = 0;

            displayLedConnectorComboBox.Enabled = pins.Count > 0;            
        }
    }
}
