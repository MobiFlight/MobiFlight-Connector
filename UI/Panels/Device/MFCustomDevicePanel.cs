using MobiFlight.CustomDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFCustomDevicePanel : UserControl
    {

        private MobiFlight.Config.CustomDevice device;
        private bool initialized = false;
        
        public event EventHandler Changed;

        public MFCustomDevicePanel()
        {
            InitializeComponent();
            groupBoxPinSettings.Controls.Clear();
        }

        public MFCustomDevicePanel(MobiFlight.Config.CustomDevice device, List<MobiFlightPin> Pins): this()
        {
            this.device = device;

            var deviceDefinition = CustomDeviceDefinitions.GetDeviceByType(device.CustomType);
            var PinsI2C = new List<MobiFlightPin>();
            var labels = deviceDefinition.Config.Pins;
            var i2cEnabled = deviceDefinition.Config.I2C?.Enabled ?? false;

            if (i2cEnabled)
            {
                labels = new List<string>() { "Address" };
                deviceDefinition.Config.I2C.Addresses.ForEach(p => PinsI2C.Add(new MobiFlightPin() { 
                    Name = p, 
                    isI2C = true, 
                    Pin = byte.Parse(p.Replace("0x",""), System.Globalization.NumberStyles.AllowHexSpecifier) 
                }));                
            }

            var i = 0;
            foreach (var pin in labels)
            {
                if (!i2cEnabled && i == deviceDefinition.Config.Pins.Count) break;

                var currentComboBox = new MFCustomDevicePanelPin(
                    pin,
                    i2cEnabled ? PinsI2C : Pins,
                    device.ConfiguredPins[i]
                );
                currentComboBox.Changed += value_Changed;
                currentComboBox.Dock = DockStyle.Bottom;
                groupBoxPinSettings.Controls.Add(currentComboBox);
                i++;
            }

            textBoxName.Text = device.Name;
            textBoxName.TextChanged += value_Changed;

            groupBoxAdditionalConfig.Visible = false;

            if (deviceDefinition.Config.Custom.Enabled)
            {
                textBoxAdditionalConfig.TextChanged += value_Changed;
                textBoxAdditionalConfig.Text = device.Config;
                groupBoxAdditionalConfig.Visible = true;
            }

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();
            if (Changed != null)
                Changed(device, new EventArgs());
        }

        private void setValues()
        {   
            for(var i=0; i<device.ConfiguredPins.Count; i++)
            {
                device.ConfiguredPins[i] = (groupBoxPinSettings.Controls[i] as MFCustomDevicePanelPin).SelectedPin().ToString();
            }
            
            device.Name = textBoxName.Text;
            device.Config = textBoxAdditionalConfig.Text;
        }
    }
}
