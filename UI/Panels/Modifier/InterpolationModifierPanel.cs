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
    public partial class InterpolationModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler ModifierChanged;
        public InterpolationModifierPanel()
        {
            InitializeComponent();
        }

        public void fromConfig(ModifierBase c)
        {
            var config = c as Interpolation;

            if (config != null)
            {
                interpolationPanel1.syncFromConfig(config);
            }
        }

        public ModifierBase toConfig()
        {
            var config = new Interpolation();
            interpolationPanel1.syncToConfig(config);
            return config;
        }

        private void control_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
