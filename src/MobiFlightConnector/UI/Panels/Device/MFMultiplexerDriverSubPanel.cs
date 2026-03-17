using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFMultiplexerDriverSubPanel : UserControl
    {
        public event EventHandler Changed;
        public event EventHandler MoveToFirstMux;
        public delegate void notifyPinListChangeEvent(List<MobiFlightPin> Pins);

        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private MultiplexerDriver multiplexerDriver;
        public notifyPinListChangeEvent notifyPinListChange;
        private bool initialized;

        public MFMultiplexerDriverSubPanel()
        {
            InitializeComponent();
            mfPinS0ComboBox.Items.Clear();
            mfPinS1ComboBox.Items.Clear();
            mfPinS2ComboBox.Items.Clear();
            mfPinS3ComboBox.Items.Clear();
        }

        public MFMultiplexerDriverSubPanel(MultiplexerDriver multiplexerDriver, List<MobiFlightPin> Pins, bool isEnabled = false)
            : this()
        {
            pinList = Pins; // Keep pin list stored

            this.multiplexerDriver = multiplexerDriver;
            UpdateFreePinsInDropDowns();

            SuspendLayout();

            //if (mfPinS0ComboBox.Items.Count > 3)
            //{
            //    mfPinS0ComboBox.SelectedIndex = 0;
            //    mfPinS1ComboBox.SelectedIndex = 1;
            //    mfPinS2ComboBox.SelectedIndex = 2;
            //    mfPinS3ComboBox.SelectedIndex = 3;
            //}

            enable(isEnabled);
            // isEnabled should affects comboboxes and user message only
            labelMaster.Visible = isEnabled;
            labelSlave.Visible = !isEnabled;

            ResumeLayout();

            initialized = true;
        }

        public void UpdateFreePinsInDropDowns(List<MobiFlightPin> newPinList = null)
        {
            // This method is public (and has an optional "new pin list" argument) because it must be called by the
            // host MFInputMultiplexerPanel in case its pin data changes.
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            if (newPinList != null)
            {
                pinList = newPinList;
            }
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS0ComboBox, pinList, this.multiplexerDriver.PinSx[0]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS1ComboBox, pinList, this.multiplexerDriver.PinSx[1]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS2ComboBox, pinList, this.multiplexerDriver.PinSx[2]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS3ComboBox, pinList, this.multiplexerDriver.PinSx[3]);

            if (newPinList == null)
            {
                // Only execute if pin change is local, not when signaled from outside!
                // Signal pin change do host object, if any
                if (notifyPinListChange != null) notifyPinListChange(pinList);
            }

            initialized = exInitialized;
        }
        private void ReassignFreePinsInDropDowns(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (multiplexerDriver.PinSx[]) is updated with the new value read from the changed ComboBox;
            var newPin = comboBox.SelectedItem.ToString();
            if (comboBox == mfPinS0ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref this.multiplexerDriver.PinSx[0]); } else
            if (comboBox == mfPinS1ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref this.multiplexerDriver.PinSx[1]); } else
            if (comboBox == mfPinS2ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref this.multiplexerDriver.PinSx[2]); } else
            if (comboBox == mfPinS3ComboBox) { ComboBoxHelper.reassignPin(newPin, pinList, ref this.multiplexerDriver.PinSx[3]); }
            // then the others are updated too 
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }
        public void enable(bool status)
        {
            mfPinS0ComboBox.Enabled = status;
            mfPinS1ComboBox.Enabled = status;
            mfPinS2ComboBox.Enabled = status;
            mfPinS3ComboBox.Enabled = status;
            btnGotoSetting.Visible = !status;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            
            if (sender is ComboBox)
                ReassignFreePinsInDropDowns(sender as ComboBox);

            if (Changed != null)
                Changed(multiplexerDriver, new EventArgs());
        }

        private void btnGotoSetting_Click(object sender, EventArgs e)
        {
            if (!initialized) return;
            if (MoveToFirstMux != null)
                MoveToFirstMux(multiplexerDriver, new EventArgs());
        }

    }
}