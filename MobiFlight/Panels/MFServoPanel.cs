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

        public MFServoPanel()
        {
            InitializeComponent();
            foreach (Int16 i in MobiFlightModuleInfo.MEGA_PINS)
            {
                mfPinComboBox.Items.Add(i);
            }
            mfPinComboBox.SelectedIndex = 0;
        }

        public MFServoPanel(Config.Servo servo)
            : this()
        {
            // TODO: Complete member initialization
            this.servo = servo;
            ComboBoxHelper.SetSelectedItem(mfPinComboBox, servo.DataPin);
            textBox1.Text = servo.Name;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            servo.DataPin = mfPinComboBox.Text;
            servo.Name = textBox1.Text;

            if (Changed!=null)
                Changed(servo, new EventArgs());
        }
    }
}
