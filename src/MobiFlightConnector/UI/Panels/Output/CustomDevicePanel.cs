using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.OutputConfig;
using System.Net.NetworkInformation;

namespace MobiFlight.UI.Panels
{
    public partial class CustomDevicePanel : UserControl
    {
        public CustomDevicePanel()
        {
            InitializeComponent();
        }

        public void SetCustomDeviceNames(List<ListItem<MobiFlightCustomDevice>> pins)
        {
            customDeviceNamesComboBox.ValueMember = "Value";
            customDeviceNamesComboBox.DisplayMember = "Label";
            customDeviceNamesComboBox.DataSource = pins;


            if (pins.Count > 0)
                customDeviceNamesComboBox.SelectedIndex = 0;

            customDeviceNamesComboBox.Enabled = pins.Count > 0;
        }

        public void syncFromConfig(OutputConfigItem config)
        {
            if (!(config.Device is CustomDevice)) return;

            var customDevice = config.Device as CustomDevice;
            if (!ComboBoxHelper.SetSelectedItem(customDeviceNamesComboBox, customDevice.CustomName))
            {
                Log.Instance.log("Exception on selecting item in Custom Device Name ComboBox.", LogSeverity.Error);
            }

            if (!ComboBoxHelper.SetSelectedItemByValue(MessageTypeComboBox, customDevice.MessageType.ToString()))
            {
                Log.Instance.log("Exception on selecting item in Custom Device Name ComboBox.", LogSeverity.Error);
            }

            valueTextBox.Text = customDevice.Value;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.Device = new CustomDevice();
            var customDevice = config.Device as CustomDevice;

            if (customDeviceNamesComboBox.SelectedValue != null)
            {
                customDevice.CustomName = (customDeviceNamesComboBox.SelectedValue as MobiFlightCustomDevice).Name.ToString();
            }

            if (MessageTypeComboBox.SelectedValue != null)
            {
                customDevice.MessageType = (MessageTypeComboBox.SelectedValue as CustomDevices.MessageType).Id;
            }

            customDevice.Value = valueTextBox.Text;

            return config;
        }

        private void customDeviceNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var messages = new List<ListItem<CustomDevices.MessageType>>();
            var customDevice = (customDeviceNamesComboBox.SelectedValue as MobiFlightCustomDevice);

            if (customDevice == null || customDevice.CustomDevice == null) 
                return;


            customDevice.CustomDevice
                        .MessageTypes
                        .ForEach(
                            (m) => { messages.Add(new ListItem<CustomDevices.MessageType>() { Value = m, Label = m.Label }); }
                        );

            MessageTypeComboBox.DisplayMember = "Label";
            MessageTypeComboBox.ValueMember = "Value";
            MessageTypeComboBox.DataSource = messages;

            if (messages.Count > 0)
                MessageTypeComboBox.SelectedIndex = 0;

            MessageTypeComboBox.Enabled = messages.Count > 0;
        }

        private void MessageTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var message = MessageTypeComboBox.SelectedValue as CustomDevices.MessageType;

            if (message == null) return;

            MessageDescriptionLabel.Text = message.Description;
        }
    }
}
