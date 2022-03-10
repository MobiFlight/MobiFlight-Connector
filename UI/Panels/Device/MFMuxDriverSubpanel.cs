using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFMuxDriverSubPanel : UserControl
    {
        public event EventHandler Changed;
        public event EventHandler MoveToFirstMux;
        public delegate void notifyPinListChangeEvent(List<MobiFlightPin> Pins);

        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private MuxDriver muxDriver;
        public notifyPinListChangeEvent notifyPinListChange;
        private bool initialized;

        public MFMuxDriverSubPanel()
        {
            InitializeComponent();
            mfPinS0ComboBox.Items.Clear();
            mfPinS1ComboBox.Items.Clear();
            mfPinS2ComboBox.Items.Clear();
            mfPinS3ComboBox.Items.Clear();
        }

        public MFMuxDriverSubPanel(MuxDriver muxDriver, List<MobiFlightPin> Pins, bool isEnabled = false)
            : this()
        {
            pinList = Pins; // Keep pin list stored

            this.muxDriver = muxDriver;
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
            // host MFDigInputMuxPanel in case its pin data changes.
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            if (newPinList != null)
            {
                pinList = newPinList;
            }
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS0ComboBox, pinList, this.muxDriver.PinSx[0]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS1ComboBox, pinList, this.muxDriver.PinSx[1]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS2ComboBox, pinList, this.muxDriver.PinSx[2]);
            ComboBoxHelper.BindMobiFlightFreePins(mfPinS3ComboBox, pinList, this.muxDriver.PinSx[3]);

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
            // Here, the config data (muxDriver.PinSx[]) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPinS0ComboBox) { ComboBoxHelper.reassignPin(mfPinS0ComboBox, pinList, ref this.muxDriver.PinSx[0]); }
            else
            if (comboBox == mfPinS1ComboBox) { ComboBoxHelper.reassignPin(mfPinS1ComboBox, pinList, ref this.muxDriver.PinSx[1]); }
            else
            if (comboBox == mfPinS2ComboBox) { ComboBoxHelper.reassignPin(mfPinS2ComboBox, pinList, ref this.muxDriver.PinSx[2]); }
            else
            if (comboBox == mfPinS3ComboBox) { ComboBoxHelper.reassignPin(mfPinS3ComboBox, pinList, ref this.muxDriver.PinSx[3]); }
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
            //setValues();
            ReassignFreePinsInDropDowns(sender as ComboBox);
            if (Changed != null)
                Changed(muxDriver, new EventArgs());
        }

        private void btnGotoSetting_Click(object sender, EventArgs e)
        {
            if (!initialized) return;
            if (MoveToFirstMux != null)
                MoveToFirstMux(muxDriver, new EventArgs());
        }

    }
}