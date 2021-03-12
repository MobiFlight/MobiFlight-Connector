using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class DisplayPinPanel : UserControl
    {
        public bool WideStyle = false;
        private MobiFlightModule Module;

        public DisplayPinPanel()
        {
            InitializeComponent();
            displayPortComboBox.SelectedIndexChanged += displayPortComboBox_SelectedIndexChanged;
        }

        public void SetSelectedPort(string value)
        {
            displayPortComboBox.SelectedValue = value;
        }

        public void SetSelectedPin(string value)
        {
            displayPinComboBox.SelectedValue = value;
        }

        public void SetPorts(List<ListItem> ports)
        {
            displayPortComboBox.DataSource = new List<ListItem>(ports);
            displayPortComboBox.DisplayMember = "Label";
            displayPortComboBox.ValueMember = "Value";
            if (ports.Count>0)
                displayPortComboBox.SelectedIndex = 0;

            displayPinBrightnessPanel.Visible = displayPinBrightnessPanel.Enabled = displayPortComboBox.Visible = displayPortComboBox.Enabled = ports.Count > 0;
            
        }

        public void SetPins(List<ListItem> pins)
        {
            displayPinComboBox.DataSource = new List<ListItem>(pins);
            displayPinComboBox.DisplayMember = "Label";
            displayPinComboBox.ValueMember = "Value";

            if (pins.Count>0)
                displayPinComboBox.SelectedIndex = 0;

            displayPinComboBox.Enabled = pins.Count > 0;
            displayPinComboBox.Width = WideStyle ? displayPinComboBox.MaximumSize.Width : displayPinComboBox.MinimumSize.Width;
        }

        private void displayPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            
            // enable setting only for ports higher than B
            // and set selected item to previous item if divider has been chosen
            // to ensure correct values
            if ((cb.SelectedItem as ListItem).Value == "-----")
            {
                cb.SelectedIndex -= 1;
                return;
            }

            displayPinBrightnessPanel.Enabled = cb.SelectedIndex > 2;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            String serial = config.DisplaySerial;
            if (serial != null && serial.Contains('/'))
            {
                serial = serial.Split('/')[1].Trim();
            }

            if (config.DisplayPin != null && config.DisplayPin != "")
            {
                string port = "";
                string pin = config.DisplayPin;

                if (serial != null && serial.IndexOf("SN") != 0)
                {
                    port = config.DisplayPin.Substring(0, 1);
                    pin = config.DisplayPin.Substring(1);
                }

                // preselect normal pin drop downs
                if (!ComboBoxHelper.SetSelectedItem(displayPortComboBox, port)) { /* TODO: provide error message */ }
                if (!ComboBoxHelper.SetSelectedItem(displayPinComboBox, pin)) { /* TODO: provide error message */ }

                int range = displayPinBrightnessTrackBar.Maximum - displayPinBrightnessTrackBar.Minimum;
                displayPinBrightnessTrackBar.Value = (int)((config.DisplayPinBrightness / (double)255) * (range)) + displayPinBrightnessTrackBar.Minimum;

                displayPwmCheckBox.Checked = config.DisplayPinPWM;
            }
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.DisplayPin = displayPortComboBox.Text + displayPinComboBox.Text;
            config.DisplayPinBrightness = (byte)(255 * ((displayPinBrightnessTrackBar.Value) / (double)(displayPinBrightnessTrackBar.Maximum)));
            config.DisplayPinPWM = displayPwmCheckBox.Checked;

            return config;
        }

        internal void SetModule(MobiFlightModule module)
        {
            Module = module;
        }

        private void displayPinComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Module != null)
            {
                String pin = (sender as ComboBox).SelectedItem.ToString();
                foreach (var item in Module.GetConnectedDevices(pin))
                {
                    pwmPinPanel.Visible = Module.getPwmPins().Contains((byte)(item as MobiFlightOutput).Pin);
                    return;
                }
            }
        }
    }
}
