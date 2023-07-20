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
            if (!ComboBoxHelper.SetSelectedItem(customDeviceNamesComboBox, config.CustomDevice.CustomName))
            {
                Log.Instance.log("Exception on selecting item in Custom Device Name ComboBox.", LogSeverity.Error);
            }

            if (!ComboBoxHelper.SetSelectedItemByValue(MessageTypeComboBox, config.CustomDevice.MessageType))
            {
                Log.Instance.log("Exception on selecting item in Custom Device Name ComboBox.", LogSeverity.Error);
            }
            
            valueTextBox.Text = config.CustomDevice.Value;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (customDeviceNamesComboBox.SelectedValue != null)
            {
                config.CustomDevice.CustomName = (customDeviceNamesComboBox.SelectedValue as MobiFlightCustomDevice).Name.ToString ();
            }

            if (MessageTypeComboBox.SelectedValue != null)
            {
                config.CustomDevice.MessageType = (MessageTypeComboBox.SelectedValue as CustomDevices.MessageType).Id;
            }

            config.CustomDevice.Value = valueTextBox.Text;

            return config;
        }

        private void customDeviceNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var messages = new List<ListItem<CustomDevices.MessageType>>();
            var customDevice = (customDeviceNamesComboBox.SelectedValue as MobiFlightCustomDevice);

            if (customDevice == null) return;


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
