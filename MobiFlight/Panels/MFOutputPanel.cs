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

        public MFOutputPanel(List<byte> FreePins)
        {
            InitializeComponent();
            foreach (Int16 i in FreePins)
            {
                mfPinComboBox.Items.Add(i);
            }
            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }
        }

        public MFOutputPanel(Config.Output output, List<byte> FreePins)
            : this(FreePins)
        {
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
