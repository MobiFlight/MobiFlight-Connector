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
            if (!ComboBoxHelper.SetSelectedItem(servoAddressesComboBox, config.ServoAddress))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Servo Address ComboBox", LogSeverity.Debug);
            }

            if (config.ServoMin != null) minValueTextBox.Text = config.ServoMin;
            if (config.ServoMax != null) maxValueTextBox.Text = config.ServoMax;
            if (config.ServoMaxRotationPercent != null) maxRotationPercentNumericUpDown.Text = config.ServoMaxRotationPercent;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (servoAddressesComboBox.SelectedValue != null)
            {
                config.ServoAddress = servoAddressesComboBox.SelectedValue.ToString();
                config.ServoMin = minValueTextBox.Text;
                config.ServoMax = maxValueTextBox.Text;
                config.ServoMaxRotationPercent = maxRotationPercentNumericUpDown.Text;
            }

            return config;
        }
    }
}
