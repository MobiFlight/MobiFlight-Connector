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

        public MFStepperPanel()
        {
            InitializeComponent();
            for (int i = 0; i != 56; i++)
            {
                mfPin1ComboBox.Items.Add(i);
                mfPin2ComboBox.Items.Add(i);
                mfPin3ComboBox.Items.Add(i);
                mfPin4ComboBox.Items.Add(i);
            }
            mfPin1ComboBox.SelectedIndex = mfPin2ComboBox.SelectedIndex = mfPin3ComboBox.SelectedIndex = mfPin4ComboBox.SelectedIndex = 0;
        }

        public MFStepperPanel(Config.Stepper stepper)
            : this()
        {
            // TODO: Complete member initialization
            this.stepper = stepper;
            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, stepper.Pin1);
            ComboBoxHelper.SetSelectedItem(mfPin2ComboBox, stepper.Pin2);
            ComboBoxHelper.SetSelectedItem(mfPin3ComboBox, stepper.Pin3);
            ComboBoxHelper.SetSelectedItem(mfPin4ComboBox, stepper.Pin4);
            mfNameTextBox.Text = stepper.Name;
        }
        
        private void applyButton_Click(object sender, EventArgs e)
        {
            stepper.Pin1 = mfPin1ComboBox.SelectedText;
            stepper.Pin2 = mfPin1ComboBox.SelectedText;
            stepper.Pin3 = mfPin1ComboBox.SelectedText;
            stepper.Pin4 = mfPin1ComboBox.SelectedText;
            stepper.Name = mfNameTextBox.Text;
            Changed(stepper, new EventArgs());
        }
    }
}
