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
            mfPinComboBox.Items.Clear();
        }

        public MFCustomDevicePanel(MobiFlight.Config.CustomDevice device, List<MobiFlightPin> Pins): this()
        {
            this.device = device;

            var deviceDefinition = CustomDeviceDefinitions.GetDeviceByType(device.CustomType);
            var i = 0;
            var comboBoxes = new Dictionary<ComboBox, String>() {
                { mfPinComboBox, device.Pin1},
                { comboBox1, device.Pin2},
                { comboBox2, device.Pin3},
                { comboBox3, device.Pin4},
                { comboBox4, device.Pin5},
                { comboBox5, device.Pin6}
            };

            foreach (var key in comboBoxes.Keys)
            {
                (key.Parent).Visible = false;
            }

            foreach (var pin in deviceDefinition.Config.Pins)
            {
                if (i == deviceDefinition.Config.Pins.Count) break;
                var key = comboBoxes.Keys.ToArray()[i];
                ((key.Parent).Controls[0] as Label).Text = deviceDefinition.Config.Pins[i];
                updateComboBox(key, Pins, comboBoxes[key]);
                i++;
            }

            textBox1.Text = device.Name;
            initialized = true;
        }

        private void updateComboBox(ComboBox comboBox, List<MobiFlightPin> Pins, string pin) {
            ComboBoxHelper.BindMobiFlightFreePins(comboBox, Pins, pin);
            (comboBox.Parent).Visible = true;
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
            device.Pin1 = mfPinComboBox.SelectedItem.ToString();
            device.Name = textBox1.Text;
        }
    }
}
