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
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            button.Pin = mfPinComboBox.Text;
            button.Name = textBox1.Text;
            Changed(button, new EventArgs());
        }
    }
}
