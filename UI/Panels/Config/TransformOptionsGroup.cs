using MobiFlight.Base;
using MobiFlight.InputConfig;
using System;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class TransformOptionsGroup : UserControl
    {
        protected Boolean PanelMode = true;

        public TransformOptionsGroup()
        {
            InitializeComponent();
        }

        public void setMode(bool isOutputPanel)
        {
            PanelMode = isOutputPanel;

            // and the value panel vice versa
            // only if we deal with inputs
            ValuePanel.Visible = !PanelMode;
            fsuipcMoreOptionsGroupBox.Visible = !PanelMode;

            AutoSize = isOutputPanel;
        }

        public void ShowValuePanel(bool visible)
        {
            ValuePanel.Visible = visible;
            fsuipcMoreOptionsGroupBox.Visible = visible;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            if (config == null)
            {
                // this happens when casting badly
                return;
            }

            fsuipcValueTextBox.Text = config.Value;
        }

        internal void syncFromConfig(IFsuipcConfigItem config)
        {
            if (config == null)
            {
                // this happens when casting badly
                return;
            }
            fsuipcValueTextBox.Text = config.Value;
        }

        internal void syncToConfig(IConfigItem config)
        {
            config.Value = fsuipcValueTextBox.Text;
        }

        internal void syncToConfig(FsuipcOffsetInputAction config)
        {
            config.Value = fsuipcValueTextBox.Text;
        }
    }
}
