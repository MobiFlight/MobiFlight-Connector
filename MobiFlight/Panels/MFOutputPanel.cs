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

        public MFOutputPanel()
        {
            InitializeComponent();
            for (int i = 0; i != 56; i++)
            {
                mfPinComboBox.Items.Add(i);
            }
            mfPinComboBox.SelectedIndex = 0;
        }

        public MFOutputPanel(Config.Output output)
            : this()
        {
            // TODO: Complete member initialization
            this.output = output;
            ComboBoxHelper.SetSelectedItem(mfPinComboBox, output.Pin);
            textBox1.Text = output.Name;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            output.Pin = mfPinComboBox.Text;
            output.Name = textBox1.Text;
            Changed(output, new EventArgs());
        }
    }
}
