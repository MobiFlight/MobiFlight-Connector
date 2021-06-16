using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFServoPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private MobiFlight.Config.Servo servo;
        bool initialized = false;

        public MFServoPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFServoPanel(MobiFlight.Config.Servo servo, List<MobiFlightPin> Pins)
            : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, Pins, servo.DataPin);

            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }
            // TODO: Complete member initialization
            this.servo = servo;
            mfPinComboBox.SelectedValue = byte.Parse(servo.DataPin);
            textBox1.Text = servo.Name;
            setValues();

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
