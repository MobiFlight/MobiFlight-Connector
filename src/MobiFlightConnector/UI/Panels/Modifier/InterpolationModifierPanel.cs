using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Modifier.InterpolationModifier;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Modifier
{
    public partial class InterpolationModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler<InterpolationMappingPanel> InvalidXorYvalue;
        public event EventHandler<InterpolationMappingPanel> DuplicateXvalue;
        public event EventHandler ModifierChanged;
        public InterpolationModifierPanel()
        {
            InitializeComponent();
            DuplicateXvalue += InterpolationModifierPanel_DuplicateXvalue;
            InvalidXorYvalue += InterpolationModifierPanel_InvalidXorYvalue;
        }

        public void fromConfig(ModifierBase c)
        {
            Interpolation config = c as Interpolation;
            if (config == null) return;

            panelSequences.Controls.Clear();

            // add a default config
            if (config.GetValues().Count == 0)
            {
                config.Add(0, 0);
                config.Add(100, 1024);
            }

            var i = 0;

            foreach (double Key in config.GetValues().Keys)
            {
                var t = new Tuple<double, double>(Key, config.GetValues()[Key]);
                var p = new InterpolationMappingPanel();
                p.ModifierChanged += value_Changed;
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
            int i = 0;
            try
            {
                for (i = 0; i != panelSequences.Controls.Count; i++)
                {
                    var control = (panelSequences.Controls[i] as InterpolationMappingPanel);
                    var mapping = control.toConfig();

                    if (mapping == null)
                    {
                        InvalidXorYvalue?.Invoke(this, control);
                        continue;
                    }
                    config.Add(mapping.Item1, mapping.Item2);
                }
            }
            catch (XvalueAlreadyExistsException)
            {
                DuplicateXvalue?.Invoke(this, panelSequences.Controls[i] as InterpolationMappingPanel);
            }

            return config;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            InterpolationModifierPanel_Validating(this, new CancelEventArgs());
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var t = toConfig() as Interpolation;
            var p = new InterpolationMappingPanel();
            p.ModifierChanged += value_Changed;
            p.fromConfig(t.NextItem());
            p.Dock = DockStyle.Bottom;
            p.Leave += value_Changed;
            panelSequences.Controls.Add(p);
        }

        private void InterpolationModifierPanel_DuplicateXvalue(object sender, InterpolationMappingPanel e)
        {
            var errorMessage = i18n._tr("uiLabelInterpolationDuplicateXvalueNotAllowed");
            e.SetError(errorMessage);
        }

        private void InterpolationModifierPanel_InvalidXorYvalue(object sender, InterpolationMappingPanel e)
        {
            var errorMessage = i18n._tr("uiLabelDuplicateNotAValidValue");
            e.SetError(errorMessage, true);
        }

        private void InterpolationModifierPanel_Validating(object sender, CancelEventArgs e)
        {
            RemoveAllErrors();
            var config = toConfig() as Interpolation;
            if (config.Count == panelSequences.Controls.Count) return;

            e.Cancel = true;
        }

        private void RemoveAllErrors()
        {
            for (var i = 0; i != panelSequences.Controls.Count; i++)
            {
                (panelSequences.Controls[i] as InterpolationMappingPanel).RemoveError();
            }
        }
    }
}
