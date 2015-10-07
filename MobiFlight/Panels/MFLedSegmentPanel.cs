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
            if (Parent != null) mfIntensityTrackBar.BackColor = Parent.BackColor;
            foreach (Int16 i in MobiFlightModuleInfo.MEGA_PINS)
            {
                mfPin1ComboBox.Items.Add(i);
                mfPin2ComboBox.Items.Add(i);
                mfPin3ComboBox.Items.Add(i);
            }
            mfPin1ComboBox.SelectedIndex = mfPin2ComboBox.SelectedIndex = mfPin3ComboBox.SelectedIndex = 0;
        }

        public MFLedSegmentPanel(Config.LedModule ledModule):this()
        {
            // TODO: Complete member initialization
            this.ledModule = ledModule;
            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, ledModule.DinPin);
            ComboBoxHelper.SetSelectedItem(mfPin2ComboBox, ledModule.ClsPin);
            ComboBoxHelper.SetSelectedItem(mfPin3ComboBox, ledModule.ClkPin);
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, ledModule.NumModules);
            textBox1.Text = ledModule.Name;
            mfIntensityTrackBar.Value = ledModule.Brightness;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            ledModule.DinPin = mfPin1ComboBox.Text;
            ledModule.ClsPin = mfPin2ComboBox.Text;
            ledModule.ClkPin = mfPin3ComboBox.Text;
            ledModule.Name = textBox1.Text;
            ledModule.Brightness = (byte)(mfIntensityTrackBar.Value);
            ledModule.NumModules = mfNumModulesComboBox.Text;

            if (Changed != null)
                Changed(ledModule, new EventArgs());
        }
    }
}
