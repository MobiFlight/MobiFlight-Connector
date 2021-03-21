using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFShiftRegisterPanel : UserControl
    {
        private ShiftRegister shiftRegister;
        private bool initialized;
        public event EventHandler Changed;
        private int MAX_MODULES = 4;
        private const string NA_STRING = "N/A";

        public MFShiftRegisterPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
        }

        public MFShiftRegisterPanel(ShiftRegister shiftRegister, List<byte> FreePins) : this()
        {
            List<byte> Pin1Pins = FreePins.ToList(); if (Int16.Parse(shiftRegister.LatchPin) > 0) Pin1Pins.Add(Byte.Parse(shiftRegister.LatchPin)); Pin1Pins.Sort();
            List<byte> Pin2Pins = FreePins.ToList(); if (Int16.Parse(shiftRegister.ClockPin) > 0) Pin2Pins.Add(Byte.Parse(shiftRegister.ClockPin)); Pin2Pins.Sort();
            List<byte> Pin3Pins = FreePins.ToList(); if (Int16.Parse(shiftRegister.DataPin) > 0) Pin3Pins.Add(Byte.Parse(shiftRegister.DataPin)); Pin3Pins.Sort();
            
            short pinInt;
            Int16.TryParse(shiftRegister.PWMPin, out pinInt);
            List<byte> Pin4Pins = FreePins.ToList(); if (pinInt > 0) Pin4Pins.Add(Byte.Parse(shiftRegister.PWMPin)); Pin4Pins.Sort();            

            foreach (byte pin in Pin1Pins) mfPin1ComboBox.Items.Add(pin);
            foreach (byte pin in Pin2Pins) mfPin2ComboBox.Items.Add(pin);
            foreach (byte pin in Pin3Pins) mfPin3ComboBox.Items.Add(pin);
            foreach (byte pin in Pin4Pins) mfPinPWMComboBox.Items.Add(pin);
            mfPinPWMComboBox.Items.Insert(0, NA_STRING);

            if (mfPin1ComboBox.Items.Count > 2)
            {
                mfPin1ComboBox.SelectedIndex = 0;
                mfPin2ComboBox.SelectedIndex = 1;
                mfPin3ComboBox.SelectedIndex = 2;                
            }
            // Default to N/A
            mfPinPWMComboBox.SelectedIndex = 0;

            toolTip1.SetToolTip(pwmLabel, "Some Shift-Registers (LED Drivers) support setting the brightness using a PWM pin. Set to N/A if not used.");

            for (int i = 1; i <= MAX_MODULES; i++)
            {
                mfNumModulesComboBox.Items.Add(i);
            }

            // TODO: Complete member initialization
            this.shiftRegister = shiftRegister;

            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, shiftRegister.LatchPin);
            ComboBoxHelper.SetSelectedItem(mfPin2ComboBox, shiftRegister.ClockPin);
            ComboBoxHelper.SetSelectedItem(mfPin3ComboBox, shiftRegister.DataPin);
            ComboBoxHelper.SetSelectedItem(mfPinPWMComboBox, shiftRegister.PWMPin);
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, shiftRegister.NumModules);
            
            textBox1.Text = shiftRegister.Name;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(shiftRegister, new EventArgs());
        }

        private void setValues()
        {
            shiftRegister.LatchPin = mfPin1ComboBox.Text;
            shiftRegister.ClockPin = mfPin2ComboBox.Text;
            shiftRegister.DataPin = mfPin3ComboBox.Text;
            if (!mfPinPWMComboBox.Text.Equals(NA_STRING))
            {
                int isIntValue;
                if (int.TryParse(mfPinPWMComboBox.Text, out isIntValue))
                {
                    shiftRegister.PWMPin = mfPinPWMComboBox.Text;
                }               
            } else
            {
                shiftRegister.PWMPin = "-1";
            }
            shiftRegister.Name = textBox1.Text;
            shiftRegister.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text)?"1": mfNumModulesComboBox.Text;
        }

    }
}
