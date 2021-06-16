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
    public partial class MFAnalogPanel : UserControl
    {
        // Means that the analog value must be > then the sensitivity value provided.
        // As higher the value, as less sensitive the analog device will be.
        static int DEFAULT_SENSITIVITY = 2;
        static int MIN_SENSITIVITY = 2;
        static int MAX_SENSITIVITY = 50;
        static String DEFAULT_SENSITIVITY_STRING = DEFAULT_SENSITIVITY.ToString();


        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        private MobiFlight.Config.Analog analog;
        bool initialized = false;        

        public MFAnalogPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();
        }

        public MFAnalogPanel(MobiFlight.Config.Analog analogDevice, List<MobiFlightPin> FreePins)
            : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, FreePins, analogDevice.Pin);

            if (mfPinComboBox.Items.Count > 0)
            {
                mfPinComboBox.SelectedIndex = 0;
            }

            // TODO: Complete member initialization
            this.analog = analogDevice;
            ComboBoxHelper.SetSelectedItem(mfPinComboBox, analog.Pin);
            textBoxSensitivity.Text = analog.Sensitivity;
            textBox1.Text = analog.Name;
            setValues();

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            setValues();

            if (Changed != null)
                Changed(analog, new EventArgs());
        }

        private void setValues()
        {
            analog.Pin = mfPinComboBox.Text;
            analog.Name = textBox1.Text;            

            int sensitivityValue;
            
            if (IsValidSensitivity(textBoxSensitivity.Text))
            {
                analog.Sensitivity = textBoxSensitivity.Text;
            } else
            {
                analog.Sensitivity = DEFAULT_SENSITIVITY_STRING;
            }
        }

        private bool IsValidSensitivity(string strValue )
        {
            if (Int32.TryParse(strValue, out int parsedValue))
            {
                if (parsedValue >= MIN_SENSITIVITY && parsedValue <= MAX_SENSITIVITY)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
