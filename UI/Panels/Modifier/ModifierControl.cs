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
    public partial class ModifierControl : UserControl
    {
        public ModifierBase Modifier { get; set; }

        public ModifierControl()
        {
            InitializeComponent();
        }

        public void fromConfig(ModifierBase modifier)
        {
            labelModifier.Text = modifier.ToString();
            checkBoxActive.Checked = modifier.Active;
        }

        public ModifierBase toConfig() { 
            return Modifier;
        }
    }
}
