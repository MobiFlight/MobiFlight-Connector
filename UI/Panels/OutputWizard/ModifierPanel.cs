using MobiFlight.Config;
using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Modifier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.OutputWizard
{
    public partial class ModifierPanel : UserControl
    {
        ModifierList modifierList;

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
            modifierList = config.Modifiers;
            UpdateControls();
        }

        private void UpdateControls()
        {
            var controlList = new List<Control>();

           
            for (var i=modifierList.Items.Count-1; i>=0; i--)
            {
                var modifier = modifierList.Items[i];
                var panel = new ModifierControl();
                panel.fromConfig(modifier);
                panel.ModifierMovedDown += Panel_ModifierMovedDown;
                panel.ModifierMovedUp += Panel_ModifierMovedUp;
                panel.ModifierRemoved += Panel_ModifierRemoved;
                panel.Dock = DockStyle.Top;
                panel.Last = (i == modifierList.Items.Count - 1);
                panel.First = (i == 0);
                if (panel.Last)
                    panel.Selected = true;
                panel.MouseDown += (s, e) => { 
                    DoDragDrop(this, DragDropEffects.Move); 
                };
                controlList.Add(panel);
            }
            modifierListPanel.SuspendLayout();
            modifierListPanel.Controls.Clear();
            modifierListPanel.Controls.AddRange(controlList.ToArray());
            modifierListPanel.ResumeLayout();
        }

        private void Panel_ModifierRemoved(object sender, EventArgs e)
        {
            var index = modifierListPanel.Controls.IndexOf(sender as ModifierControl);

            modifierList.Items.RemoveAt(modifierList.Items.Count - index - 1);
            modifierListPanel.Controls.RemoveAt(index);
            // UpdateControls();
        }

        private void Panel_ModifierMovedUp(object sender, EventArgs e)
        {
            var index = modifierListPanel.Controls.IndexOf(sender as ModifierControl);
            index = modifierList.Items.Count - index - 1;
            var modifier = modifierList.Items[index];

            modifierList.Items.RemoveAt(index);
            modifierList.Items.Insert(index - 1, modifier);

            UpdateControls();
        }

        private void Panel_ModifierMovedDown(object sender, EventArgs e)
        {
            var index = modifierListPanel.Controls.IndexOf(sender as ModifierControl);
            index = modifierList.Items.Count - index - 1;
            var modifier = modifierList.Items[index];

            modifierList.Items.RemoveAt(index);
            modifierList.Items.Insert(index + 1, modifier);

            UpdateControls();
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

            modifier.Active = true;
            modifierList.Items.Add(modifier);
            UpdateControls();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.Show(button1, new Point(0, button1.Height));
        }
    }
}
