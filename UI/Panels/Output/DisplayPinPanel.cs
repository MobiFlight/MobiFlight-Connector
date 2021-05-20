using System;
using System.Collections.Generic;
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

        internal void EnablePWMSelect(bool enable)
        {
            //pwmPinPanel.Visible = Module.getPwmPins().Contains((byte)(item as MobiFlightOutput).Pin);
            pwmPinPanel.Visible = enable;
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
            if (config.DisplayPin != null && config.DisplayPin != "")
            {
                string port = pinSelectPanel?.SetSelectgedPinsFromString(config.DisplayPin, config.DisplaySerial);

                // preselect normal pin drop downs
                if (!ComboBoxHelper.SetSelectedItem(displayPortComboBox, port)) { /* TODO: provide error message */ }

                int range = displayPinBrightnessTrackBar.Maximum - displayPinBrightnessTrackBar.Minimum;
                displayPinBrightnessTrackBar.Value = (int)((config.DisplayPinBrightness / (double)255) * (range)) + displayPinBrightnessTrackBar.Minimum;

                displayPwmCheckBox.Checked = config.DisplayPinPWM;
            }
        }

        virtual internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.DisplayPin = pinSelectPanel?.GetSelectedPinString();                       

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
                    pwmPinPanel.Visible = Module.getPwmPins()
                                                .Find(x => x.Pin == (byte)(item as MobiFlightOutput).Pin) != null;
                    return;
                }
            }
        }

        internal void SetPins(List<ListItem> pins)
        {
            pinSelectPanel?.SetPins(pins);            
        }
    }
}
