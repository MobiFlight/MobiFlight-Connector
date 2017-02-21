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
    public partial class MFOutputPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private Config.Output output;
        bool initialized = false;

        public MFOutputPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFOutputPanel(Config.Output output, List<byte> FreePins)
            : this()
        {
            List<byte> Pin1Pins = FreePins.ToList(); Pin1Pins.Add(Convert.ToByte(output.Pin)); Pin1Pins.Sort();
            foreach (byte pin in Pin1Pins) mfPinComboBox.Items.Add(pin);

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

        private void setValues()
        {
            output.Pin = mfPinComboBox.Text;
            output.Name = textBox1.Text;
        }
    }
}
