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
    public partial class StepperPanel : UserControl
    {


        public event EventHandler<ManualCalibrationTriggeredEventArgs> OnManualCalibrationTriggered;
        public event EventHandler<StepperConfigChangedEventArgs> OnSetZeroTriggered;
        public event EventHandler<StepperConfigChangedEventArgs> OnStepperSelected;

        int[] StepValues = { -50, -10, -1, 1, 10, 50 };
        ErrorProvider errorProvider = new ErrorProvider();

        public StepperPanel()
        {
            InitializeComponent();
        }

        public void ShowManualCalibation(Boolean state)
        {
            groupBox2.Enabled = state;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // stepper initialization
            if (!ComboBoxHelper.SetSelectedItem(stepperAddressesComboBox, config.Stepper.Address))
            {
                // TODO: provide error message
                Log.Instance.log($"{GetType().Name}.syncFromConfig : Exception on selecting item in Stepper Address ComboBox", LogSeverity.Debug);
            }

            if (config.Stepper.InputRev != null) inputRevTextBox.Text = config.Stepper.InputRev;
            if (config.Stepper.OutputRev != null) outputRevTextBox.Text = config.Stepper.OutputRev;
            if (config.Stepper.TestValue != null) stepperTestValueTextBox.Text = config.Stepper.TestValue;
            CompassModeCheckBox.Checked = config.Stepper.CompassMode;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (stepperAddressesComboBox.SelectedValue != null)
            {
                config.Stepper.Address = stepperAddressesComboBox.SelectedValue.ToString();
                config.Stepper.InputRev = inputRevTextBox.Text;
                config.Stepper.OutputRev = outputRevTextBox.Text;
                config.Stepper.TestValue = stepperTestValueTextBox.Text;
                config.Stepper.CompassMode = CompassModeCheckBox.Checked;
            }
            return config;
        }

        public void SetSelectedAddress(string value)
        {
            stepperAddressesComboBox.SelectedValue = value;
        }

        public void SetAdresses(List<ListItem> pins)
        {
            // when we initialize the stepper panel
            // we don't want to trigger this
            stepperAddressesComboBox.SelectedValueChanged -= stepperAddressesComboBox_SelectedValueChanged;
            stepperAddressesComboBox.DataSource = new List<ListItem>(pins);
            stepperAddressesComboBox.DisplayMember = "Label";
            stepperAddressesComboBox.ValueMember = "Value";

            if (pins.Count > 0)
                stepperAddressesComboBox.SelectedIndex = 0;

            stepperAddressesComboBox.Enabled = pins.Count > 0;

            // let's enable this again
            stepperAddressesComboBox.SelectedValueChanged += stepperAddressesComboBox_SelectedValueChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OnManualCalibrationTriggered != null)
            {
                ManualCalibrationTriggeredEventArgs eventArgs = new ManualCalibrationTriggeredEventArgs();
                eventArgs.StepperAddress = stepperAddressesComboBox.SelectedValue.ToString();
                eventArgs.Steps = StepValues[trackBar1.Value];

                OnManualCalibrationTriggered(this, eventArgs);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OnSetZeroTriggered?.Invoke(stepperAddressesComboBox.SelectedValue,
                                   new StepperConfigChangedEventArgs()
                                   {
                                       StepperAddress = stepperAddressesComboBox.SelectedValue.ToString()
                                   });
        }

        private void inputRevTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(sender as Control).Parent.Enabled) return;

            String value = (sender as TextBox).Text.Trim();

            if (value == "") e.Cancel = true;
            if (e.Cancel)
            {
                displayError(sender as Control, i18n._tr("uiMessagePanelsStepperInputRevolutionsMustNonEmpty"));
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
                displayError(sender as Control, i18n._tr("uiMessagePanelsStepperInputRevolutionsMustBeGreaterThan0"));
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
            MessageBox.Show(message, i18n._tr("Hint"));
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

        private void stepperAddressesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).Items.Count == 0) return;
            if (OnStepperSelected != null)
                OnStepperSelected(sender, new StepperConfigChangedEventArgs() { StepperAddress = stepperAddressesComboBox.SelectedValue.ToString() });
        }
    }

    public class ManualCalibrationTriggeredEventArgs : StepperConfigChangedEventArgs
    {
        public int Steps { get; set; }
    }

    public class StepperConfigChangedEventArgs : EventArgs
    {
        public String StepperAddress { get; set; }
    }
}
