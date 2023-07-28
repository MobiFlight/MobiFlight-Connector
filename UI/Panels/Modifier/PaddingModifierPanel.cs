﻿using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Padding = MobiFlight.Modifier.Padding;

namespace MobiFlight.UI.Panels.Modifier
{
    public partial class PaddingModifierPanel : UserControl, IModifierConfigPanel
    {
        public event EventHandler ModifierChanged;

        public PaddingModifierPanel()
        {
            InitializeComponent();

            comboBox1.Leave += control_Leave;
            textBoxCharacter.Leave += control_Leave;
            textBoxCharacter.Leave += control_Leave;
            textBoxLength.Leave += control_Leave;
            textBoxLength.TextChanged += control_Leave;

            comboBox1.Items.Clear();
            var items = new List<ListItem<Padding.PaddingDirection>>() {
                new ListItem<Padding.PaddingDirection>()
                {
                    Value = MobiFlight.Modifier.Padding.PaddingDirection.Left,
                    Label = MobiFlight.Modifier.Padding.PaddingDirection.Left.ToString()
                },
                new ListItem<Padding.PaddingDirection>()
                {
                    Value = MobiFlight.Modifier.Padding.PaddingDirection.Right,
                    Label = MobiFlight.Modifier.Padding.PaddingDirection.Right.ToString()
                }
            };
            comboBox1.DataSource = items;
            comboBox1.ValueMember = "Value";
            comboBox1.DisplayMember = "Label";
        }

        public void fromConfig (ModifierBase c)
        {
            var config = c as Padding;

            if (config == null) return;

            textBoxCharacter.Text = config.Character.ToString();
            textBoxLength.Text = config.Length.ToString();
            ComboBoxHelper.SetSelectedItem(comboBox1, config.Direction.ToString());
        }

        public ModifierBase toConfig () {
            int length = 5;
            int.TryParse(textBoxLength.Text, out length);

            return new Padding()
            {
                Active = true,
                Direction = (Padding.PaddingDirection)comboBox1.SelectedValue,
                Character = textBoxCharacter.Text[0],
                Length = length
            };
        }

        private void control_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
