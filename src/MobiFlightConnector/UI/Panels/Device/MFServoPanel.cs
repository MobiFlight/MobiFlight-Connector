using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFServoPanel : UserControl
    {
        private MobiFlight.Firmware.Servo servo;
        private bool initialized = false;
        public event EventHandler Changed;

        public MFServoPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFServoPanel(MobiFlight.Firmware.Servo servo, List<MobiFlightPin> Pins): this()
        {
            this.servo = servo;
            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, Pins, servo.DataPin);
            textBox1.Text = servo.Name;
            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            setValues();
            if (Changed!=null)
                Changed(servo, new EventArgs());
        }

        private void setValues()
        {
            servo.DataPin = mfPinComboBox.SelectedItem.ToString();
            servo.Name = textBox1.Text;
        }
    }
}
