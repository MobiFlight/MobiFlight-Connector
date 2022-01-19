using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFServoPanel : UserControl
    {
        private MobiFlight.Config.Servo servo;
        private bool initialized = false;
        public event EventHandler Changed;

        public MFServoPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFServoPanel(MobiFlight.Config.Servo servo, List<MobiFlightPin> Pins): this()
        {
            this.servo = servo;
            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, Pins, servo.DataPin);

            ////if (mfPinComboBox.Items.Count > 0) {
            ////    mfPinComboBox.SelectedIndex = 0;
            ////}
            ////// TODO: Complete member initialization
            ////mfPinComboBox.SelectedValue = byte.Parse(servo.DataPin);
            
            textBox1.Text = servo.Name;
            ////setValues();

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
