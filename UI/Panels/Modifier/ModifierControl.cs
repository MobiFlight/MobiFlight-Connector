using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
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
                if(value) buttonDown.Text = "";
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
            OriginalColor = BackColor;
            Selected = false;

            foreach (var c in new List<Control>(){ labelModifier, labelModifierType, button1, buttonUp, buttonDown})
            {
                c.MouseEnter += ModifierControl_MouseEnter;
                c.MouseLeave += ModifierControl_MouseLeave;
            }
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

            return null;
        }

        public void fromConfig(ModifierBase modifier)
        {
            Modifier = modifier;

            labelModifierType.Text = modifier.ToString().Replace("MobiFlight.Modifier.", "");
            labelModifier.Text = modifier.ToSummaryLabel();
            checkBoxActive.Checked = modifier.Active;
            panelDetails.Controls.Clear();

            var panel = CreatePanel(modifier.GetType());

            if (panel == null) return;

            panel.Dock = DockStyle.Top;
            (panel as IModifierConfigPanel).fromConfig(modifier);
            (panel as IModifierConfigPanel).ModifierChanged += Panel_ModifierChanged;
            panelDetails.Controls.Add(panel);
        }

        private void Panel_ModifierChanged(object sender, EventArgs e)
        {
            _modifier = (sender as IModifierConfigPanel)?.toConfig();
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
            OriginalColor = BackColor;
            BackColor = Color.AliceBlue;
        }

        private void ModifierControl_MouseLeave(object sender, EventArgs e)
        {
            BackColor = OriginalColor;
        }

        private void control_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            ModifierMovedUp?.Invoke(this, EventArgs.Empty);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            ModifierMovedDown?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModifierRemoved?.Invoke(this, EventArgs.Empty);
        }

        private void checkBoxActive_CheckedChanged(object sender, EventArgs e)
        {
            Modifier.Active = checkBoxActive.Checked;
        }
    }
}
