using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
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
            if (!(config.Device is Servo)) return;

            var servo = config.Device as Servo;

            if (!ComboBoxHelper.SetSelectedItem(servoAddressesComboBox, servo.Address))
            {
                // TODO: provide error message
                Log.Instance.log($"Exception on selecting item {servo.Address} in Servo address ComboBox.", LogSeverity.Error);
            }

            if (servo.Min != null) minValueTextBox.Text = servo.Min;
            if (servo.Max != null) maxValueTextBox.Text = servo.Max;
            if (servo.MaxRotationPercent != null) maxRotationPercentNumericUpDown.Text = servo.MaxRotationPercent;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.Device = new Servo();
            var servo = config.Device as Servo;
            if (servoAddressesComboBox.SelectedValue != null)
            {
                servo.Address = servoAddressesComboBox.SelectedValue.ToString();
                servo.Min = minValueTextBox.Text;
                servo.Max = maxValueTextBox.Text;
                servo.MaxRotationPercent = maxRotationPercentNumericUpDown.Text;
            }

            return config;
        }
    }
}
