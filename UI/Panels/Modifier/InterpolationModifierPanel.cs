using MobiFlight.Config;
using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Modifier.BlinkModifier;
using MobiFlight.UI.Panels.Modifier.InterpolationModifier;
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

            if (config == null) return;

            panelSequences.Controls.Clear();

            if (config.GetValues().Count == 0)
            {
                config.Add(0, 0);
                config.Add(1024, 1024);
            }

            var i = 0;

            foreach (double Key in config.GetValues().Keys)
            {
                var t = new Tuple<double, double>(Key, config.GetValues()[Key]);
                var p = new InterpolationMappingPanel();
                p.fromConfig(t);
                p.Dock = DockStyle.Bottom;
                p.Leave += value_Changed;
                p.ShowDeleteButton = !(i < 2);
                i++;
                panelSequences.Controls.Add(p);
            }

            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        public ModifierBase toConfig()
        {
            var config = new Interpolation();

            for (var i = 0; i != panelSequences.Controls.Count; i++)
            {
                var mapping = (panelSequences.Controls[i] as InterpolationMappingPanel).toConfig();
                config.Add(mapping.Item1, mapping.Item2);
            }
            
            return config;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var t = toConfig() as Interpolation;
            var p = new InterpolationMappingPanel();
            p.fromConfig(t.NextItem());
            p.Dock = DockStyle.Bottom;
            p.Leave += value_Changed;
            panelSequences.Controls.Add(p);
        }
    }
}
