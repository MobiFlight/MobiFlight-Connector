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
    public partial class MFDigInputMuxPanel : UserControl
    {
        private DigInputMux digInputMux;
        private bool initialized;
        public event EventHandler Changed;
        private int MAX_MODULES = 2;            // Only possible values: 1 module for HCT4051, 2 for HCT4067
        private const string NA_STRING = "N/A";

        public MFDigInputMuxPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
        }

        public MFDigInputMuxPanel(DigInputMux digInputMux, List<MobiFlightPin> Pins) : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, Pins, digInputMux.DataPin);

            if (mfPin1ComboBox.Items.Count > 2)
            {
                mfPin1ComboBox.SelectedIndex = 0;
            }

            for (int i = 1; i <= MAX_MODULES; i++)
            {
                mfNumModulesComboBox.Items.Add(i);
            }

            this.digInputMux = digInputMux;

            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, digInputMux.DataPin);
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, digInputMux.NumModules);

            textBox1.Text = digInputMux.Name;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(digInputMux, new EventArgs());
        }

        private void setValues()
        {
            digInputMux.DataPin = mfPin1ComboBox.Text;
            digInputMux.Name = textBox1.Text;
            digInputMux.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
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