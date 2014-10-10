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

        public MFButtonPanel()
        {
            InitializeComponent();
            for (int i = 0; i != 56; i++)
            {
                mfPinComboBox.Items.Add(i);
            }
            mfPinComboBox.SelectedIndex = 0;
        }

        public MFButtonPanel(Config.Button button)
            : this()
        {
            // TODO: Complete member initialization
            this.button = button;
            ComboBoxHelper.SetSelectedItem(mfPinComboBox, button.Pin);
            textBox1.Text = button.Name;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            button.Pin = mfPinComboBox.Text;
            button.Name = textBox1.Text;

            if (Changed != null)
                Changed(button, new EventArgs());
        }
    }
}
