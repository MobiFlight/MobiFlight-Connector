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
            ////ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, Pins, shiftRegister.LatchPin);
            ////ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, Pins, shiftRegister.ClockPin);
            ////ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, Pins, shiftRegister.DataPin);
            update_lists();

            ////if (mfPin1ComboBox.Items.Count > 2)
            ////{
            ////    mfPin1ComboBox.SelectedIndex = 0;
            ////    mfPin2ComboBox.SelectedIndex = 1;
            ////    mfPin3ComboBox.SelectedIndex = 2;                
            ////}

            for (int i = 1; i <= MAX_MODULES; i++) {
                mfNumModulesComboBox.Items.Add(i);
            }

            ////mfPin1ComboBox.SelectedValue = byte.Parse(shiftRegister.LatchPin);
            ////mfPin2ComboBox.SelectedValue = byte.Parse(shiftRegister.ClockPin);
            ////mfPin3ComboBox.SelectedValue = byte.Parse(shiftRegister.DataPin);
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, shiftRegister.NumModules);
            textBox1.Text = shiftRegister.Name;

            initialized = true;
        }

        private void setNonPinValues()
        {
            ////shiftRegister.LatchPin = mfPin1ComboBox.SelectedItem.ToString();
            ////shiftRegister.ClockPin = mfPin2ComboBox.SelectedItem.ToString();
            ////shiftRegister.DataPin = mfPin3ComboBox.SelectedItem.ToString();
            shiftRegister.Name = textBox1.Text;
            shiftRegister.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
        }
        private void update_lists()
        {
            bool ex_initialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, shiftRegister.LatchPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, pinList, shiftRegister.ClockPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, pinList, shiftRegister.DataPin);
            initialized = ex_initialized;
        }

        private void update_all(ComboBox comboBox)
        {
            bool ex_initialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (shiftRegister.XXXPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(mfPin1ComboBox, pinList, ref shiftRegister.LatchPin); } else
            if (comboBox == mfPin2ComboBox) { ComboBoxHelper.reassignPin(mfPin2ComboBox, pinList, ref shiftRegister.ClockPin); } else
            if (comboBox == mfPin3ComboBox) { ComboBoxHelper.reassignPin(mfPin3ComboBox, pinList, ref shiftRegister.DataPin); }
            // then the others are updated too 
            update_lists();

            initialized = ex_initialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            update_all(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(shiftRegister, new EventArgs());
        }
    }
}
