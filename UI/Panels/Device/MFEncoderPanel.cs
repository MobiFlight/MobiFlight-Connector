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
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private MobiFlight.Config.Encoder encoder;
        bool initialized = false;

        public MFEncoderPanel()
        {
            InitializeComponent();
            mfLeftPinComboBox.Items.Clear();
            mfRightPinComboBox.Items.Clear();
        }

        public MFEncoderPanel(MobiFlight.Config.Encoder encoder, List<MobiFlightPin> Pins)
            : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfLeftPinComboBox, Pins, encoder.PinLeft);
            ComboBoxHelper.BindMobiFlightFreePins(mfRightPinComboBox, Pins, encoder.PinRight);
            
            // Default standard selected values, next pins available
            if (mfLeftPinComboBox.Items.Count > 1)
            {
                mfLeftPinComboBox.SelectedIndex = 0;
                mfRightPinComboBox.SelectedIndex = 1;
            }

            this.encoder = encoder;
            mfLeftPinComboBox.SelectedValue = byte.Parse(encoder.PinLeft);
            mfRightPinComboBox.SelectedValue = byte.Parse(encoder.PinRight);

            ComboBoxHelper.SetSelectedItemByIndex(mfEncoderTypeComboBox, int.Parse(encoder.EncoderType));

            textBox1.Text = encoder.Name;
            setValues();

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(encoder, new EventArgs());
        }

        private void setValues()
        {
            encoder.PinLeft = mfLeftPinComboBox.SelectedItem.ToString();
            encoder.PinRight = mfRightPinComboBox.SelectedItem.ToString();
            encoder.EncoderType = mfEncoderTypeComboBox.SelectedIndex.ToString();
            encoder.Name = textBox1.Text;
        }
    }
}
