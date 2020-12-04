using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
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

        public MFServoPanel(MobiFlight.Config.Servo servo, List<byte> FreePins)
            : this()
        {
            List<byte> Pin1Pins = FreePins.ToList(); Pin1Pins.Add(Convert.ToByte(servo.DataPin)); Pin1Pins.Sort();
            foreach (byte pin in Pin1Pins) mfPinComboBox.Items.Add(pin);
            
            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }
            // TODO: Complete member initialization
            this.servo = servo;
            ComboBoxHelper.SetSelectedItem(mfPinComboBox, servo.DataPin);
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
            servo.DataPin = mfPinComboBox.Text;
            servo.Name = textBox1.Text;
        }
    }
}
