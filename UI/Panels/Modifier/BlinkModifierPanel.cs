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
    public partial class BlinkModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler ModifierChanged;

        public BlinkModifierPanel()
        {
            InitializeComponent();
            textBoxBlinkValue.TextChanged += control_Leave;
            textBoxOnOffSequence.Leave += control_Leave;
        }

        public void fromConfig(ModifierBase c)
        {
            var config = c as Blink;

            if (config == null) return;

            textBoxBlinkValue.Text = config.BlinkValue;
            textBoxOnOffSequence.Text = String.Join(",", config.OnOffSequence);
        }

        public ModifierBase toConfig()
        {
            var onOffSequence = new List<int>();
            textBoxOnOffSequence.Text.Split(',').ToList().ForEach(e =>
            {
                int value;
                if (!int.TryParse(e.Trim(), out value)) return;

                onOffSequence.Add(value);
            });

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
    }
}
