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
                control.Leave += value_Changed;
                control.TextChanged += value_Changed;
            }
        }

        private void value_Changed(object sender, EventArgs e)
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
            if (!int.TryParse(SubStringFromTextBox.Text, out int startPosition)) return new Substring();
            if (!int.TryParse(SubStringToTextBox.Text,out int endPosition)) return new Substring();

            return new Substring()
            {
                Start = startPosition,
                End = endPosition
            };
        }
    }
}
