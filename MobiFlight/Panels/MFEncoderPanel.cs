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

        public MFEncoderPanel()
        {
            InitializeComponent();
            for (int i = 0; i != 56; i++)
            {
                mfLeftPinComboBox.Items.Add(i);
                mfRightPinComboBox.Items.Add(i);
            }
            mfLeftPinComboBox.SelectedIndex = 0;
            mfRightPinComboBox.SelectedIndex = 0;
        }

        public MFEncoderPanel(Config.Encoder encoder)
            : this()
        {
            // TODO: Complete member initialization
            this.encoder = encoder;
            ComboBoxHelper.SetSelectedItem(mfLeftPinComboBox, encoder.PinLeft);
            ComboBoxHelper.SetSelectedItem(mfRightPinComboBox, encoder.PinRight);
            textBox1.Text = encoder.Name;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            encoder.PinLeft = mfLeftPinComboBox.Text;
            encoder.PinRight = mfRightPinComboBox.Text;
            encoder.Name = textBox1.Text;
            Changed(encoder, new EventArgs());
        }
    }
}
