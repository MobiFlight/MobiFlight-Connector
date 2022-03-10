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
    public partial class MFButtonPanel : UserControl
    {
        private MobiFlight.Config.Button button;
        private bool initialized = false;

        public event EventHandler Changed;

        public MFButtonPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFButtonPanel(MobiFlight.Config.Button button, List<MobiFlightPin> Pins): this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, Pins, button.Pin);

            this.button = button;
            mfPinComboBox.SelectedValue = byte.Parse(button.Pin);
            textBox1.Text = button.Name;
            ////setValues();

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            setValues();
            if (Changed != null)
                Changed(button, new EventArgs());
        }

        private void setValues()
        {
            button.Pin = mfPinComboBox.SelectedItem.ToString();
            button.Name = textBox1.Text;
        }
    }
}
