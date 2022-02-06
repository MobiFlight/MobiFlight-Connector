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
    public partial class MFEncoderPanel : UserControl
    {

        private MobiFlight.Config.Encoder encoder;
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private bool initialized = false;

        public event EventHandler Changed;

        public MFEncoderPanel()
        {
            InitializeComponent();
            mfLeftPinComboBox.Items.Clear();
            mfRightPinComboBox.Items.Clear();
        }

        public MFEncoderPanel(MobiFlight.Config.Encoder encoder, List<MobiFlightPin> Pins) : this()
        {
            pinList = Pins;
            this.encoder = encoder;
            update_lists();

            // Default standard selected values, next pins available
            mfLeftPinComboBox.SelectedValue = byte.Parse(encoder.PinLeft);
            mfRightPinComboBox.SelectedValue = byte.Parse(encoder.PinRight);
            ComboBoxHelper.SetSelectedItemByIndex(mfEncoderTypeComboBox, int.Parse(encoder.EncoderType));
            textBox1.Text = encoder.Name;

            initialized = true;
        }

        private void setNonPinValues()
        {
            encoder.EncoderType = mfEncoderTypeComboBox.SelectedIndex.ToString();
            encoder.Name = textBox1.Text;
        }
        private void update_lists()
        {
            bool ex_initialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfLeftPinComboBox, pinList, encoder.PinLeft);
            ComboBoxHelper.BindMobiFlightFreePins(mfRightPinComboBox, pinList, encoder.PinRight);
            initialized = ex_initialized;
        }

        private void update_all(ComboBox comboBox)
        {
            bool ex_initialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (encoder.XXXPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfLeftPinComboBox) { ComboBoxHelper.reassignPin(mfLeftPinComboBox, pinList, ref encoder.PinLeft); } else
            if (comboBox == mfRightPinComboBox) { ComboBoxHelper.reassignPin(mfRightPinComboBox, pinList, ref encoder.PinRight); }
            // then the others are updated too 
            update_lists();

            initialized = ex_initialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            update_all(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(encoder, new EventArgs());
        }
    }
}
