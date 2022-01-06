using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFInputShiftRegisterPanel : UserControl
    {
        private InputShiftRegister inputShiftRegister;
        private bool initialized;
        public event EventHandler Changed;
        private int MAX_MODULES = 4;
        private const string NA_STRING = "N/A";

        public MFInputShiftRegisterPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
        }

        public MFInputShiftRegisterPanel(InputShiftRegister inputShiftRegister, List<MobiFlightPin> Pins) : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, Pins, inputShiftRegister.LatchPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, Pins, inputShiftRegister.ClockPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, Pins, inputShiftRegister.DataPin);

            if (mfPin1ComboBox.Items.Count > 2)
            {
                mfPin1ComboBox.SelectedIndex = 0;
                mfPin2ComboBox.SelectedIndex = 1;
                mfPin3ComboBox.SelectedIndex = 2;
            }

            for (int i = 1; i <= MAX_MODULES; i++)
            {
                mfNumModulesComboBox.Items.Add(i);
            }

            this.inputShiftRegister = inputShiftRegister;

            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, inputShiftRegister.LatchPin);
            ComboBoxHelper.SetSelectedItem(mfPin2ComboBox, inputShiftRegister.ClockPin);
            ComboBoxHelper.SetSelectedItem(mfPin3ComboBox, inputShiftRegister.DataPin);
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, inputShiftRegister.NumModules);

            textBox1.Text = inputShiftRegister.Name;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(inputShiftRegister, new EventArgs());
        }

        private void setValues()
        {
            inputShiftRegister.LatchPin = mfPin1ComboBox.Text;
            inputShiftRegister.ClockPin = mfPin2ComboBox.Text;
            inputShiftRegister.DataPin = mfPin3ComboBox.Text;
            inputShiftRegister.Name = textBox1.Text;
            inputShiftRegister.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
        }
    }
}