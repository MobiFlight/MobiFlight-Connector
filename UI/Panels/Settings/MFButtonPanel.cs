using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFButtonPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private MobiFlight.Config.Button button;
        bool initialized = false;

        public MFButtonPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFButtonPanel(MobiFlight.Config.Button button, List<byte> FreePins)
            : this()
        {
            List<byte> Pin1Pins = FreePins.ToList(); Pin1Pins.Add(Convert.ToByte(button.Pin)); Pin1Pins.Sort();
            foreach (byte pin in Pin1Pins) mfPinComboBox.Items.Add(pin);

            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }

            // TODO: Complete member initialization
            this.button = button;
            ComboBoxHelper.SetSelectedItem(mfPinComboBox, button.Pin);
            textBox1.Text = button.Name;
            setValues();

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(button, new EventArgs());
        }

        private void setValues()
        {
            button.Pin = mfPinComboBox.Text;
            button.Name = textBox1.Text;
        }
    }
}
