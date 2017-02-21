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
    public partial class MFEncoderPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private Config.Encoder encoder;
        bool initialized = false;

        public MFEncoderPanel(List<byte> FreePins)
        {
            InitializeComponent();
            foreach (byte i in FreePins)
            {
                mfLeftPinComboBox.Items.Add(i);
                mfRightPinComboBox.Items.Add(i);
            }
            if (mfLeftPinComboBox.Items.Count > 1)
            {
                mfLeftPinComboBox.SelectedIndex = 0;
                mfRightPinComboBox.SelectedIndex = 1;
            }
        }

        public MFEncoderPanel(Config.Encoder encoder, List<byte> FreePins)
            : this(FreePins)
        {
            // TODO: Complete member initialization
            this.encoder = encoder;
            ComboBoxHelper.SetSelectedItem(mfLeftPinComboBox, encoder.PinLeft);
            ComboBoxHelper.SetSelectedItem(mfRightPinComboBox, encoder.PinRight);
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
            encoder.PinLeft = mfLeftPinComboBox.Text;
            encoder.PinRight = mfRightPinComboBox.Text;
            encoder.Name = textBox1.Text;
        }
    }
}
