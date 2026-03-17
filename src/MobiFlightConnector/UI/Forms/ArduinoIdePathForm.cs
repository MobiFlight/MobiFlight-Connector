using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Forms
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
                    i18n._tr("uiMessageInvalidArduinoIdePath"),
                    i18n._tr("Hint")
                    );
            }
            else
            {
                button1.Enabled = true;
                Properties.Settings.Default.ArduinoIdePathDefault = firmwareArduinoIdePathTextBox.Text;
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

        private void arduinoDownloadLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.arduinoDownloadLinkLabel.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.arduino.cc/en/Main/Software");            
        }
    }
}
