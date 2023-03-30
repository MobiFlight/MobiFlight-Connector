using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class StepperPanel : UserControl
    {
        public event EventHandler<ManualCalibrationTriggeredEventArgs> OnManualCalibrationTriggered;
        public event EventHandler<StepperConfigChangedEventArgs> OnSetZeroTriggered;
        public event EventHandler<StepperConfigChangedEventArgs> OnStepperSelected;
        protected StepperProfilePreset StepperProfile;

        int[] StepValues = { -50, -10, -1, 1, 10, 50 };
        ErrorProvider errorProvider = new ErrorProvider();

        public StepperPanel()
        {
            InitializeComponent();
            stepperAddressesComboBox.SelectedValueChanged += stepperAddressesComboBox_SelectedValueChanged;
        }

        public void ShowManualCalibration(Boolean state)
        {
            groupBox2.Enabled = state;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // stepper initialization
            if (!ComboBoxHelper.SetSelectedItem(stepperAddressesComboBox, config.Stepper.Address))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in Stepper address ComboBox.", LogSeverity.Debug);
            }

            inputRevTextBox.Text            = config.Stepper.InputRev.ToString();
            outputRevTextBox.Text           = config.Stepper.OutputRev.ToString();

            if (config.Stepper.Speed>0)
                SpeedTextBox.Text               = config.Stepper.Speed.ToString();

            if (config.Stepper.Acceleration>0)
                AccelerationTextBox.Text        = config.Stepper.Acceleration.ToString();

            stepperTestValueTextBox.Text    = config.Stepper.TestValue.ToString();
            CompassModeCheckBox.Checked     = config.Stepper.CompassMode;
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            if (stepperAddressesComboBox.SelectedValue != null)
            {
                config.Stepper.Address      = stepperAddressesComboBox.SelectedValue.ToString();
                config.Stepper.InputRev     = Int16.Parse(inputRevTextBox.Text);
                config.Stepper.OutputRev    = Int16.Parse(outputRevTextBox.Text);
                config.Stepper.TestValue    = Int16.Parse(stepperTestValueTextBox.Text);
                config.Stepper.CompassMode  = CompassModeCheckBox.Checked;
                config.Stepper.Acceleration = Int16.Parse(AccelerationTextBox.Text);
                config.Stepper.Speed        = Int16.Parse(SpeedTextBox.Text);
            }
            return config;
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

            // let's enable this again
            if (pins.Count > 0)
            {
                stepperAddressesComboBox.SelectedIndex = 0;
            }

            stepperAddressesComboBox.Enabled = pins.Count > 0;
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
                // all boxes should only accept
                // positive numbers,
                // exception is the test value textbox
                var v = Int16.Parse(value);
                if (sender != stepperTestValueTextBox)
                    e.Cancel = !(v > 0);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                displayError(sender as Control, i18n._tr("uiMessageValidationMustBeNumber"));
                return;
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

        private void stepperAddressesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((sender as ComboBox).Items.Count == 0) return;
            if (OnStepperSelected != null)
                OnStepperSelected(sender, new StepperConfigChangedEventArgs() { StepperAddress = stepperAddressesComboBox.SelectedValue.ToString() });

            UpdateResetButtonVisibility();
        }

        internal void SetStepperProfile(StepperProfilePreset profilePreset)
        {
            // we assume that it is safe to update the values
            // when we have a different id
            if (StepperProfile?.id != profilePreset.id)
            {
                outputRevTextBox.Text = profilePreset.StepsPerRevolution.ToString();
                AccelerationTextBox.Text = profilePreset.Acceleration.ToString();
                SpeedTextBox.Text = profilePreset.Speed.ToString();
            }

            this.StepperProfile = profilePreset;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (sender == OutputRevResetButton)
            {
                outputRevTextBox.Text = this.StepperProfile.StepsPerRevolution.ToString();
            }
            else if (sender == SpeedResetButton)
                SpeedTextBox.Text = this.StepperProfile.Speed.ToString();
            else if (sender == AccelerationResetButton)
                AccelerationTextBox.Text = this.StepperProfile.Acceleration.ToString();
        }

        private void StepperSettingsTextBox_TextChanged(object sender, EventArgs e)
        {
            if (StepperProfile == null) return;
            UpdateResetButtonVisibility();
        }

        private void UpdateResetButtonVisibility()
        {
            if (StepperProfile == null) return;

            OutputRevResetButton.Visible = outputRevTextBox.Text != this.StepperProfile.StepsPerRevolution.ToString();
            AccelerationResetButton.Visible = AccelerationTextBox.Text != this.StepperProfile.Acceleration.ToString();
            SpeedResetButton.Visible = SpeedTextBox.Text != this.StepperProfile.Speed.ToString();
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
