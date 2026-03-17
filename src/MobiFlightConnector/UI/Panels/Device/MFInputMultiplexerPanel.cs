using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFInputMultiplexerPanel : UserControl
    {
        private List<MobiFlightPin> pinList;    // COMPLETE list of pins (includes status)
        private bool                initialized;
        private bool                firstMuxed;
        private InputMultiplexer         inputMultiplexer;
        private MFMultiplexerDriverSubPanel selectorPanel; 
        
        private int MAX_MODULES = 2;            // Only possible values: 1 module for HCT4051, 2 for HCT4067
        private const string NA_STRING = "N/A";
        
        public event EventHandler   Changed;
        // event that will re-throw the same event from the selectorPanel: see note to gotoToFirstMux()
        public event EventHandler   MoveToFirstMux;

        public MFInputMultiplexerPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
        }

        public MFInputMultiplexerPanel(InputMultiplexer inputMultiplexer, List<MobiFlightPin> Pins, bool isFirstMuxed) : this()
        {
            pinList = Pins; // Keep pin list stored
            firstMuxed = isFirstMuxed;

            // Set selector panel values
            selectorPanel = new MFMultiplexerDriverSubPanel(inputMultiplexer.Selector, Pins, isFirstMuxed);
            selectorPanel.Changed += this.Changed;
            selectorPanel.MoveToFirstMux += new EventHandler(gotoToFirstMux);
            selectorPanel.notifyPinListChange = receivePinChange;
            selectorPanel.UpdateFreePinsInDropDowns(Pins);

            muxDrvPanel.Controls.Add(selectorPanel);
            selectorPanel.Dock = DockStyle.Fill;

            // Set Data pin values
            this.inputMultiplexer = inputMultiplexer;
            UpdateFreePinsInDropDowns();

            List<ListItem> options = new List<ListItem>();
            options.Add(new ListItem() { Value = "1", Label = "8-bit multiplexer" });
            options.Add(new ListItem() { Value = "2", Label = "16-bit multiplexer" });

            mfNumModulesComboBox.DataSource = options;
            mfNumModulesComboBox.ValueMember = "Value";
            mfNumModulesComboBox.DisplayMember = "Label";

            ComboBoxHelper.SetSelectedItemByValue(mfNumModulesComboBox, inputMultiplexer.NumBytes);
            textBox1.Text = inputMultiplexer.Name;

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
            inputMultiplexer.Name = textBox1.Text;
            inputMultiplexer.NumBytes = string.IsNullOrEmpty(mfNumModulesComboBox.SelectedValue.ToString()) ? "1" : mfNumModulesComboBox.SelectedValue.ToString();
        }
        public void UpdateFreePinsInDropDowns(List<MobiFlightPin> newPinList = null)
        {
            // This method is public (and has an optional "new pin list" argument) because it must be called by the
            // embedded MFMultiplexerDriverSubPanel in case its pin data changes.
            bool exInitialized = initialized;
            initialized = false;    // inhibit value_Changed events
            if (newPinList != null)
            {
                pinList = newPinList;
            }
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, pinList, inputMultiplexer.DataPin);
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
            // Here, the config data (inputMultiplexer.DataPin) is updated with the new value read from the changed ComboBox;
            if (comboBox == mfPin1ComboBox) { 
                ComboBoxHelper.reassignPin(mfPin1ComboBox.SelectedItem.ToString(), pinList, ref inputMultiplexer.DataPin); 
            }
            UpdateFreePinsInDropDowns();

            initialized = exInitialized;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;
            if (sender is ComboBox)
                ReassignFreePinsInDropDowns(sender as ComboBox);
            setNonPinValues();
            if (Changed != null)
                Changed(inputMultiplexer, new EventArgs());
        }
    }
}