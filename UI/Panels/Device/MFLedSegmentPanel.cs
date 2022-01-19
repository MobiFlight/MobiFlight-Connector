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
        
        public event EventHandler Changed;

        public MFLedSegmentPanel()
        {
            InitializeComponent();
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

            update_lists();

            //if (mfPin1ComboBox.Items.Count > 2) {
                // mfPin1ComboBox.SelectedIndex = 0;
                // mfPin2ComboBox.SelectedIndex = 1;
                // mfPin3ComboBox.SelectedIndex = 2;
            //}

            textBox1.Text = ledModule.Name;
            mfIntensityTrackBar.Value = ledModule.Brightness;
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, ledModule.NumModules);

            initialized = true;
        }
        private void setNonPinValues()
        {
            ledModule.Name = textBox1.Text;
            ledModule.Brightness = (byte)(mfIntensityTrackBar.Value);
            ledModule.NumModules = mfNumModulesComboBox.Text;
        }
        private void update_lists()
        {
            bool ex_initialized = initialized;
            initialized = false;    // inhibit value_Changed events
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, ledModule.DinPin);
            //mfPin1ComboBox.SelectedValue = byte.Parse(ledModule.DinPin);  // moved inside BindMobiFlightFreePins()
            ComboBoxHelper.BindMobiFlightFreePins(mfPin2ComboBox, pinList, ledModule.ClsPin);
            //mfPin2ComboBox.SelectedValue = byte.Parse(ledModule.ClsPin);  // moved inside BindMobiFlightFreePins()
            ComboBoxHelper.BindMobiFlightFreePins(mfPin3ComboBox, pinList, ledModule.ClkPin);
            //mfPin3ComboBox.SelectedValue = byte.Parse(ledModule.ClkPin);  // moved inside BindMobiFlightFreePins()
            initialized = ex_initialized;
        }

        private void update_all(ComboBox comboBox)
        {
            bool ex_initialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (ledModule.XXXPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(mfPin1ComboBox, pinList, ref ledModule.DinPin); } else
            if (comboBox == mfPin2ComboBox) { ComboBoxHelper.reassignPin(mfPin2ComboBox, pinList, ref ledModule.ClsPin); } else
            if (comboBox == mfPin3ComboBox) { ComboBoxHelper.reassignPin(mfPin3ComboBox, pinList, ref ledModule.ClkPin); }
            // then the others are updated too 
            update_lists();

            initialized = ex_initialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            update_all(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(ledModule, new EventArgs());
        }
    }
}
