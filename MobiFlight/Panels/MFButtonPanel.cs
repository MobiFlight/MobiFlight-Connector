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
    public partial class MFButtonPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private Config.Button button;
        bool initialized = false;

        public MFButtonPanel(List<int> usedPins)
        {
            InitializeComponent();
            foreach (Int16 i in MobiFlightModuleInfo.MEGA_PINS)
            {
                if (usedPins.IndexOf(i) == -1)
                {
                    mfPinComboBox.Items.Add(i);
                }
            }
            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }
        }

        public MFButtonPanel(Config.Button button, List<int> usedPins)
            : this(usedPins)
        {
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
