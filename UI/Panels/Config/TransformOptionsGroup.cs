using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class TransformOptionsGroup : UserControl
    {
        public event EventHandler ModifyTabLink;
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

        internal void syncToConfig(OutputConfigItem config)
        {
            config.Value = fsuipcValueTextBox.Text;
        }

        internal void syncToConfig(IFsuipcConfigItem config)
        {
            config.Value = fsuipcValueTextBox.Text;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            ModifyTabLink?.Invoke(this, EventArgs.Empty);
        }
    }
}
