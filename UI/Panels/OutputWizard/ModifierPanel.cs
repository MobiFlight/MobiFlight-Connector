using Microsoft.Web.WebView2.Core;
using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Modifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.OutputWizard
{
    public partial class ModifierPanel : UserControl
    {
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
            foreach (var modifier in config.Modifiers.Items)
            {
                var panel = new ModifierControl();
                panel.fromConfig(modifier);
            }
        }

        public void toConfig(OutputConfigItem config)
        {
            /* 
             * // refactor!!!
            comparisonPanel_syncToConfig();

            if(interpolationPanel1.Save) {
                // backward compatibility until we have refactored the 
                // multipliers in the UI
                if (config.Interpolation == null) { config.Interpolation = new Modifier.Interpolation(); }
                config.Interpolation.Active = interpolationCheckBox.Checked;
                interpolationPanel1.syncToConfig(config.Interpolation);
            }
*/
        }
    }
}
