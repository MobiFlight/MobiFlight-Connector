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
    public partial class MFShiftRegisterPanel : UserControl
    {
        private ShiftRegister shiftRegister;
        private bool initialized;
        public event EventHandler Changed;
        private int MAX_MODULES = 4;
        private const string NA_STRING = "N/A";

        public MFShiftRegisterPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
        }

        public MFShiftRegisterPanel(ShiftRegister shiftRegister, List<MobiFlightPin> Pins) : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, Pins, shiftRegister.LatchPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, Pins, shiftRegister.ClockPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, Pins, shiftRegister.DataPin);

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

            // TODO: Complete member initialization
            this.shiftRegister = shiftRegister;

            mfPin1ComboBox.SelectedValue = byte.Parse(shiftRegister.LatchPin);
            mfPin2ComboBox.SelectedValue = byte.Parse(shiftRegister.ClockPin);
            mfPin3ComboBox.SelectedValue = byte.Parse(shiftRegister.DataPin);
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, shiftRegister.NumModules);
            
            textBox1.Text = shiftRegister.Name;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(shiftRegister, new EventArgs());
        }

        private void setValues()
        {
            shiftRegister.LatchPin = mfPin1ComboBox.SelectedItem.ToString();
            shiftRegister.ClockPin = mfPin2ComboBox.SelectedItem.ToString();
            shiftRegister.DataPin = mfPin3ComboBox.SelectedItem.ToString();           
            shiftRegister.Name = textBox1.Text;
            shiftRegister.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text)?"1": mfNumModulesComboBox.Text;
        }

        static public bool BindMobiFlightFreePins(ComboBox comboBox, List<MobiFlightPin> Pins, String CurrentPin)
        {
            List<MobiFlightPin> UsablePins = Pins.ConvertAll(pin => new MobiFlightPin(pin));
            if (UsablePins.Exists(x => x.Pin == byte.Parse(CurrentPin)))
                UsablePins.Find(x => x.Pin == byte.Parse(CurrentPin)).Used = false;

            comboBox.DataSource = UsablePins.FindAll(x => x.Used == false);
            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "Pin";

            return false;
        }

    }
}
