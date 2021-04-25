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
    public partial class MFLedSegmentPanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private MobiFlight.Config.LedModule ledModule;
        bool initialized = false;

        public MFLedSegmentPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
            if (Parent != null) mfIntensityTrackBar.BackColor = Parent.BackColor;
        }

        public MFLedSegmentPanel(MobiFlight.Config.LedModule ledModule, List<MobiFlightPin> Pins):this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, Pins, ledModule.DinPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, Pins, ledModule.ClsPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, Pins, ledModule.ClkPin);

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
