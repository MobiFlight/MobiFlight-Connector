using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Modifier
{
    public partial class TransformModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler ModifierChanged;

        public TransformModifierPanel()
        {
            InitializeComponent();
            expressionTextBox.Leave += control_Leave;
            expressionTextBox.TextChanged += control_Leave;
        }

        private void control_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        public void fromConfig(ModifierBase config)
        {
            expressionTextBox.Text = (config as Transformation)?.Expression;
        }

        public ModifierBase toConfig()
        {
            return new Transformation()
            {
                Expression = expressionTextBox.Text
            };
        }
    }
}
