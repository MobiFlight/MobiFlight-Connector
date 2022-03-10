using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFDigInputMuxPanel : UserControl
    {
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private bool                initialized;
        private bool                firstMuxed;
        private DigInputMux         digInputMux;
        private MFMuxDriverSubPanel selectorPanel; 
        
        private int MAX_MODULES = 2;            // Only possible values: 1 module for HCT4051, 2 for HCT4067
        private const string NA_STRING = "N/A";
        
        public event EventHandler   Changed;
        // event that will re-throw the same event from the selectorPanel: see note to gotoToFirstMux()
        public event EventHandler   MoveToFirstMux;

        public MFDigInputMuxPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
        }

        public MFDigInputMuxPanel(DigInputMux digInputMux, List<MobiFlightPin> Pins, bool isFirstMuxed) : this()
        {
            pinList = Pins; // Keep pin list stored
            firstMuxed = isFirstMuxed;

            // Set selector panel values
            selectorPanel = new MFMuxDriverSubPanel(digInputMux.Selector, Pins, isFirstMuxed);
            selectorPanel.Changed += this.Changed;
            selectorPanel.MoveToFirstMux += new EventHandler(gotoToFirstMux);
            selectorPanel.notifyPinListChange = receivePinChange;
            selectorPanel.UpdateFreePinsInDropDowns(Pins);

            muxDrvPanel.Controls.Add(selectorPanel);
            selectorPanel.Dock = DockStyle.Fill;

            // Set Data pin values
            this.digInputMux = digInputMux;
            UpdateFreePinsInDropDowns();

            // Set non-pin data values
            for (int i = 1; i <= MAX_MODULES; i++)
            {
                mfNumModulesComboBox.Items.Add(i);
            }
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, digInputMux.NumModules);
            textBox1.Text = digInputMux.Name;

            initialized = true;
        }
        private void gotoToFirstMux(object sender, EventArgs e) 
        {
            // Since the event for the "jump" to the first mux selector host is originated
            // by the selector subpanel (our dependent) and must be processed by the device tree (our owner),
            // but these have otherwise no means of being in contact, we act as a proxy by implementing an event
            // like the original one in order to rethrow it.
            this.MoveToFirstMux(sender, e);
        }

        private void receivePinChange(List<MobiFlightPin> newPinList)
        {
            // this event is received when a pin changes in the dependent selectorPanel,
            // and we have to update our available pin list
            UpdateFreePinsInDropDowns(newPinList);
        }


        private void setNonPinValues()
        {
            //digInputMux.DataPin = mfPin1ComboBox.Text;
            digInputMux.Name = textBox1.Text;
            digInputMux.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
        }
        public void UpdateFreePinsInDropDowns(List<MobiFlightPin> newPinList = null)
        {
            // This method is public (and has an optional "new pin list" argument) because it must be called by the
            // embedded MFMuxDriverSubPanel in case its pin data changes.
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            if (newPinList != null)
            {
                pinList = newPinList;
            }
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, digInputMux.DataPin);
            if(!firstMuxed)
            {
                selectorPanel.UpdateFreePinsInDropDowns(pinList);
            }
            initialized = exInitialized;
        }

        private void ReassignFreePinsInDropDowns(ComboBox comboBox)
        {
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events

            // First update the one that is changed
            // Here, the config data (digInputMux.DataPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPin1ComboBox) { ComboBoxHelper.reassignPin(mfPin1ComboBox, pinList, ref digInputMux.DataPin); }
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            ReassignFreePinsInDropDowns(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(digInputMux, new EventArgs());
        }
    }
}