using MobiFlight.Base;
using MobiFlight.OutputConfig;
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
            displayPinComboBox.SelectedIndexChanged += displayPinComboBox_SelectedIndexChanged;
            MultiPinSelectPanel.SelectionChanged += MultiPinSelectPanel_SelectionChanged;

            MultiPinSelectPanel.Visible = false;
            singlePinSelectFlowLayoutPanel.Visible = true;
            displayPinBrightnessPanel.Visible = displayPinBrightnessPanel.Enabled = displayPortComboBox.Visible = displayPortComboBox.Enabled = false;
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

            if (Module != null && pins.Count > 1)
            {
                // this is MobiFlight Outputs
                _MultiSelectOptions(true);
            }
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

            String serial = config.ModuleSerial;
            serial = SerialNumber.ExtractSerial(serial);

            if (!(config.Device is Output)) return;

            string port = "";
            var cfg = config.Device as Output;
            string pin = cfg.DisplayPin;

            if (SerialNumber.IsJoystickSerial(serial) ||
                SerialNumber.IsMidiBoardSerial(serial))                                   
            {
                // disable multi-select option
                _MultiSelectOptions(false);
                pin = cfg.DisplayPin;
            }
            else if (SerialNumber.IsArcazeSerial(serial))
            {
                // these are Arcaze Boards.
                // Arcaze Boards only have "single output"
                port = cfg.DisplayPin.Substring(0, 1);
                pin = cfg.DisplayPin.Substring(1);

                // disable multi-select option
                _MultiSelectOptions(false);
            } else if (SerialNumber.IsMobiFlightSerial(serial)) {

                // this is MobiFlight Outputs
                _MultiSelectOptions(true);

                // initialize multi-select panel
                MultiPinSelectPanel?.SetSelectedPinsFromString(cfg.DisplayPin, config.ModuleSerial);

                // get the first from the multi select
                pin = cfg.DisplayPin.Split(Panels.PinSelectPanel.POSITION_SEPERATOR)[0];

                selectMultiplePinsCheckBox.Checked = cfg.DisplayPin.Split(Panels.PinSelectPanel.POSITION_SEPERATOR).Length > 1;
            }

            // preselect normal pin drop downs
            if (!ComboBoxHelper.SetSelectedItem(displayPortComboBox, port)) { /* TODO: provide error message */ }

            if (displayPinComboBox.Items.Count == 0) {
                displayPinComboBox.DataSource = new List<ListItem>() { new ListItem() { Label = pin, Value = pin } };
            }

            if (!ComboBoxHelper.SetSelectedItem(displayPinComboBox, pin)) { /* TODO: provide error message */ }

            int range = displayPinBrightnessTrackBar.Maximum - displayPinBrightnessTrackBar.Minimum;
            displayPinBrightnessTrackBar.Value = (int)((cfg.DisplayPinBrightness / (double)255) * (range)) + displayPinBrightnessTrackBar.Minimum;

            displayPwmCheckBox.Checked = cfg.DisplayPinPWM;
        }

        private void _MultiSelectOptions(bool state)
        {
            MultiSelectSupport = state;
            selectMultiplePinsCheckBox.Visible = state;
        }

        virtual internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            var cfg = new Output()
            {
                DisplayPin = displayPortComboBox.Text + displayPinComboBox.Text,
            };

            if (selectMultiplePinsCheckBox.Checked)
                cfg.DisplayPin = MultiPinSelectPanel?.GetSelectedPinString();                       

            cfg.DisplayPinBrightness = (byte)(255 * ((displayPinBrightnessTrackBar.Value) / (double)(displayPinBrightnessTrackBar.Maximum)));
            cfg.DisplayPinPWM = pwmPinPanel.Enabled && displayPwmCheckBox.Checked;

            config.Device = cfg;
            return config;
        }

        internal void SetModule(MobiFlightModule module)
        {
            Module = module;
        }

        private void displayPinComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Module == null) return;

            String pin = (sender as ComboBox).SelectedItem.ToString();
            foreach (var item in Module.GetConnectedDevices(pin))
            {
                pwmPinPanel.Enabled = pwmPinPanel.Visible 
                                    = Module.getPwmPins()
                                            .Find(x => x.Pin == (uint)(item as MobiFlightOutput).Pin) != null;
                return;
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

        private void MultiPinSelectPanel_SelectionChanged(object sender, List<ListItem> selectedPins)
        {
            pwmPinPanel.Enabled = false;

            if (Module == null) return;

            var pwmPins = Module.getPwmPins();
            pwmPinPanel.Enabled = pwmPinPanel.Visible
                                = selectedPins.All(
                                    pin => pwmPins.Find(
                                        pwmPin => pwmPin.Pin == (Module.GetConnectedDevices(pin.Value).First() as MobiFlightOutput).Pin
                                    ) != null);
        }
    }
}
