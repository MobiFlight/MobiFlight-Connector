using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class ProSimInputPanel : UserControl, IPanelConfigSync
    {
        public ProSimInputPanel()
        {
            InitializeComponent();
        }

        public void syncFromConfig(object config)
        {
            ProSimInputAction inputAction = config as ProSimInputAction;
            proSimDatarefPanel1.Path = inputAction.Path;
            textBox2.Text = inputAction.Expression;

        }

        public InputAction ToConfig()
        {
            return new ProSimInputAction
            {

                Path = proSimDatarefPanel1.Path.Trim(),
                Expression = textBox2.Text.Trim(),
            };
        }
    }
}
