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
    public partial class MFLedSegmentPanel : UserControl
    {
        private MobiFlight.Config.LedModule ledModule;
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private bool initialized = false;
        string currentMode;
        private string exMAXClsPin = "";

        public event EventHandler Changed;

        public MFLedSegmentPanel()
        {
            InitializeComponent();
            InitializeDisplayTypeComboBox();
            mfPin1ComboBox.Items.Clear();
            mfPin2ComboBox.Items.Clear();
            mfPin3ComboBox.Items.Clear();
            if (Parent != null) mfIntensityTrackBar.BackColor = Parent.BackColor;
        }

        public MFLedSegmentPanel(MobiFlight.Config.LedModule ledModule, List<MobiFlightPin> Pins) : this()
        {
            pinList = Pins; // Keep pin list stored
                            // Since the panel is synced whenever a new device is selected, the free/used list won't change
                            // (because of changes elsewhere) as long as we remain in this panel, so we can keep it stored
                            // for the lifetime of the panel.

            this.ledModule = ledModule;
            textBox1.Text = ledModule.Name;
            mfIntensityTrackBar.Value = ledModule.Brightness;
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, ledModule.NumModules);

            initialized = true;

            currentMode = ledModule.ClsPin;
            switch (currentMode) {
                case MobiFlight.Config.LedModule.MODEL_TM1637_4D:
                case MobiFlight.Config.LedModule.MODEL_TM1637_6D:
                    mfDisplayTypeComboBox.SelectedValue = ledModule.ClsPin;
                    break;
                default:
                    mfDisplayTypeComboBox.SelectedValue = MobiFlight.Config.LedModule.MODEL_MAX72xx;
                    break;
            }

            UpdateFreePinsInDropDowns();

        }

        private void InitializeDisplayTypeComboBox()
        {
            mfDisplayTypeComboBox.Items.Clear();

            List<ListItem> languageOptions = new List<ListItem>();
            languageOptions.Add(new ListItem() { Value = MobiFlight.Config.LedModule.MODEL_MAX72xx, Label = "MAX72xx" });
            languageOptions.Add(new ListItem() { Value = MobiFlight.Config.LedModule.MODEL_TM1637_4D, Label = "TM1637 - 4Digit" });
            languageOptions.Add(new ListItem() { Value = MobiFlight.Config.LedModule.MODEL_TM1637_6D, Label = "TM1637 - 6Digit" });
            mfDisplayTypeComboBox.DataSource = languageOptions;
            mfDisplayTypeComboBox.DisplayMember = "Label";
            mfDisplayTypeComboBox.ValueMember = "Value";
            mfDisplayTypeComboBox.SelectedIndex = 0;
        }

        private bool isMax()
        { 
            //return mfDisplayTypeComboBox.SelectedIndex == 0; 
            return mfDisplayTypeComboBox.SelectedValue.ToString() == MobiFlight.Config.LedModule.MODEL_MAX72xx; 
        }
        private void setNonPinValues()
        {
            ledModule.Name = textBox1.Text;
            ledModule.Brightness = (byte)(mfIntensityTrackBar.Value);
            ledModule.NumModules = isMax()? mfNumModulesComboBox.Text : "1";
        }
        private void setMAXMode(String mode) // bool MAXmode)
        {
            var MAXmode = (mode == MobiFlight.Config.LedModule.MODEL_MAX72xx);
            mfPin2ComboBox.Visible = MAXmode;
            mfNumModulesComboBox.Visible = MAXmode;
            mfPin2Label.Visible = MAXmode;
            numberOfModulesLabel.Visible = MAXmode;
            label3.Visible = MAXmode;
        }
        private void changeMAXMode(String mode)
        {
            setMAXMode(mode);
            if (mode == MobiFlight.Config.LedModule.MODEL_MAX72xx) {
                // First try and see if the "old" pin is still available, otherwise assign the first free one
                ledModule.ClsPin = exMAXClsPin;
                ComboBoxHelper.assignPin(pinList, ref ledModule.ClsPin);
                UpdateFreePinsInDropDowns();
            } else {
                if(currentMode == MobiFlight.Config.LedModule.MODEL_MAX72xx) exMAXClsPin = ledModule.ClsPin;
                // "freePin()" is the first half part only of "ReassignFreePinsInDropDowns()"
                ComboBoxHelper.freePin(pinList, ledModule.ClsPin);
                // Make sure to use a neutral value
                ledModule.ClsPin = "";
                UpdateFreePinsInDropDowns();
                // Now replace it with the special marker
                ledModule.ClsPin = mode;
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
            if (!initialized) {
                setMAXMode(newMode);
            } else {
                changeMAXMode(newMode);
            }
        }
    }
}
