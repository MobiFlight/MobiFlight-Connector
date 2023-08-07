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
    public partial class SubstringModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler ModifierChanged;

        public SubstringModifierPanel()
        {
            InitializeComponent();
            foreach(var control in new List<Control>() { SubStringToTextBox, SubStringFromTextBox} )
            {
                control.Leave += control_Leave;
                control.TextChanged += control_Leave;
            }
        }

        private void control_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        public void fromConfig(ModifierBase config)
        {
            SubStringFromTextBox.Text = (config as Substring)?.Start.ToString();
            SubStringToTextBox.Text = (config as Substring)?.End.ToString();
        }

        public ModifierBase toConfig()
        {
            return new Substring()
            {
                Start = int.Parse(SubStringFromTextBox.Text),
                End = int.Parse(SubStringToTextBox.Text)
            };
        }
    }
}
