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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void SetSelectedAddress(string value)
        {
            MessageTypeComboBox.SelectedValue = value;
        }

        public void SetCustomDeviceNames(List<ListItem<MobiFlightCustomDevice>> pins)
        {
            //addressesComboBox.SelectedValueChanged -= addressesComboBox_SelectedValueChanged;
            customDeviceNamesComboBox.ValueMember = "Value"; 
            customDeviceNamesComboBox.DisplayMember = "Label";
            customDeviceNamesComboBox.DataSource = pins;


            if (pins.Count > 0)
                customDeviceNamesComboBox.SelectedIndex = 0;

            customDeviceNamesComboBox.Enabled = pins.Count > 0;

            //addressesComboBox.SelectedValueChanged += addressesComboBox_SelectedValueChanged;
        }

        public void syncFromConfig(OutputConfigItem config)
        {
            if (!ComboBoxHelper.SetSelectedItem(customDeviceNamesComboBox, config.CustomDevice.Name))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in Custom Device Name ComboBox.", LogSeverity.Error);
            }

            if (!ComboBoxHelper.SetSelectedItemByValue(MessageTypeComboBox, config.CustomDevice.MessageType))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in Custom Device Name ComboBox.", LogSeverity.Error);
            }

        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (customDeviceNamesComboBox.SelectedValue != null)
            {
                config.CustomDevice.Name = (customDeviceNamesComboBox.SelectedValue as MobiFlightCustomDevice).Name.ToString ();
            }

            if (MessageTypeComboBox.SelectedValue != null)
            {
                config.CustomDevice.MessageType = (MessageTypeComboBox.SelectedValue as CustomDevices.MessageType).Id;
            }

            return config;
        }

        private void addressesComboBox_SelectedValueChanged(object sender, EventArgs e)
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
