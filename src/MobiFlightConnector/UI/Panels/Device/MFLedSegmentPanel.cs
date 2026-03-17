using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MobiFlight.Config;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFLedSegmentPanel : UserControl
    {
        private LedModule ledModule;
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private bool initialized = false;
        string currentMode;
        private string OldMaxClsPin = "";
        public bool SupportsTM1637 { get; set; } = false;

        public event EventHandler Changed;

        public MFLedSegmentPanel()
        {
            InitializeComponent();
            UpdateTrackBarBackgroundColor();
            InitializeDisplayTypeComboBox();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
        }

        public MFLedSegmentPanel(LedModule ledModule, List<MobiFlightPin> Pins, bool supportsTM1637) : this()
        {
            pinList = Pins; // Keep pin list stored
                            // Since the panel is synced whenever a new device is selected, the free/used list won't change
                            // (because of changes elsewhere) as long as we remain in this panel, so we can keep it stored
                            // for the lifetime of the panel.

            this.ledModule = ledModule;
            textBox1.Text = ledModule.Name;
            mfIntensityTrackBar.Value = ledModule.Brightness;
            SupportsTM1637 = supportsTM1637;
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, ledModule.NumModules);

            initialized = true;
            mfDisplayTypeComboBox.SelectedValue = ledModule.ModelType;

            UpdateFreePinsInDropDowns();
        }

        private void InitializeDisplayTypeComboBox()
        {
            mfDisplayTypeComboBox.Items.Clear();

            List<ListItem> languageOptions = new List<ListItem>
            {
                new ListItem() { Value = LedModule.MODEL_TYPE_MAX72xx, Label = "MAX7219 / MAX7221" },
                new ListItem() { Value = LedModule.MODEL_TYPE_TM1637_4DIGIT, Label = "TM1637 - 4 digits" },
                new ListItem() { Value = LedModule.MODEL_TYPE_TM1637_6DIGIT, Label = "TM1637 - 6 digits" }
            };
            mfDisplayTypeComboBox.DataSource = languageOptions;
            mfDisplayTypeComboBox.DisplayMember = "Label";
            mfDisplayTypeComboBox.ValueMember = "Value";
            mfDisplayTypeComboBox.SelectedIndex = 0;
        }

        public void UpdateTrackBarBackgroundColor()
        {
            mfIntensityTrackBar.BackColor = System.Drawing.Color.FromArgb(255,249,249,249);
        }

        private bool isMax()
        { 
            return mfDisplayTypeComboBox.SelectedValue.ToString() == LedModule.MODEL_TYPE_MAX72xx; 
        }
        private void setNonPinValues()
        {
            ledModule.Name = textBox1.Text;
            ledModule.Brightness = (byte)(mfIntensityTrackBar.Value);
            ledModule.NumModules = isMax()? mfNumModulesComboBox.Text : "1";
        }
        private void setMAXMode(String mode) // bool MAXmode)
        {
            ledModule.ModelType = mode;

            var MAXmode = (mode == LedModule.MODEL_TYPE_MAX72xx);

            mfPin2ComboBox.Visible       = MAXmode;
            mfNumModulesComboBox.Visible = MAXmode;
            mfPin2Label.Visible          = MAXmode;
            numberOfModulesLabel.Visible = MAXmode;
            label3.Visible               = MAXmode;
        }
        private void changeMAXMode(String mode)
        {
            setMAXMode(mode);
            if (mode == LedModule.MODEL_TYPE_MAX72xx) {
                // First try and see if the "old" pin is still available, otherwise assign the first free one
                ledModule.ClsPin = OldMaxClsPin;
                ComboBoxHelper.assignPin(pinList, ref ledModule.ClsPin);
                UpdateFreePinsInDropDowns();
            } else {
                if(currentMode == LedModule.MODEL_TYPE_MAX72xx) OldMaxClsPin = ledModule.ClsPin;
                // "freePin()" is the first half part only of "ReassignFreePinsInDropDowns()"
                if (ledModule.ClsPin != "") {
                    ComboBoxHelper.freePin(pinList, ledModule.ClsPin);
                }
                // Make sure to use a neutral value
                ledModule.ClsPin = "0";
                UpdateFreePinsInDropDowns();
            }
            currentMode = mode;
            if (Changed != null)
                Changed(ledModule, new EventArgs());
        }
        private void UpdateFreePinsInDropDowns()
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, ledModule.DinPin);
            if(isMax()) ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, pinList, ledModule.ClsPin);
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, pinList, ledModule.ClkPin);
            initialized = exInitialized;
        }

        private void ReassignFreePinsInDropDowns(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (ledModule.XXXPin) is updated with the new value read from the changed ComboBox;
            var newPin = comboBox.SelectedItem.ToString();
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref ledModule.DinPin); } else
            if (comboBox == mfPin2ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref ledModule.ClsPin); } else
            if (comboBox == mfPin3ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref ledModule.ClkPin); }
            // then the others are updated too 
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            if(sender == mfPin1ComboBox || sender == mfPin2ComboBox || sender == mfPin3ComboBox) { 
                ReassignFreePinsInDropDowns(sender as ComboBox);
            }
            setNonPinValues();
            if (Changed != null)
                Changed(ledModule, new EventArgs());
        }
        
        private void mfDisplayTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!initialized) return;
            
            string newMode = (sender as ComboBox).SelectedValue.ToString();

            if ((newMode == LedModule.MODEL_TYPE_TM1637_4DIGIT || newMode == LedModule.MODEL_TYPE_TM1637_6DIGIT) && !SupportsTM1637)
            {
                MessageBox.Show(i18n._tr("uiMessageSettingsDialogFirmwareVersionTooLowException"), i18n._tr("Hint"));
                mfDisplayTypeComboBox.SelectedValue = LedModule.MODEL_TYPE_MAX72xx;
                return;
            }

            changeMAXMode(newMode);
            
            Changed?.Invoke(ledModule, new EventArgs());
        }
    }
}
