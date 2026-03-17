using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.UI.Panels.Config;
using MobiFlight.InputConfig;

namespace MobiFlight.UI.Panels.Action
{
    public partial class LuaMacroInputPanel : UserControl, IPanelConfigSync
    {
        public LuaMacroInputPanel()
        {
            InitializeComponent();
        }

        private void _loadPresets()
        {
            bool isLoaded = true;
        }
        
        public void syncFromConfig(object config)
        {
            LuaMacroInputAction inputAction = config as LuaMacroInputAction;
            if (inputAction == null) return;
            
            MacroNameTextBox.Text = inputAction.MacroName;
            MacroValueTextBox.Text = inputAction.MacroValue.ToString();
        }

        public InputConfig.InputAction ToConfig()
        {
            LuaMacroInputAction result = new LuaMacroInputAction();
            result.MacroName = MacroNameTextBox.Text.Trim();
            result.MacroValue = MacroValueTextBox.Text;
            return result;
        }
    }


}
