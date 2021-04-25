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
    public partial class MFOutputPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private MobiFlight.Config.Output output;
        bool initialized = false;
        String MobiFlightModuleType;

        public MFOutputPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFOutputPanel(MobiFlight.Config.Output output, List<MobiFlightPin> Pins, String MobiFlightModuleType)
            : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, Pins, output.Pin);
            this.MobiFlightModuleType = MobiFlightModuleType;

            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }

            // TODO: Complete member initialization
            this.output = output;
            ComboBoxHelper.SetSelectedItem(mfPinComboBox, output.Pin);
            textBox1.Text = output.Name;
            setValues();

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(output, new EventArgs());
        }

        private bool isPwmPin()
        {
            bool result = false;
            byte bPin = byte.Parse(mfPinComboBox.Text);
            MobiFlightPin pin;

            switch (MobiFlightModuleType)
            {
                case MobiFlightModuleInfo.TYPE_MEGA:
                    pin = MobiFlightModuleInfo.MEGA_PINS.Find(x => (x.Pin == bPin));
                    result = pin.isPWM;
                    break;

                case MobiFlightModuleInfo.TYPE_MICRO:
                    pin = MobiFlightModuleInfo.MICRO_PINS.Find(x => (x.Pin == bPin));
                    result = pin.isPWM;
                    break;

                case MobiFlightModuleInfo.TYPE_UNO:
                    pin = MobiFlightModuleInfo.UNO_PINS.Find(x => (x.Pin == bPin));
                    result = pin.isPWM;
                    break;

            }
            return result;
        }

        private void setValues()
        {
            mfPinLabel.Text = isPwmPin() ? "Pin (PWM)" : "Pin";
            output.Pin = mfPinComboBox.Text;
            output.Name = textBox1.Text;
        }
    }
}
