using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Modifier.BlinkModifier
{
    public partial class BlinkSequencePanel : UserControl
    {
        public event EventHandler ModifierChanged;

        public BlinkSequencePanel()
        {
            InitializeComponent();
            textBoxOnTime.Leave += TextBox_Leave;
            textBoxOffTime.Leave += TextBox_Leave;
            textBoxOnTime.TextChanged += TextBox_Leave;
            textBoxOffTime.TextChanged += TextBox_Leave;
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        public void fromConfig(Tuple<int, int> sequence)
        {
            if (sequence == null) return;
            textBoxOnTime.Text = sequence.Item1.ToString();
            textBoxOffTime.Text = sequence.Item2.ToString();
        }

        public Tuple<int, int> toConfig()
        {
            int on, off;
            if (!int.TryParse(textBoxOnTime.Text, out on) || 
                !int.TryParse(textBoxOffTime.Text, out off)) 
                return null;

            return new Tuple<int, int>(on, off);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
