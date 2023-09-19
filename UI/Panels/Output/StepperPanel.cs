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

        private StepperProfilePreset stepperProfile;

        int[] stepValues = { -50, -10, -1, 1, 10, 50 };
        ErrorProvider errorProvider = new ErrorProvider();

        public StepperPanel()
        {
            InitializeComponent();
            stepperAddressesComboBox.SelectedValueChanged += stepperAddressesComboBox_SelectedValueChanged;
        }

        public void ShowManualCalibration(bool state)
        {
            groupBox2.Enabled = state;
            trackBar1.Focus();
        }

        internal void SyncFromConfig(OutputConfigItem config)
        {
            // stepper initialization
            if (!ComboBoxHelper.SetSelectedItem(stepperAddressesComboBox, config.Stepper.Address))
            {
                // TODO: provide error message
                Log.Instance.log("Exception on selecting item in Stepper address ComboBox.", LogSeverity.Debug);
            }

            inputRevTextBox.Text                = config.Stepper.InputRev.ToString();
            outputRevTextBox.Text               = config.Stepper.OutputRev.ToString();

            if (config.Stepper.Speed>0)
                SpeedTextBox.Text               = config.Stepper.Speed.ToString();

            if (config.Stepper.Acceleration>0)
                AccelerationTextBox.Text        = config.Stepper.Acceleration.ToString();

            CompassModeCheckBox.Checked         = config.Stepper.CompassMode;
        }

        internal OutputConfigItem SyncToConfig(OutputConfigItem config)
        {
            if (stepperAddressesComboBox.SelectedValue != null)
            {
                config.Stepper.Address      = stepperAddressesComboBox.SelectedValue.ToString();
                config.Stepper.InputRev     = Int16.Parse(inputRevTextBox.Text);
                config.Stepper.OutputRev    = Int16.Parse(outputRevTextBox.Text);
                config.Stepper.CompassMode  = CompassModeCheckBox.Checked;
                config.Stepper.Acceleration = Int16.Parse(AccelerationTextBox.Text);
                config.Stepper.Speed        = Int16.Parse(SpeedTextBox.Text);
            }
            return config;
        }

        public void SetAddresses(List<ListItem> pins)
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
            TriggerManualCalibration();
        }

        private void TriggerManualCalibration()
        {
            if (OnManualCalibrationTriggered == null)
                return;
            
            var eventArgs = new ManualCalibrationTriggeredEventArgs
                {
                    StepperAddress = stepperAddressesComboBox.SelectedValue.ToString(),
                    Steps = stepValues[trackBar1.Value]
                };

             OnManualCalibrationTriggered(this, eventArgs);
             button2.Enabled=true;
            
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            SetZero();
        }

        private void SetZero()
        {
            OnSetZeroTriggered?.Invoke(stepperAddressesComboBox.SelectedValue,
                new StepperConfigChangedEventArgs()
                {
                    StepperAddress = stepperAddressesComboBox.SelectedValue.ToString()
                });
            button2.Enabled=false;
        }
        
        private void enter_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Return:
                case (char)Keys.D0:
                case (char)Keys.NumPad0:
                    SetZero();
                    break;
                
                case (char)Keys.Space:
                    TriggerManualCalibration();
                    break;
            }
        }
        
        
        private void inputRevTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!((Control)sender).Parent.Enabled) return;

            var value = ((TextBox)sender).Text.Trim();

            if (value == "") e.Cancel = true;
            if (e.Cancel)
            {
                DisplayError((Control)sender, i18n._tr("uiMessagePanelsStepperInputRevolutionsMustNonEmpty"));
                return;
            }
            else
            {
                RemoveError((Control)sender);
            }

            try
            {
                // all boxes should only accept
                // positive numbers,
                var v = Int16.Parse(value);
                e.Cancel = !(v > 0);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                DisplayError((Control)sender, i18n._tr("uiMessageValidationMustBeNumber"));
                return;
            }

            if (e.Cancel)
            {
                DisplayError((Control)sender, i18n._tr("uiMessagePanelsStepperInputRevolutionsMustBeGreaterThan0"));
                return;
            }
            else
            {
                RemoveError((Control)sender);
            }
        }

        private void DisplayError(Control control, string message)
        {
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, i18n._tr("Hint"));
        }

        private void RemoveError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }

        private void stepperAddressesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).Items.Count == 0) return;
            if (OnStepperSelected != null)
                OnStepperSelected(sender, new StepperConfigChangedEventArgs() { StepperAddress = stepperAddressesComboBox.SelectedValue.ToString() });

            UpdateResetButtonVisibility();
        }

        internal void SetStepperProfile(StepperProfilePreset profilePreset)
        {
            // we assume that it is safe to update the values
            // when we have a different id
            if (stepperProfile?.id != profilePreset.id)
            {
                outputRevTextBox.Text = profilePreset.StepsPerRevolution.ToString();
                AccelerationTextBox.Text = profilePreset.Acceleration.ToString();
                SpeedTextBox.Text = profilePreset.Speed.ToString();
            }

            this.stepperProfile = profilePreset;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (sender == OutputRevResetButton)
            {
                outputRevTextBox.Text = this.stepperProfile.StepsPerRevolution.ToString();
            }
            else if (sender == SpeedResetButton)
                SpeedTextBox.Text = this.stepperProfile.Speed.ToString();
            else if (sender == AccelerationResetButton)
                AccelerationTextBox.Text = this.stepperProfile.Acceleration.ToString();
        }

        private void StepperSettingsTextBox_TextChanged(object sender, EventArgs e)
        {
            if (stepperProfile == null) return;
            UpdateResetButtonVisibility();
        }

        private void UpdateResetButtonVisibility()
        {
            if (stepperProfile == null) return;

            OutputRevResetButton.Visible = outputRevTextBox.Text != this.stepperProfile.StepsPerRevolution.ToString();
            AccelerationResetButton.Visible = AccelerationTextBox.Text != this.stepperProfile.Acceleration.ToString();
            SpeedResetButton.Visible = SpeedTextBox.Text != this.stepperProfile.Speed.ToString();
        }

        
    }

    public class ManualCalibrationTriggeredEventArgs : StepperConfigChangedEventArgs
    {
        public int Steps { get; set; }
    }

    public class StepperConfigChangedEventArgs : EventArgs
    {
        public string StepperAddress { get; set; }
    }
}
