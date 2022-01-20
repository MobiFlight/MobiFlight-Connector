using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFDigInputMuxPanel : UserControl
    {
        private DigInputMux         _digInputMux;
        private MFMuxDriverPanel    _selectorPanel; 
        private bool                _initialized;
        public event EventHandler   Changed;
        private int MAX_MODULES = 2;            // Only possible values: 1 module for HCT4051, 2 for HCT4067
        private const string NA_STRING = "N/A";

        public MFDigInputMuxPanel()
        {
            InitializeComponent();
            mfPin1ComboBox.Items.Clear();
        }

        public MFDigInputMuxPanel(DigInputMux digInputMux, List<MobiFlightPin> Pins, bool isFirstMuxed) : this()
        {
            ComboBoxHelper.BindMobiFlightFreePins(mfPin1ComboBox, Pins, digInputMux.DataPin);

            if (mfPin1ComboBox.Items.Count > 2)
            {
                mfPin1ComboBox.SelectedIndex = 0;
            }

            for (int i = 1; i <= MAX_MODULES; i++)
            {
                mfNumModulesComboBox.Items.Add(i);
            }

            this._digInputMux = digInputMux;

            ComboBoxHelper.SetSelectedItem(mfPin1ComboBox, digInputMux.DataPin);
            ComboBoxHelper.SetSelectedItem(mfNumModulesComboBox, digInputMux.NumModules);

            textBox1.Text = digInputMux.Name;

            _selectorPanel = new MFMuxDriverPanel(digInputMux.Selector, Pins, isFirstMuxed);
            _selectorPanel.Changed += this.Changed;
            muxDrvPanel.Controls.Add(_selectorPanel);

            //TODO: _selectorPanel.Enabled = (this is first mux client)

            _initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!_initialized) return;

            setValues();

            if (Changed != null)
                Changed(_digInputMux, new EventArgs());
        }

        private void setValues()
        {
            _digInputMux.DataPin = mfPin1ComboBox.Text;
            _digInputMux.Name = textBox1.Text;
            _digInputMux.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
        }
    }
}