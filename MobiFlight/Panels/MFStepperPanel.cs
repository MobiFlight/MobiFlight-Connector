using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.Panels
{
    public partial class MFStepperPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private Config.Stepper stepper;
        bool initialized = false;

        public MFStepperPanel(List<int> usedPins)
        {
            InitializeComponent();
            foreach (Int16 i in MobiFlightModuleInfo.MEGA_PINS)
            {
                if (usedPins.IndexOf(i) == -1)
                {
                    mfPin1ComboBox.Items.Add(i);
                    mfPin2ComboBox.Items.Add(i);
                    mfPin3ComboBox.Items.Add(i);
                    mfPin4ComboBox.Items.Add(i);
                    mfBtnPinComboBox.Items.Add(i);
                }
            }
            if (mfPin1ComboBox.Items.Count > 4)
            {
                mfPin1ComboBox.SelectedIndex = 0;
                mfPin2ComboBox.SelectedIndex = 1;
                mfPin3ComboBox.SelectedIndex = 2;
                mfPin4ComboBox.SelectedIndex = 3;
                mfBtnPinComboBox.SelectedIndex = 4;
            }
        }

        public MFStepperPanel(Config.Stepper stepper, List<int> usedPins)
            : this(usedPins)
        {
            // TODO: Complete member initialization
            this.stepper = stepper;
            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, stepper.Pin1);
            ComboBoxHelper.SetSelectedItem(mfPin2ComboBox, stepper.Pin2);
            ComboBoxHelper.SetSelectedItem(mfPin3ComboBox, stepper.Pin3);
            ComboBoxHelper.SetSelectedItem(mfPin4ComboBox, stepper.Pin4);
            ComboBoxHelper.SetSelectedItem(mfPin4ComboBox, stepper.Pin4);
            ComboBoxHelper.SetSelectedItem(mfBtnPinComboBox, stepper.BtnPin);
            mfNameTextBox.Text = stepper.Name;
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
            stepper.BtnPin = mfBtnPinComboBox.Text;
            stepper.Name = mfNameTextBox.Text;
        }
    }
}