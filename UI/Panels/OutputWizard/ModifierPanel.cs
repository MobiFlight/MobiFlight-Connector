using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Modifier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.OutputWizard
{
    public partial class ModifierPanel : UserControl
    {
        public event EventHandler ModifierChanged;

        ModifierList modifierList = new ModifierList();

        public ModifierPanel()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            modifierListPanel.Controls.Clear();
        }

        public void fromConfig(OutputConfigItem config)
        {
            modifierList = config.Modifiers.Clone() as ModifierList;
            UpdateControls();
        }

        public void UpdateModifier(ModifierBase modifier)
        {
            modifierList.Items?.Clear();
            modifierList.Items.Add(modifier);
            UpdateControls();
        }

        private void UpdateControls()
        {
            var controlList = new List<Control>();
            modifierListPanel.SuspendLayout();
            modifierListPanel.Controls.Clear();

            for (var i=modifierList.Items.Count-1; i>=0; i--)
            {
                var modifier = modifierList.Items[i];
                AddModifierControl(modifierListPanel.Controls, i, modifier);
            }
            modifierListPanel.ResumeLayout();
        }

        private void AddModifierControl(ControlCollection controlList, int i, ModifierBase modifier, bool prepend = false)
        {
            modifierListPanel.SuspendLayout();
            var panel = new ModifierControl();
            panel.fromConfig(modifier);
            panel.ModifierMovedDown += Panel_ModifierMovedDown;
            panel.ModifierMovedUp += Panel_ModifierMovedUp;
            panel.ModifierRemoved += Panel_ModifierRemoved;
            panel.ModifierChanged += Panel_ModifierChanged;
            panel.Dock = DockStyle.Top;
            panel.Last = (i == modifierList.Items.Count - 1);
            panel.First = (i == 0);
            panel.MouseDown += (s, e) =>
            {
                DoDragDrop(this, DragDropEffects.Move);
            };
            controlList.Add(panel);

            if (prepend)
            {
                controlList.SetChildIndex(panel, 0);
            }
            modifierListPanel.ResumeLayout();
        }

        private void Panel_ModifierChanged(object sender, EventArgs e)
        {
            // sender is the panel
            if (sender as ModifierControl == null) return;
            ModifierControl panel = (ModifierControl)sender;

            var index = modifierListPanel.Controls.GetChildIndex(panel);
            var count = modifierList.Items.Count;
            modifierList.Items[count - index - 1] = panel.Modifier;

            //ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateArrows()
        {
            var controlList = modifierListPanel.Controls;

            for (var i = 0; i < controlList.Count; i++)
            {
                var panel = controlList[i] as ModifierControl;
                panel.SuspendLayout();
                panel.First = (i == controlList.Count - 1);
                panel.Last = (i == 0);
                panel.ResumeLayout();
            }
        }

        private void Panel_ModifierRemoved(object sender, EventArgs e)
        {
            var index = modifierListPanel.Controls.IndexOf(sender as ModifierControl);

            modifierList.Items.RemoveAt(modifierList.Items.Count - index - 1);
            modifierListPanel.Controls.RemoveAt(index);
            UpdateArrows();
        }

        private void Panel_ModifierMovedUp(object sender, EventArgs e)
        {
            var index = modifierListPanel.Controls.IndexOf(sender as ModifierControl);
            var modifierIndex = modifierList.Items.Count - index - 1;
            var modifier = modifierList.Items[modifierIndex];

            modifierList.Items.RemoveAt(modifierIndex);
            modifierList.Items.Insert(modifierIndex - 1, modifier);
            (modifierListPanel.Controls[index+1] as ModifierControl).First = true;
            (modifierListPanel.Controls[index+1] as ModifierControl).Last = true;
            (modifierListPanel.Controls[index] as ModifierControl).First = true;
            (modifierListPanel.Controls[index] as ModifierControl).Last = true;
            modifierListPanel.Controls.SetChildIndex(modifierListPanel.Controls[index], index + 1);
            UpdateArrows();
        }

        private void Panel_ModifierMovedDown(object sender, EventArgs e)
        {
            var index = modifierListPanel.Controls.IndexOf(sender as ModifierControl);
            var modifierIndex = modifierList.Items.Count - index - 1;
            var modifier = modifierList.Items[modifierIndex];

            modifierList.Items.RemoveAt(modifierIndex);
            modifierList.Items.Insert(modifierIndex + 1, modifier);
            (modifierListPanel.Controls[index-1] as ModifierControl).First = true;
            (modifierListPanel.Controls[index-1] as ModifierControl).Last = true;
            (modifierListPanel.Controls[index] as ModifierControl).First = true;
            (modifierListPanel.Controls[index] as ModifierControl).Last = true;
            modifierListPanel.Controls.SetChildIndex(modifierListPanel.Controls[index], index - 1);
            UpdateArrows();
        }

        public void toConfig(OutputConfigItem config)
        {
            config.Modifiers = modifierList;
        }

        private void modifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifierBase modifier = new Transformation();

            if (sender == compareToolStripMenuItem)
            {
                modifier = new Comparison();
            } else if (sender == interpolationToolStripMenuItem)
            {
                modifier = new Interpolation();
            } else if (sender == blinkToolStripMenuItem) { 
                modifier = new Blink();
            }
            else if (sender == paddingToolStripMenuItem)
            {
                modifier = new MobiFlight.Modifier.Padding();
            }
            else if (sender == substringToolStripMenuItem)
            {
                modifier = new MobiFlight.Modifier.Substring();
            }

            modifier.Active = true;
            modifierList.Items.Add(modifier);
            AddModifierControl(modifierListPanel.Controls, modifierList.Items.Count-1, modifier, true);
            UpdateArrows();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.Show(button1, new Point(0, button1.Height));
        }
    }
}
