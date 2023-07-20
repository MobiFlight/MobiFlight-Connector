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
        private List<ComboBox> _comboboxList = new List<ComboBox>();

        public MFCustomDevicePanel()
        {
            InitializeComponent();
            comboBox0.Items.Clear();
            _comboboxList = new List<ComboBox>() {
                { comboBox0},
                { comboBox1 },
                { comboBox2 },
                { comboBox3 },
                { comboBox4 },
                { comboBox5 }
            };
            _comboboxList.ForEach(cb => cb.SelectedIndexChanged += value_Changed);
        }

        public MFCustomDevicePanel(MobiFlight.Config.CustomDevice device, List<MobiFlightPin> Pins): this()
        {
            this.device = device;

            var deviceDefinition = CustomDeviceDefinitions.GetDeviceByType(device.CustomType);
            var i = 0;

            _comboboxList.ForEach(cb => cb.Parent.Visible = false);

            foreach (var pin in deviceDefinition.Config.Pins)
            {
                if (i == deviceDefinition.Config.Pins.Count) break;
                var currentComboBox = _comboboxList[i];
                ((currentComboBox.Parent).Controls[0] as Label).Text = deviceDefinition.Config.Pins[i];
                updateComboBox(currentComboBox, Pins, device.VirtualPins[i]);
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
            for(var i=0; i<device.VirtualPins.Count; i++)
            {
                device.VirtualPins[i] = _comboboxList[i].SelectedItem.ToString();
            }
            
            device.Name = textBox1.Text;
        }
    }
}
