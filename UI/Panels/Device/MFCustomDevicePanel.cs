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
            groupBox1.Controls.Clear();
        }

        public MFCustomDevicePanel(MobiFlight.Config.CustomDevice device, List<MobiFlightPin> Pins): this()
        {
            this.device = device;

            var deviceDefinition = CustomDeviceDefinitions.GetDeviceByType(device.CustomType);
            var i = 0;

            foreach (var pin in deviceDefinition.Config.Pins)
            {
                if (i == deviceDefinition.Config.Pins.Count) break;

                var currentComboBox = new MFCustomDevicePanelPin(deviceDefinition.Config.Pins[i], Pins, device.VirtualPins[i]);
                currentComboBox.Changed += value_Changed;
                currentComboBox.Dock = DockStyle.Bottom;
                groupBox1.Controls.Add(currentComboBox);
                i++;
            }

            textBox1.Text = device.Name;
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
            for(var i=0; i<device.VirtualPins.Count; i++)
            {
                device.VirtualPins[i] = (groupBox1.Controls[i] as MFCustomDevicePanelPin).SelectedPin().ToString();
            }
            
            device.Name = textBox1.Text;
        }
    }
}
