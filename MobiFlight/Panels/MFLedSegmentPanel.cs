using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class MFLedSegmentPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private Config.LedModule ledModule;
        bool initialized = false;

        public MFLedSegmentPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
            if (Parent != null) mfIntensityTrackBar.BackColor = Parent.BackColor;
        }

        public MFLedSegmentPanel(Config.LedModule ledModule, List<byte> FreePins):this()
        {
            List<byte> Pin1Pins = FreePins.ToList(); if (Int16.Parse(ledModule.DinPin) > 0) Pin1Pins.Add(Byte.Parse(ledModule.DinPin)); Pin1Pins.Sort();
            List<byte> Pin2Pins = FreePins.ToList(); if (Int16.Parse(ledModule.ClsPin) > 0) Pin2Pins.Add(Byte.Parse(ledModule.ClsPin)); Pin2Pins.Sort();
            List<byte> Pin3Pins = FreePins.ToList(); if (Int16.Parse(ledModule.ClkPin) > 0) Pin3Pins.Add(Byte.Parse(ledModule.ClkPin)); Pin3Pins.Sort();

            foreach (byte pin in Pin1Pins) mfPin1ComboBox.Items.Add(pin);
            foreach (byte pin in Pin2Pins) mfPin2ComboBox.Items.Add(pin);
            foreach (byte pin in Pin3Pins) mfPin3ComboBox.Items.Add(pin);

            if (mfPin1ComboBox.Items.Count > 2)
            {
                mfPin1ComboBox.SelectedIndex = 0;
                mfPin2ComboBox.SelectedIndex = 1;
                mfPin3ComboBox.SelectedIndex = 2;
            }
            
            // TODO: Complete member initialization
            this.ledModule = ledModule;

            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, ledModule.DinPin);
            ComboBoxHelper.SetSelectedItem(mfPin2ComboBox, ledModule.ClsPin);
            ComboBoxHelper.SetSelectedItem(mfPin3ComboBox, ledModule.ClkPin);

            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, ledModule.NumModules);

            textBox1.Text = ledModule.Name;
            mfIntensityTrackBar.Value = ledModule.Brightness;
            //setValues();

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(ledModule, new EventArgs());
        }

        private void setValues()
        {
            ledModule.DinPin = mfPin1ComboBox.Text;
            ledModule.ClsPin = mfPin2ComboBox.Text;
            ledModule.ClkPin = mfPin3ComboBox.Text;
            ledModule.Name = textBox1.Text;
            ledModule.Brightness = (byte)(mfIntensityTrackBar.Value);
            ledModule.NumModules = mfNumModulesComboBox.Text;
        }
    }
}
