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
    public partial class ServoPanel : UserControl
    {
        public ServoPanel()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void SetSelectedAddress(string value)
        {
            servoAddressesComboBox.SelectedValue = value;
        }

        public void SetAdresses(List<ListItem> pins)
        {
            servoAddressesComboBox.DataSource = new List<ListItem>(pins);
            servoAddressesComboBox.DisplayMember = "Label";
            servoAddressesComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                servoAddressesComboBox.SelectedIndex = 0;

            servoAddressesComboBox.Enabled = pins.Count > 0;            
        }
    }
}
