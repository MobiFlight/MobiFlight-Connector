using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.Panels
{
    public partial class MFModulePanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private MobiFlightModuleInfo module;
        bool initialized = false;

        public MFModulePanel()
        {
            InitializeComponent();
        }

        public MFModulePanel(MobiFlightModuleInfo module)
            : this()
        {
            // TODO: Complete member initialization
            this.module = module;
            textBox1.Text = module.Name;
            FirmwareValueLabel.Text = module.Version;
            SerialValueLabel.Text = module.Serial;
            TypeValueLabel.Text = module.Type.ToString();
            PortValueLabel.Text = module.Port;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            // module.Name = textBox1.Text;

            if (Changed != null)
                Changed(module, new EventArgs());
        }
    }
}
