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
        protected Boolean PanelMode = true;

        public TransformOptionsGroup()
        {
            InitializeComponent();
        }

        public void setMode(bool isOutputPanel)
        {
            PanelMode = isOutputPanel;
            // the transform field only is visible
            // if we are dealing with outputs
            MultiplyPanel.Visible = PanelMode;

            // and the value panel vice versa
            // only if we deal with inputs
            ValuePanel.Visible = !PanelMode;

            AutoSize = isOutputPanel;
        }

        public void ShowMultiplyPanel(bool visible)
        {
            MultiplyPanel.Visible = false && PanelMode;
        }

        public void ShowSubStringPanel(bool visible)
        {
            SubstringPanel.Visible = false && PanelMode;
        }

        public void ShowValuePanel(bool visible)
        {
            ValuePanel.Visible = false && PanelMode;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            if (config == null)
            {
                // this happens when casting badly
                return;
            }

            // multiplier
            if (config.FSUIPC.OffsetType != FSUIPCOffsetType.String)
            {
                TransformationCheckBox.Checked = config.Transform.Active;
                SubstringTransformationCheckBox.Checked = false;
            }
            else
            {
                TransformationCheckBox.Checked = false;
                SubstringTransformationCheckBox.Checked = config.Transform.Active;
            }

            fsuipcMultiplyTextBox.Text = config.Transform.Expression;
            fsuipcValueTextBox.Text = config.Value;

            // substring panel
            SubStringFromTextBox.Text = config.Transform.SubStrStart.ToString();
            SubStringToTextBox.Text = config.Transform.SubStrEnd.ToString();
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            if (config.FSUIPC.OffsetType != FSUIPCOffsetType.String)
            {
                config.Transform.Active = TransformationCheckBox.Checked;
            }
            else
            {
                config.Transform.Active = SubstringTransformationCheckBox.Checked;
            }

            // TODO: refactor this conditional stuff.
            if (fsuipcMultiplyTextBox.Visible)
                config.Transform.Expression = fsuipcMultiplyTextBox.Text;
            if (SubStringFromTextBox.Text != "")
                config.Transform.SubStrStart = Byte.Parse(SubStringFromTextBox.Text);
            if (SubStringToTextBox.Text != "")
                config.Transform.SubStrEnd = Byte.Parse(SubStringToTextBox.Text);
            config.Value = fsuipcValueTextBox.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            fsuipcMultiplyTextBox.Enabled = (sender as CheckBox).Checked;
        }
    }
}
