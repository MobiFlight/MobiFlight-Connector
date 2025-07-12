using System;
using System.Windows.Forms;
using MobiFlight.InputConfig;

namespace MobiFlight.UI.Panels.Action
{
    public partial class ProSimActionInputPanel : UserControl
    {
        public event EventHandler Changed;
        ErrorProvider errorProvider = new ErrorProvider();

        public ProSimActionInputPanel()
        {
            InitializeComponent();
        }

        public bool Validate()
        {
            bool isValid = true;

            if (PathTextBox.Text.Trim() == "")
            {
                errorProvider.SetError(PathTextBox, "A DataRef path is required.");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(PathTextBox, "");
            }

            return isValid;
        }

        public void ToConfig(ProSimInputAction config)
        {
            config.Path = PathTextBox.Text;
            config.Expression = ExpressionTextBox.Text;
        }

        public void FromConfig(ProSimInputAction config)
        {
            PathTextBox.Text = config.Path;
            ExpressionTextBox.Text = config.Expression;
        }

        private void PathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        private void ExpressionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Changed != null)
                Changed(this, new EventArgs());
        }
    }
} 