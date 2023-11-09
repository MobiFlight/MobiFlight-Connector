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

namespace MobiFlight.UI.Panels.Modifier.InterpolationModifier
{
    public partial class InterpolationMappingPanel : UserControl
    {
        public event EventHandler ModifierChanged;
        public bool ShowDeleteButton { 
            get { return button1.Visible; } 
            set { button1.Visible = value; } 
        }
        public InterpolationMappingPanel()
        {
            InitializeComponent();
            textBoxFromValue.Leave += value_Changed;
            textBoxToValue.Leave += value_Changed;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }

        public void fromConfig(Tuple<double, double> sequence)
        {
            if (sequence == null) return;
            textBoxFromValue.Text = sequence.Item1.ToString();
            textBoxToValue.Text = sequence.Item2.ToString();
        }

        public Tuple<double, double> toConfig()
        {
            if (!int.TryParse(textBoxFromValue.Text, out int on) || 
                !int.TryParse(textBoxToValue.Text, out int off)) 
                return null;

            return new Tuple<double, double>(on, off);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
