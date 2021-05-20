using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFStepperPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private MobiFlight.Config.Stepper stepper;
        bool initialized = false;

        public MFStepperPanel()
        {
            InitializeComponent();

            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
            mfPin4ComboBox.Items.Clear();
            mfBtnPinComboBox.Items.Clear();
        }

        public MFStepperPanel(MobiFlight.Config.Stepper stepper, List<MobiFlightPin> Pins)
            : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, Pins, stepper.Pin1);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, Pins, stepper.Pin2);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, Pins, stepper.Pin3);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin4ComboBox, Pins, stepper.Pin4);
            ComboBoxHelper.BindMobiFlightFreePins(mfBtnPinComboBox, Pins, stepper.BtnPin);
                       
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
            mfPin1ComboBox.SelectedValue = byte.Parse(stepper.Pin1);
            mfPin2ComboBox.SelectedValue = byte.Parse(stepper.Pin2);
            mfPin3ComboBox.SelectedValue = byte.Parse(stepper.Pin3);
            mfPin4ComboBox.SelectedValue = byte.Parse(stepper.Pin4);

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
            stepper.Pin1 = mfPin1ComboBox.SelectedItem.ToString();
            stepper.Pin2 = mfPin2ComboBox.SelectedItem.ToString();
            stepper.Pin3 = mfPin3ComboBox.SelectedItem.ToString();
            stepper.Pin4 = mfPin4ComboBox.SelectedItem.ToString();
            stepper.BtnPin = autoZeroCheckBox.Checked ? "0" : mfBtnPinComboBox.SelectedItem.ToString();
            stepper.Name = mfNameTextBox.Text;
        }

        private void autoZeroCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mfBtnPinComboBox.Enabled = !(sender as CheckBox).Checked;
            setValues();
        }
    }
}