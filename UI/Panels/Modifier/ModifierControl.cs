using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Modifier
{
    public interface IModifierConfigPanel
    {
        event EventHandler ModifierChanged;
        void fromConfig(ModifierBase modifier);
        ModifierBase toConfig();
    }
    public partial class ModifierControl : UserControl
    {
        public event EventHandler ModifierChanged;
        public event EventHandler ModifierMovedUp;
        public event EventHandler ModifierMovedDown;
        public event EventHandler ModifierRemoved;
        public Timer HighlightTimer = new Timer();

        private bool selected;
        Color OriginalColor;
        private ModifierBase _modifier;

        public bool Selected
        {
            get { return selected; }
            set {
                selected = value;
                panelDetails.Visible = value;
                if (value) labelModifier.Text = "Editing:";
                else labelModifier.Text = _modifier?.ToSummaryLabel();
            }
        }

        private bool isFirst;
        public bool First
        {
            get { return isFirst; }
            set
            {
                isFirst = value;
                buttonUp.Enabled = !value;
                if (value) buttonUp.Text = "";
                else buttonUp.Text = "▲";
            }
        }

        private bool isLast;
        public bool Last
        {
            get { return isLast; }
            set
            {
                isLast = value;
                buttonDown.Enabled = !value;
                if (value) buttonDown.Text = "";
                else buttonDown.Text = "▼";
            }
        }

        public ModifierBase Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        public ModifierControl()
        {
            InitializeComponent();
            Selected = false;

            foreach (var c in new List<Control>(){ labelModifier, labelModifierType, button1, buttonUp, buttonDown})
            {
                c.MouseEnter += ModifierControl_MouseEnter;
                c.MouseLeave += ModifierControl_MouseLeave;
            }

            HighlightTimer.Interval = 750;
            HighlightTimer.Tick += (s, e) =>
            {
                HighlightTimer.Stop();
                BackColor = OriginalColor;
            };
        }

        private Control CreatePanel(Type modifierType)
        {
            if (modifierType == typeof(Transformation))
                return new TransformModifierPanel();

            if (modifierType == typeof(Comparison))
                return new ComparisonModifierPanel();

            if (modifierType == typeof(Interpolation))
                return new InterpolationModifierPanel();

            if (modifierType == typeof(MobiFlight.Modifier.Padding))
                return new PaddingModifierPanel();

            if (modifierType == typeof(Blink))
                return new BlinkModifierPanel();

            if (modifierType == typeof(Substring))
                return new SubstringModifierPanel();

            return null;
        }


        private Color CreateColor(Type modifierType)
        {
            if (modifierType == typeof(Transformation))
                return Color.FromArgb(0,0,0);

            if (modifierType == typeof(Comparison))
                return Color.FromArgb(230, 159, 0);

            if (modifierType == typeof(Interpolation))
                return Color.FromArgb(86, 180, 233);

            if (modifierType == typeof(MobiFlight.Modifier.Padding))
                return Color.FromArgb(0, 158, 115);

            if (modifierType == typeof(Blink))
                return Color.FromArgb(240, 228, 66);

            if (modifierType == typeof(Substring))
                return Color.FromArgb(0, 114, 178);

            // more colors for the future 
            // Color.FromArgb(213, 94, 0)
            // Color.FromArgb(204, 121, 167)

            return Color.White;
        }

        public void fromConfig(ModifierBase modifier)
        {
            Modifier = modifier;

            labelModifierType.Text = modifier.ToString().Replace("MobiFlight.Modifier.", "");
            labelModifier.Text = modifier.ToSummaryLabel();
            checkBoxActive.Checked = modifier.Active;
            panelDetails.Controls.Clear();
            panelColor.BackColor = CreateColor(modifier.GetType());

            var panel = CreatePanel(modifier.GetType());

            if (panel == null) return;

            panel.Dock = DockStyle.Top;
            (panel as IModifierConfigPanel).ModifierChanged += Panel_ModifierChanged;
            (panel as IModifierConfigPanel).fromConfig(modifier);
            panelDetails.Controls.Add(panel);
        }

        private void Panel_ModifierChanged(object sender, EventArgs e)
        {
            _modifier = (sender as IModifierConfigPanel)?.toConfig();
            _modifier.Active = checkBoxActive.Checked;
            labelModifier.Text = _modifier.ToSummaryLabel();
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        public ModifierBase toConfig() { 
            return Modifier;
        }

        private void ModifierControl_MouseUp(object sender, MouseEventArgs e)
        {
            Selected = !Selected;
        }

        private void ModifierControl_MouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.AliceBlue;
        }

        private void ModifierControl_MouseLeave(object sender, EventArgs e)
        {
            if (HighlightTimer.Enabled) return;
            BackColor = OriginalColor;
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            HighlightTimer.Start();
            ModifierMovedUp?.Invoke(this, EventArgs.Empty);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            HighlightTimer.Start();
            ModifierMovedDown?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModifierRemoved?.Invoke(this, EventArgs.Empty);
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            Modifier.Active = checkBoxActive.Checked;
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        private void buttonDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (BackColor != Color.AliceBlue) OriginalColor = BackColor;
            BackColor = Color.LightSkyBlue;
        }
    }
}
