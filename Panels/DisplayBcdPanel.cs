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
    public partial class DisplayBcdPanel : UserControl
    {
        public DisplayBcdPanel()
        {
            InitializeComponent();
        }

        public void SetPorts(List<ListItem> ports)
        {
            // displayBcdPortComboBox
            displayBcdStrobePortComboBox.DataSource = ports;
            displayBcdStrobePortComboBox.DisplayMember = "Label";
            displayBcdStrobePortComboBox.ValueMember = "Value";
            if (ports.Count > 0)
                displayBcdStrobePortComboBox.SelectedIndex = 0;

            displayBcdStrobePortComboBox.Enabled = ports.Count > 0;

            displayBcdPortComboBox.DataSource = ports;
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
            displayBcdStrobePinComboBox.DataSource = pins;
            displayBcdStrobePinComboBox.DisplayMember = "Label";
            displayBcdStrobePinComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdStrobePinComboBox.SelectedIndex = 0;

            displayBcdStrobePinComboBox.Visible = displayBcdStrobePinComboBox.Enabled = pins.Count > 0;

            displayBcdPin1ComboBox.DataSource = pins;
            displayBcdPin1ComboBox.DisplayMember = "Label";
            displayBcdPin1ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin1ComboBox.SelectedIndex = 0;

            displayBcdPin1ComboBox.Visible = displayBcdPin1ComboBox.Enabled = pins.Count > 0;

            displayBcdPin2ComboBox.DataSource = pins;
            displayBcdPin2ComboBox.DisplayMember = "Label";
            displayBcdPin2ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin2ComboBox.SelectedIndex = 0;

            displayBcdPin2ComboBox.Visible = displayBcdPin2ComboBox.Enabled = pins.Count > 0;

            displayBcdPin3ComboBox.DataSource = pins;
            displayBcdPin3ComboBox.DisplayMember = "Label";
            displayBcdPin3ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin3ComboBox.SelectedIndex = 0;

            displayBcdPin3ComboBox.Visible = displayBcdPin3ComboBox.Enabled = pins.Count > 0;

            displayBcdPin4ComboBox.DataSource = pins;
            displayBcdPin4ComboBox.DisplayMember = "Label";
            displayBcdPin4ComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayBcdPin4ComboBox.SelectedIndex = 0;

            displayBcdPin4ComboBox.Visible = displayBcdPin4ComboBox.Enabled = pins.Count > 0;
        }
    }
}
