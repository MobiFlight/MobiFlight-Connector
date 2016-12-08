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
    public partial class StepperPanel : UserControl
    {


        public event EventHandler<ManualCalibrationTriggeredEventArgs> OnManualCalibrationTriggered;
        public event EventHandler OnSetZeroTriggered;
        int[] StepValues = { -50, -10, -1, 1, 10, 50 };
        ErrorProvider errorProvider = new ErrorProvider();

        public StepperPanel()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // stepper initialization
            if (!ComboBoxHelper.SetSelectedItem(stepperAddressesComboBox, config.StepperAddress))
            {
                // TODO: provide error message
                Log.Instance.log("_syncConfigToForm : Exception on selecting item in Stepper Address ComboBox", LogSeverity.Debug);
            }

            if (config.StepperInputRev != null) inputRevTextBox.Text = config.StepperInputRev;
            if (config.StepperOutputRev != null) outputRevTextBox.Text = config.StepperOutputRev;
            if (config.StepperTestValue != null) stepperTestValueTextBox.Text = config.StepperTestValue;
            CompassModeCheckBox.Checked = config.StepperCompassMode;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (stepperAddressesComboBox.SelectedValue != null)
            {
                config.StepperAddress = stepperAddressesComboBox.SelectedValue.ToString();
                config.StepperInputRev = inputRevTextBox.Text;
                config.StepperOutputRev = outputRevTextBox.Text;
                config.StepperTestValue = stepperTestValueTextBox.Text;
                config.StepperCompassMode = CompassModeCheckBox.Checked;
            }
            return config;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void SetSelectedAddress(string value)
        {
            stepperAddressesComboBox.SelectedValue = value;
        }

        public void SetAdresses(List<ListItem> pins)
        {
            stepperAddressesComboBox.DataSource = new List<ListItem>(pins);
            stepperAddressesComboBox.DisplayMember = "Label";
            stepperAddressesComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                stepperAddressesComboBox.SelectedIndex = 0;

            stepperAddressesComboBox.Enabled = pins.Count > 0;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OnManualCalibrationTriggered != null)
            {
                ManualCalibrationTriggeredEventArgs eventArgs = new ManualCalibrationTriggeredEventArgs();                
                eventArgs.Steps = StepValues[trackBar1.Value];
                OnManualCalibrationTriggered(sender, eventArgs);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (OnSetZeroTriggered != null)
            {
                OnSetZeroTriggered(sender, e);
            }
        }

        private void inputRevTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(sender as Control).Parent.Enabled) return;

            String value = (sender as TextBox).Text.Trim();

            if (value == "") e.Cancel = true;
            if (e.Cancel)
            {
                displayError(sender as Control, MainForm._tr("uiMessagePanelsStepperInputRevolutionsMustNonEmpty"));
                return;
            }
            else
            {
                removeError(sender as Control);
            }

            try
            {
                e.Cancel = !(Int16.Parse(value) > 0);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
            }
            if (e.Cancel)
            {
                displayError(sender as Control, MainForm._tr("uiMessagePanelsStepperInputRevolutionsMustBeGreaterThan0"));
                return;
            }
            else
            {
                removeError(sender as Control);
            }
        }

        private void displayError(Control control, String message)
        {
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, MainForm._tr("Hint"));
        }

        private void removeError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }

        private void CompassModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    public class ManualCalibrationTriggeredEventArgs : EventArgs
    {
        public int Steps { get; set; }
    }
}
