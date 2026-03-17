using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFShiftRegisterPanel : UserControl
    {
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private ShiftRegister shiftRegister;
        private bool initialized;
        private int MAX_MODULES = 4;
        private const string NA_STRING = "N/A";
        
        public event EventHandler Changed;

        public MFShiftRegisterPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
        }

        public MFShiftRegisterPanel(ShiftRegister shiftRegister, List<MobiFlightPin> Pins) : this()
        {
            pinList = Pins; // Keep pin list stored

            this.shiftRegister = shiftRegister;
            UpdateFreePinsInDropDowns();

            for (int i = 1; i <= MAX_MODULES; i++) {
                mfNumModulesComboBox.Items.Add(i);
            }

            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, shiftRegister.NumModules);
            textBox1.Text = shiftRegister.Name;

            initialized = true;
        }

        private void setNonPinValues()
        {
            shiftRegister.Name = textBox1.Text;
            shiftRegister.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
        }
        private void UpdateFreePinsInDropDowns()
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, shiftRegister.LatchPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, pinList, shiftRegister.ClockPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, pinList, shiftRegister.DataPin);
            initialized = exInitialized;
        }

        private void ReassignFreePinsInDropDowns(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (shiftRegister.XXXPin) is updated with the new value read from the changed ComboBox;
            var newPin = comboBox.SelectedItem.ToString();
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref shiftRegister.LatchPin); } else
            if (comboBox == mfPin2ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref shiftRegister.ClockPin); } else
            if (comboBox == mfPin3ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref shiftRegister.DataPin); }
            // then the others are updated too 
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            if (sender is ComboBox)
                ReassignFreePinsInDropDowns(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(shiftRegister, new EventArgs());
        }
    }
}
