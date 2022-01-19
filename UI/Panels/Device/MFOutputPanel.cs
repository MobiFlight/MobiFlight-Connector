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

        private MobiFlight.Config.Output output;
        private Board MobiFlightBoard;
        private bool initialized = false;
        
        public event EventHandler Changed;

        public MFOutputPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFOutputPanel(MobiFlight.Config.Output output, List<MobiFlightPin> Pins, Board MobiFlightBoard): this()
        {
            this.output = output;
            this.MobiFlightBoard = MobiFlightBoard;

            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, Pins, output.Pin);
            ////if (mfPinComboBox.Items.Count > 0) {
            ////    mfPinComboBox.SelectedIndex = 0;
            ////}
            //// TODO: Complete member initialization
            //mfPinComboBox.SelectedValue = byte.Parse(output.Pin);
            
            textBox1.Text = output.Name;
            //setValues();

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
            byte bPin = byte.Parse(mfPinComboBox.SelectedItem.ToString());
            var pin = MobiFlightBoard.Pins.Find(x => (x.Pin == bPin));
            return pin.isPWM;
        }

        private void setValues()
        {
            mfPinLabel.Text = isPwmPin() ? "Pin (PWM)" : "Pin";
            output.Pin = mfPinComboBox.SelectedItem.ToString();
            output.Name = textBox1.Text;
        }
    }
}
