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
    public partial class StepperPanel : UserControl
    {


        public event EventHandler<ManualCalibrationTriggeredEventArgs> OnManualCalibrationTriggered;
        public event EventHandler OnSetZeroTriggered;
        int[] StepValues = { -50, -10, -1, 1, 10, 50 };


        public StepperPanel()
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
            stepperAddressesComboBox.SelectedValue = value;
        }

        public void SetAdresses(List<ListItem> pins)
        {
            stepperAddressesComboBox.DataSource = new List<ListItem>(pins);
            stepperAddressesComboBox.DisplayMember = "Label";
            stepperAddressesComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                stepperAddressesComboBox.SelectedIndex = 0;

            stepperAddressesComboBox.Enabled = pins.Count > 0;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OnManualCalibrationTriggered != null)
            {
                ManualCalibrationTriggeredEventArgs eventArgs = new ManualCalibrationTriggeredEventArgs();                
                eventArgs.Steps = StepValues[trackBar1.Value];
                OnManualCalibrationTriggered(sender, eventArgs);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (OnSetZeroTriggered != null)
            {
                OnSetZeroTriggered(sender, e);
            }
        }
    }

    public class ManualCalibrationTriggeredEventArgs : EventArgs
    {
        public int Steps { get; set; }
    }
}
