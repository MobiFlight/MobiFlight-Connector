using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.UI.Panels.Config;
using MobiFlight.InputConfig;

namespace MobiFlight.UI.Panels.Action
{
    public partial class RetriggerInputPanel : UserControl, IPanelConfigSync
    {
        public RetriggerInputPanel()
        {
            InitializeComponent();
        }

        private void _loadPresets()
        {
            bool isLoaded = true;
        }
        
        public void syncFromConfig(object config)
        {
            RetriggerInputAction inputAction = config as RetriggerInputAction;
            if (inputAction == null) return;
        }

        public InputConfig.InputAction ToConfig()
        {
            RetriggerInputAction result = new RetriggerInputAction();
            return result;
        }

    }


}
