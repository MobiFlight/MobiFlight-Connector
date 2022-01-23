using MobiFlight.Config;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class MFDigInputMuxPanel : UserControl
    {
        private bool                _initialized;
        private DigInputMux         _digInputMux;
        private MFMuxDriverSubPanel _selectorPanel; 
        
        private int MAX_MODULES = 2;            // Only possible values: 1 module for HCT4051, 2 for HCT4067
        private const string NA_STRING = "N/A";
        
        public event EventHandler   Changed;
        public event EventHandler   MoveToFirstMux;

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

            _selectorPanel = new MFMuxDriverSubPanel(digInputMux.Selector, Pins, isFirstMuxed);
            _selectorPanel.Changed += this.Changed;
            _selectorPanel.MoveToFirstMux += new EventHandler(gotoToFirstMux);

            muxDrvPanel.Controls.Add(_selectorPanel);
            _selectorPanel.Dock = DockStyle.Fill;

            _initialized = true;
        }
        private void gotoToFirstMux(object sender, EventArgs e) 
        {
            this.MoveToFirstMux(sender, e);
        }

        private void setValues()
        {
            _digInputMux.DataPin = mfPin1ComboBox.Text;
            _digInputMux.Name = textBox1.Text;
            _digInputMux.NumModules = string.IsNullOrEmpty(mfNumModulesComboBox.Text) ? "1" : mfNumModulesComboBox.Text;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!_initialized) return;

            setValues();

            if (Changed != null)
                Changed(_digInputMux, new EventArgs());
        }
    }
}