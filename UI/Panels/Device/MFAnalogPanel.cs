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
        static int DEFAULT_SENSITIVITY = 5;
        static int MIN_SENSITIVITY = 1;
        static int MAX_SENSITIVITY = 20;
        static String DEFAULT_SENSITIVITY_STRING = DEFAULT_SENSITIVITY.ToString();
        private MobiFlight.Config.AnalogInput analog;
        private bool initialized = false;        

        public event EventHandler Changed;

        public MFAnalogPanel()
        {
            InitializeComponent();
            mfPinComboBox.Items.Clear();            
            SensitivityTrackBar.ValueChanged += SensitivityTrackBar_ValueChanged;
            SensitivityTrackBar.Value = DEFAULT_SENSITIVITY;
        }

        private void SensitivityTrackBar_ValueChanged(object sender, EventArgs e)
        {
            SensitivityValueLabel.Text = (sender as TrackBar).Value.ToString();
            value_Changed(sender, e);
        }

        public MFAnalogPanel(MobiFlight.Config.AnalogInput analogDevice, List<MobiFlightPin> FreePins): this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPinComboBox, FreePins, analogDevice.Pin, true);

            this.analog = analogDevice;

            mfPinComboBox.SelectedValue = byte.Parse(analog.Pin);
            textBox1.Text = analog.Name;
            SensitivityTrackBar.Value = byte.Parse(analog.Sensitivity);
            
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
            analog.Pin = mfPinComboBox.SelectedValue.ToString();
            analog.Name = textBox1.Text;
            analog.Sensitivity = SensitivityTrackBar.Value.ToString();
        }
    }
}
