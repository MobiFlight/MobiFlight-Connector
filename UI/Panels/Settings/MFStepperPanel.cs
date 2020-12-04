using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFStepperPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private MobiFlight.Config.Stepper stepper;
        bool initialized = false;

        public MFStepperPanel(List<byte> FreePins)
        {
            InitializeComponent();

            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
            mfPin4ComboBox.Items.Clear();
            mfBtnPinComboBox.Items.Clear();
        }

        public MFStepperPanel(MobiFlight.Config.Stepper stepper, List<byte> FreePins)
            : this(FreePins)
        {
            List<byte> Pin1Pins = FreePins.ToList(); Pin1Pins.Add(Convert.ToByte(stepper.Pin1)); Pin1Pins.Sort();
            foreach (byte pin in Pin1Pins) mfPin1ComboBox.Items.Add(pin);

            List<byte> Pin2Pins = FreePins.ToList(); Pin2Pins.Add(Convert.ToByte(stepper.Pin2)); Pin2Pins.Sort();
            foreach (byte pin in Pin2Pins) mfPin2ComboBox.Items.Add(pin);

            List<byte> Pin3Pins = FreePins.ToList(); Pin3Pins.Add(Convert.ToByte(stepper.Pin3)); Pin3Pins.Sort();
            foreach (byte pin in Pin3Pins) mfPin3ComboBox.Items.Add(pin);

            List<byte> Pin4Pins = FreePins.ToList(); Pin4Pins.Add(Convert.ToByte(stepper.Pin4)); Pin4Pins.Sort();
            foreach (byte pin in Pin4Pins) mfPin4ComboBox.Items.Add(pin);

            List<byte> Pin5Pins = FreePins.ToList();
            if (stepper.BtnPin!="0") Pin5Pins.Add(Convert.ToByte(stepper.BtnPin));
            Pin5Pins.Sort();
            foreach (byte pin in Pin5Pins)
            {
                mfBtnPinComboBox.Items.Add(pin);
            }
                       
            if (mfPin1ComboBox.Items.Count > 4)
            {
                mfPin1ComboBox.SelectedIndex = 0;
                mfPin2ComboBox.SelectedIndex = 1;
                mfPin3ComboBox.SelectedIndex = 2;
                mfPin4ComboBox.SelectedIndex = 3;
                mfBtnPinComboBox.SelectedIndex = 4;
            }

            // TODO: Complete member initialization
            this.stepper = stepper;

            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, stepper.Pin1);
            ComboBoxHelper.SetSelectedItem(mfPin2ComboBox, stepper.Pin2);
            ComboBoxHelper.SetSelectedItem(mfPin3ComboBox, stepper.Pin3);
            ComboBoxHelper.SetSelectedItem(mfPin4ComboBox, stepper.Pin4);
            ComboBoxHelper.SetSelectedItem(mfPin4ComboBox, stepper.Pin4);

            mfNameTextBox.Text = stepper.Name;

            autoZeroCheckBox.Checked = stepper.BtnPin == "0";
            if (stepper.BtnPin != "0")
                ComboBoxHelper.SetSelectedItem(mfBtnPinComboBox, stepper.BtnPin);

            setValues();

            initialized = true;
        }
        
        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(stepper, new EventArgs());
        }

        private void setValues()
        {
            stepper.Pin1 = mfPin1ComboBox.Text;
            stepper.Pin2 = mfPin2ComboBox.Text;
            stepper.Pin3 = mfPin3ComboBox.Text;
            stepper.Pin4 = mfPin4ComboBox.Text;
            stepper.BtnPin = autoZeroCheckBox.Checked ? "0" : mfBtnPinComboBox.Text;
            stepper.Name = mfNameTextBox.Text;
        }

        private void autoZeroCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mfBtnPinComboBox.Enabled = !(sender as CheckBox).Checked;
            setValues();
        }
    }
}