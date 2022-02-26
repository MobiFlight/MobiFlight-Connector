using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFStepperPanel : UserControl
    {
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private MobiFlight.Config.Stepper stepper;
        private bool initialized = false;
        public event EventHandler Changed;

        public MFStepperPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
            mfPin4ComboBox.Items.Clear();
            mfBtnPinComboBox.Items.Clear();
        }

        public MFStepperPanel(MobiFlight.Config.Stepper stepper, List<MobiFlightPin> Pins): this()
        {
            pinList = Pins; // Keep pin list stored

            this.stepper = stepper;
            UpdateFreePinsInDropDowns();

            mfNameTextBox.Text = stepper.Name;
            autoZeroCheckBox.Checked = stepper.BtnPin == "0";
            if (stepper.BtnPin != "0")
                ComboBoxHelper.SetSelectedItem(mfBtnPinComboBox, stepper.BtnPin);
            initialized = true;
        }

        private void setNonPinValues()
        {
            stepper.Name = mfNameTextBox.Text;
        }

        private void UpdateFreePinsInDropDowns()
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, stepper.Pin1);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, pinList, stepper.Pin2);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, pinList, stepper.Pin3);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin4ComboBox, pinList, stepper.Pin4);
            ComboBoxHelper.BindMobiFlightFreePins(mfBtnPinComboBox, pinList, stepper.BtnPin);
            initialized = exInitialized;
        }

        private void ReassignFreePinsInDropDowns(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (stepper.XXXPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(mfPin1ComboBox, pinList, ref stepper.Pin1); } else
            if (comboBox == mfPin2ComboBox) { ComboBoxHelper.reassignPin(mfPin2ComboBox, pinList, ref stepper.Pin2); } else
            if (comboBox == mfPin3ComboBox) { ComboBoxHelper.reassignPin(mfPin3ComboBox, pinList, ref stepper.Pin3); } else
            if (comboBox == mfPin4ComboBox) { ComboBoxHelper.reassignPin(mfPin4ComboBox, pinList, ref stepper.Pin4); } else
            if (comboBox == mfBtnPinComboBox) { ComboBoxHelper.reassignPin(mfBtnPinComboBox, pinList, ref stepper.BtnPin); }
            // then the others are updated too 
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            ReassignFreePinsInDropDowns(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(stepper, new EventArgs());
        }

        private void autoZeroCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mfBtnPinComboBox.Enabled = !(sender as CheckBox).Checked;
        }
    }
}