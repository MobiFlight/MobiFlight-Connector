using MobiFlight.Config;
using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Modifier.BlinkModifier;
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
    public partial class BlinkModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler ModifierChanged;

        public BlinkModifierPanel()
        {
            InitializeComponent();
            // textBoxBlinkValue.TextChanged += control_Leave;
            // textBoxOnOffSequence.Leave += control_Leave;
        }

        public void fromConfig(ModifierBase c)
        {
            var config = c as Blink;

            if (config == null) return;

            panelSequences.Controls.Clear();

            for(var i=0; i!= config.OnOffSequence.Count; i++)
            {
                var t = new Tuple<int, int>(config.OnOffSequence[i], i + 1 != config.OnOffSequence.Count ? config.OnOffSequence[i + 1] : 0);
                var p = new BlinkSequencePanel();
                p.fromConfig(t);
                p.Dock = DockStyle.Bottom;
                panelSequences.Controls.Add(p);
                i++;
            } 
        }

        public ModifierBase toConfig()
        {
            var onOffSequence = new List<int>();

            for (var i=0; i!= panelSequences.Controls.Count; i++)
            {
                var sequence = (panelSequences.Controls[i] as BlinkSequencePanel).toConfig();
                onOffSequence.Add(sequence.Item1);
                onOffSequence.Add(sequence.Item2);
            }

            return new Blink()
            {
                Active = true,
                BlinkValue = textBoxBlinkValue.Text,
                OnOffSequence = onOffSequence
            };
        }
        private void control_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var t = new Tuple<int, int>(500, 500);
            var p = new BlinkSequencePanel();
            p.fromConfig(t);
            p.Dock = DockStyle.Bottom;
            panelSequences.Controls.Add(p);
        }
    }
}
