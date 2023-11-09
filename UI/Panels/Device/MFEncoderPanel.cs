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
            UpdateFreePinsInDropDowns();

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
        private void UpdateFreePinsInDropDowns()
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfLeftPinComboBox, pinList, encoder.PinLeft);
            ComboBoxHelper.BindMobiFlightFreePins(mfRightPinComboBox, pinList, encoder.PinRight);
            initialized = exInitialized;
        }

        private void ReassignFreePinsInDropDowns(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (encoder.XXXPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfLeftPinComboBox) { 
                ComboBoxHelper.reassignPin(mfLeftPinComboBox.SelectedItem.ToString(), pinList, ref encoder.PinLeft); 
            } else
            if (comboBox == mfRightPinComboBox) {
                ComboBoxHelper.reassignPin(mfRightPinComboBox.SelectedItem.ToString(), pinList, ref encoder.PinRight); 
            }
            // then the others are updated too 
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            if (sender is ComboBox)
                ReassignFreePinsInDropDowns(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(encoder, new EventArgs());
        }
    }
}
