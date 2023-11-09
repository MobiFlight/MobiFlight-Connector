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
            var blankOptions = new List<ListItem>()
            {
                new ListItem() { Label = "0", Value = "0" },
                new ListItem() { Label = "1", Value = "1" },
                new ListItem() { Label = "[Space]", Value = " " },
            };

            comboBoxBlinkValue.DataSource = blankOptions;
            comboBoxBlinkValue.ValueMember = "Value";
            comboBoxBlinkValue.DisplayMember = "Label";

            comboBoxBlinkValue.SelectedIndexChanged += value_Changed;
            comboBoxBlinkValue.TextChanged += value_Changed;
        }

        public void fromConfig(ModifierBase c)
        {
            var config = c as Blink;

            if (config == null) return;

            panelSequences.Controls.Clear();

            if (config.OnOffSequence.Count == 0)
            {
                config.OnOffSequence.Add(500);
                config.OnOffSequence.Add(500);
            }

            for(var i=0; i!= config.OnOffSequence.Count; i++)
            {
                var t = new Tuple<int, int>(config.OnOffSequence[i], i + 1 != config.OnOffSequence.Count ? config.OnOffSequence[i + 1] : 0);
                var p = new BlinkSequencePanel();
                p.fromConfig(t);
                p.Dock = DockStyle.Bottom;
                p.ModifierChanged += value_Changed;
                panelSequences.Controls.Add(p);
                i++;
            }

            if (!ComboBoxHelper.SetSelectedItemByValue(comboBoxBlinkValue, config.BlinkValue))
            {
                var list = (comboBoxBlinkValue.DataSource as List<ListItem>);
                list.Add(new ListItem() { Label = config.BlinkValue, Value = config.BlinkValue });
                comboBoxBlinkValue.DataSource = null;
                comboBoxBlinkValue.DataSource = list;
                comboBoxBlinkValue.ValueMember = "Value";
                comboBoxBlinkValue.DisplayMember = "Label";
                comboBoxBlinkValue.SelectedItem = config.BlinkValue;
                ComboBoxHelper.SetSelectedItemByValue(comboBoxBlinkValue, config.BlinkValue);
            };
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

            var BlinkValue = comboBoxBlinkValue.Text;
            if (comboBoxBlinkValue.SelectedItem != null)
                BlinkValue = (comboBoxBlinkValue.SelectedItem as ListItem).Value;

            return new Blink()
            {
                Active = true,
                BlinkValue = BlinkValue,
                OnOffSequence = onOffSequence
            };
        }
        private void value_Changed(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var t = new Tuple<int, int>(500, 500);
            var p = new BlinkSequencePanel();
            p.fromConfig(t);
            p.Dock = DockStyle.Bottom;
            p.ModifierChanged += value_Changed;
            panelSequences.Controls.Add(p);
        }
    }
}
