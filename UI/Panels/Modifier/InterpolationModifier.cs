using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Modifier
{
    public partial class InterpolationModifier : UserControl
    {
        public InterpolationModifier()
        {
            InitializeComponent();
        }

        public void fromConfig(Interpolation config)
        {
            if (config != null)
            {
                interpolationCheckBox.Checked = config.Active;
                interpolationPanel1.syncFromConfig(config);
            }
        }

        public void toConfig(Interpolation config)
        {

        }
    }
}
