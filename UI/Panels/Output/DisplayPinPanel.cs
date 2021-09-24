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
        public bool MultiSelectSupport = false;
        private MobiFlightModule Module;

        public DisplayPinPanel()
        {
            InitializeComponent();
            displayPortComboBox.SelectedIndexChanged += displayPortComboBox_SelectedIndexChanged;

            MultiPinSelectPanel.Visible = false;
            singlePinSelectFlowLayoutPanel.Visible = true;
            PinSelectContainer.Height = singlePinSelectFlowLayoutPanel.Height;
        }

        public void SetSelectedPort(string value)
        {
            displayPortComboBox.SelectedValue = value;
        }

        public void SetSelectedPin(string value)
        {
            displayPinComboBox.SelectedValue = value;
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

            // disable all the arcaze specific stuff
            // when there are no ports, because then
            // we are in the context of MobiFlight
            displayPinBrightnessPanel.Visible = displayPinBrightnessPanel.Enabled = displayPortComboBox.Visible = displayPortComboBox.Enabled = ports.Count > 0;
            
        }
        internal void SetPins(List<ListItem> pins)
        {
            displayPinComboBox.DataSource = new List<ListItem>(pins);
            displayPinComboBox.DisplayMember = "Label";
            displayPinComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                displayPinComboBox.SelectedIndex = 0;

            displayPinComboBox.Enabled = pins.Count > 0;
            displayPinComboBox.Width = WideStyle ? displayPinComboBox.MaximumSize.Width : displayPinComboBox.MinimumSize.Width;


            MultiPinSelectPanel?.SetPins(pins);
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

            if (config.Pin.DisplayPin != null && config.Pin.DisplayPin != "")
            {
                string port = "";
                string pin = config.Pin.DisplayPin;

                if (serial != null && serial.IndexOf("SN") != 0)
                {
                    // these are Arcaze Boards.
                    // Arcaze Boards only have "single output"
                    port = config.Pin.DisplayPin.Substring(0, 1);
                    pin = config.Pin.DisplayPin.Substring(1);

                    // disable multi-select option
                    _MultiSelectOptions(false);
                } else {

                    // this is MobiFlight Outputs
                    _MultiSelectOptions(true);

                    // initialize multi-select panel
                    MultiPinSelectPanel?.SetSelectedPinsFromString(config.Pin.DisplayPin, config.DisplaySerial);

                    // get the first from the multi select
                    pin = config.Pin.DisplayPin.Split(Panels.PinSelectPanel.POSITION_SEPERATOR)[0];

                    selectMultiplePinsCheckBox.Checked = config.Pin.DisplayPin.Split(Panels.PinSelectPanel.POSITION_SEPERATOR).Length > 1;
                }

                // preselect normal pin drop downs
                if (!ComboBoxHelper.SetSelectedItem(displayPortComboBox, port)) { /* TODO: provide error message */ }
                if (!ComboBoxHelper.SetSelectedItem(displayPinComboBox, pin)) { /* TODO: provide error message */ }

                int range = displayPinBrightnessTrackBar.Maximum - displayPinBrightnessTrackBar.Minimum;
                displayPinBrightnessTrackBar.Value = (int)((config.Pin.DisplayPinBrightness / (double)255) * (range)) + displayPinBrightnessTrackBar.Minimum;

                displayPwmCheckBox.Checked = config.Pin.DisplayPinPWM;
            }
        }

        private void _MultiSelectOptions(bool state)
        {
            MultiSelectSupport = state;
            selectMultiplePinsCheckBox.Visible = state;
        }

        virtual internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.Pin.DisplayPin = displayPortComboBox.Text + displayPinComboBox.Text;
            
            if (selectMultiplePinsCheckBox.Checked)
                config.Pin.DisplayPin = MultiPinSelectPanel?.GetSelectedPinString();                       

            config.Pin.DisplayPinBrightness = (byte)(255 * ((displayPinBrightnessTrackBar.Value) / (double)(displayPinBrightnessTrackBar.Maximum)));

            config.Pin.DisplayPinPWM = pwmPinPanel.Enabled && displayPwmCheckBox.Checked;

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
                    pwmPinPanel.Enabled = pwmPinPanel.Visible = Module.getPwmPins()
                                                .Find(x => x.Pin == (byte)(item as MobiFlightOutput).Pin) != null;
                    return;
                }
            }
        }

        private void selectMultiplePinsCheckBox_CheckedChanged(object sender, EventArgs e)
        { 
            if ((sender as CheckBox).Checked)
            {
                MultiPinSelectPanel.Visible = true;
                singlePinSelectFlowLayoutPanel.Visible = false;
                PinSelectContainer.Height = MultiPinSelectPanel.Height;
            } else
            {
                MultiPinSelectPanel.Visible = false;
                singlePinSelectFlowLayoutPanel.Visible = true;
                PinSelectContainer.Height = singlePinSelectFlowLayoutPanel.Height;
            }

        }
    }
}
