using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class RetriggerInputPanel : UserControl
    {
        public RetriggerInputPanel()
        {
            InitializeComponent();
        }

        private void _loadPresets()
        {
            bool isLoaded = true;
        }
        
        internal void syncFromConfig(InputConfig.RetriggerInputAction inputAction)
        {
            if (inputAction == null) return;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.RetriggerInputAction result = new InputConfig.RetriggerInputAction();
            return result;
        }

    }


}
