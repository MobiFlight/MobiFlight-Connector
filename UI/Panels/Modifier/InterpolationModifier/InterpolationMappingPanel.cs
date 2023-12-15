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
        private readonly ErrorProvider ErrorProvider = new ErrorProvider();
        public event EventHandler ModifierChanged;
        Color DefaultBackgroundColor;
        public bool ShowDeleteButton { 
            get { return button1.Visible; } 
            set { button1.Visible = value; } 
        }
        public InterpolationMappingPanel()
        {
            InitializeComponent();
            textBoxFromValue.Leave += Value_Changed;
            textBoxToValue.Leave += Value_Changed;
        }

        private void Value_Changed(object sender, EventArgs e)
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
            if (!double.TryParse(textBoxFromValue.Text, out double from) || 
                !double.TryParse(textBoxToValue.Text, out double to)) 
                return null;

            return new Tuple<double, double>(from, to);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        internal void SetError(string v, bool markBothFields = false)
        {
            // Just do this once.
            if (ErrorProvider.GetError(this.textBoxFromValue) != "") return;

            DefaultBackgroundColor = this.textBoxFromValue.BackColor;
            
            this.textBoxFromValue.BackColor = Color.LightPink;
            ErrorProvider.SetError(this.textBoxFromValue, v);

            if (!markBothFields) return;

            this.textBoxToValue.BackColor = Color.LightPink;
            //ErrorProvider.SetError(this.textBoxToValue, v);
        }

        internal void RemoveError()
        {
            this.textBoxFromValue.BackColor = DefaultBackgroundColor;
            this.textBoxToValue.BackColor = DefaultBackgroundColor;
            ErrorProvider.SetError(this.textBoxFromValue, "");
            ErrorProvider.SetError(this.textBoxToValue, "");
        }

        private void TextBoxFromValue_Leave(object sender, EventArgs e)
        {
            ModifierChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
