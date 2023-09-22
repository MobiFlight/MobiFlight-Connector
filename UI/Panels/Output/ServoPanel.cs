using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
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

        public void syncFromConfig(OutputConfigItem config)
        {
            if (!ComboBoxHelper.SetSelectedItem(servoAddressesComboBox, config.Servo.Address))
            {
                // TODO: provide error message
                Log.Instance.log($"Exception on selecting item {config.Servo.Address} in Servo address ComboBox.", LogSeverity.Error);
            }

            if (config.Servo.Min != null) minValueTextBox.Text = config.Servo.Min;
            if (config.Servo.Max != null) maxValueTextBox.Text = config.Servo.Max;
            if (config.Servo.MaxRotationPercent != null) maxRotationPercentNumericUpDown.Text = config.Servo.MaxRotationPercent;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (servoAddressesComboBox.SelectedValue != null)
            {
                config.Servo.Address = servoAddressesComboBox.SelectedValue.ToString();
                config.Servo.Min = minValueTextBox.Text;
                config.Servo.Max = maxValueTextBox.Text;
                config.Servo.MaxRotationPercent = maxRotationPercentNumericUpDown.Text;
            }

            return config;
        }
    }
}
