using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight
{
    public partial class ArduinoIdePathForm : Form
    {
        public ArduinoIdePathForm()
        {
            InitializeComponent();
        }

        private void firmwareArduinoIdeButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                firmwareArduinoIdePathTextBox.Text = fbd.SelectedPath;
                firmwareArduinoIdePathTextBox.Focus();
                (sender as Button).Focus();
            }

            if (!MobiFlightFirmwareUpdater.IsValidArduinoIdePath(firmwareArduinoIdePathTextBox.Text))
            {
                button1.Enabled = false;
                MessageBox.Show(
                    MainForm._tr("Hint"),
                    "Please check your Arduino IDE installation. The path cannot be used, avrdude has not been found.");
            }
            else
            {
                button1.Enabled = true;
                Properties.Settings.Default.ArduinoIdePath = firmwareArduinoIdePathTextBox.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
