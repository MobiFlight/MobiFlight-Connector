using CommandMessenger;
using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static MobiFlight.UI.Panels.Settings.Device.MFStepperPanel;
using static System.Windows.Forms.Design.AxImporter;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFStepperPanel : UserControl
    {
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private MobiFlight.Config.Stepper stepper;
        private bool initialized = false;
        public event EventHandler Changed;

        public enum StepperMode
        {
            FULLSTEP,
            HALFSTEP,
            DRIVER
        }

        public static List<ListItem<StepperPreset>> Presets = new List<ListItem<StepperPreset>>()
        {
            new ListItem<StepperPreset>()
            {
                Label = "28BYJ - Half-step mode (recommended)",
                Value = new StepperPreset()
                {
                    id = 0,
                    Mode = StepperMode.HALFSTEP,
                    Speed = 1400,
                    Acceleration = 2800,
                    BacklashCompensation = 20,
                    StepsPerRevolution = 4080
                }
            },
            new ListItem<StepperPreset>()
            {
                Label = "28BYJ - Full-step mode (classic)",
                Value = new StepperPreset()
                {
                    id = 1,
                    Mode = StepperMode.FULLSTEP,
                    Speed = 467,
                    Acceleration = 800,
                    BacklashCompensation = 3,
                    StepsPerRevolution = 2040
                }
            },
            new ListItem<StepperPreset>()
            {
                Label = "x.27 - Half-step mode",
                Value = new StepperPreset()
                {
                    id = 2,
                    Mode = StepperMode.HALFSTEP,
                    Speed = 1800,
                    Acceleration = 3600,
                    BacklashCompensation = 0,
                    StepsPerRevolution = 1100
                }
            },
            new ListItem<StepperPreset>()
            {
                Label = "Generic - EasyDriver",
                Value = new StepperPreset()
                {
                    id = 3,
                    Mode = StepperMode.DRIVER,
                    Speed = 400,
                    Acceleration = 800,
                    BacklashCompensation = 0,
                    StepsPerRevolution = 1000
                }
            }
        };

        public static List<ListItem<StepperMode>> Modes = new List<ListItem<StepperMode>>()
        {
            new ListItem<StepperMode>() { Label = "Half-step mode", Value = StepperMode.HALFSTEP },
            new ListItem<StepperMode>() { Label = "Full-step mode", Value = StepperMode.FULLSTEP },
            new ListItem<StepperMode>() { Label = "Driver", Value = StepperMode.DRIVER}
        };

        public class StepperPreset {
            public int id { get; set; }
            public StepperMode Mode { get; set; }
            public int StepsPerRevolution { get; set; }
            public int Speed { get; set; }
            public int Acceleration { get; set; }
            public int BacklashCompensation { get; set; }
            public bool Deactivate { get; set; }
        }

        public MFStepperPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
            mfPin4ComboBox.Items.Clear();
            mfBtnPinComboBox.Items.Clear();

            InitializeModeComboBox();
            InitializePresetComboBox();

            PresetComboBox.SelectedIndexChanged += PresetComboBox_SelectedValueChanged;
            ModeComboBox.SelectedIndexChanged += ModeComboBox_SelectedValueChanged;
        }

        private void ModeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var mode = (StepperMode) ModeComboBox.SelectedValue;
            mfPin3Label.Visible = mfPin3ComboBox.Visible = mode != StepperMode.DRIVER;
            mfPin4Label.Visible = mfPin4ComboBox.Visible = mode != StepperMode.DRIVER;
            if (mode == StepperMode.DRIVER)
            {
                ComboBoxHelper.reassignPin(mfPin1ComboBox, pinList, ref stepper.Pin3);
                ComboBoxHelper.reassignPin(mfPin2ComboBox, pinList, ref stepper.Pin4);
            }
            
            UpdateFreePinsInDropDowns();
        }

        private void PresetComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var preset = PresetComboBox.SelectedValue as StepperPreset;
            ModeComboBox.SelectedValue = preset.Mode;
            BacklashTextBox.Text = preset.BacklashCompensation.ToString();
            DefaultSpeedTextBox.Text = preset.Speed.ToString();
            DefaultAccelerationTextBox.Text = preset.Acceleration.ToString();
            deactivateCheckBox.Checked = preset.Deactivate;
        }

        private void InitializePresetComboBox()
        {
            PresetComboBox.Items.Clear();
            PresetComboBox.DataSource = Presets;
            PresetComboBox.ValueMember = "Value";
            PresetComboBox.DisplayMember = "Label";
        }

        private void InitializeModeComboBox()
        {
            var options = new List<ListItem<StepperMode>>();

            ModeComboBox.Items.Clear();
            ModeComboBox.DataSource = Modes;
            ModeComboBox.ValueMember = "Value";
            ModeComboBox.DisplayMember = "Label";
        }

        public MFStepperPanel(MobiFlight.Config.Stepper stepper, List<MobiFlightPin> Pins): this()
        {
            pinList = Pins; // Keep pin list stored

            this.stepper = stepper;
            UpdateFreePinsInDropDowns();

            mfNameTextBox.Text = stepper.Name;
            autoZeroCheckBox.Checked = stepper.BtnPin == "0";

            if (stepper.BtnPin != "0")
                ComboBoxHelper.SetSelectedItem(mfBtnPinComboBox, stepper.BtnPin);

            // Load the profile first
            StepperPreset savedPreset = Presets.Find(x => x.Value.id == stepper.Profile).Value;

            PresetComboBox.SelectedValue = savedPreset;
            ;
            // Then restore potential custom values
            StepperMode saveMode = Modes.Find(x => x.Value == (StepperMode)stepper.Mode).Value;
            ModeComboBox.SelectedValue = saveMode;

            deactivateCheckBox.Checked = stepper.Deactivate;
            BacklashTextBox.Text = stepper.Backlash.ToString();
            DefaultSpeedTextBox.Text = Presets.Find(x => (x.Value.id == stepper.Profile))
                                              .Value.Speed.ToString();

            DefaultAccelerationTextBox.Text = Presets.Find(x => (x.Value.id == stepper.Profile))
                                                          .Value.Acceleration.ToString();

            initialized = true;
        }

        private void setNonPinValues()
        {
            stepper.Name = mfNameTextBox.Text;
            stepper.Mode = (int) ModeComboBox.SelectedValue;
            stepper.Backlash = int.Parse(BacklashTextBox.Text);
            stepper.Deactivate = deactivateCheckBox.Checked;
            stepper.Profile = (PresetComboBox.SelectedValue as StepperPreset).id;
        }

        private void UpdateFreePinsInDropDowns()
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, stepper.Pin1);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, pinList, stepper.Pin2);
            if ((StepperMode) ModeComboBox.SelectedValue != StepperMode.DRIVER)
            {
                ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, pinList, stepper.Pin3);
                ComboBoxHelper.BindMobiFlightFreePins(mfPin4ComboBox, pinList, stepper.Pin4);
            }
            
            ComboBoxHelper.BindMobiFlightFreePins(mfBtnPinComboBox, pinList, stepper.BtnPin);
            initialized = exInitialized;
        }

        private void ReassignFreePinsInDropDowns(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (stepper.XXXPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(mfPin1ComboBox, pinList, ref stepper.Pin1); } else
            if (comboBox == mfPin2ComboBox) { ComboBoxHelper.reassignPin(mfPin2ComboBox, pinList, ref stepper.Pin2); } else
            if (comboBox == mfPin3ComboBox) { ComboBoxHelper.reassignPin(mfPin3ComboBox, pinList, ref stepper.Pin3); } else
            if (comboBox == mfPin4ComboBox) { ComboBoxHelper.reassignPin(mfPin4ComboBox, pinList, ref stepper.Pin4); } else
            if (comboBox == mfBtnPinComboBox) { ComboBoxHelper.reassignPin(mfBtnPinComboBox, pinList, ref stepper.BtnPin); }
            // then the others are updated too 
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            ReassignFreePinsInDropDowns(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(stepper, new EventArgs());
        }

        private void autoZeroCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mfBtnPinComboBox.Enabled = !(sender as System.Windows.Forms.CheckBox).Checked;
        }
    }
}