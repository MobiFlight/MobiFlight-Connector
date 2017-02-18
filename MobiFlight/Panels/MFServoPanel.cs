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
    public partial class MFServoPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private Config.Servo servo;
        bool initialized = false;

        public MFServoPanel(List<int> usedPins)
        {
            InitializeComponent();
            foreach (Int16 i in MobiFlightModuleInfo.MEGA_PINS)
            {
                if (usedPins.IndexOf(i) == -1)
                {
                    mfPinComboBox.Items.Add(i);
                }
            }
            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }
        }

        public MFServoPanel(Config.Servo servo, List<int> usedPins)
            : this(usedPins)
        {
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
