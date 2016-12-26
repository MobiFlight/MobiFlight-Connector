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
    public partial class LuaMacroInputPanel : UserControl
    {
        public LuaMacroInputPanel()
        {
            InitializeComponent();
        }

        private void _loadPresets()
        {
            bool isLoaded = true;
        }
        
        internal void syncFromConfig(InputConfig.LuaMacroInputAction inputAction)
        {
            if (inputAction == null) return;
            MacroNameTextBox.Text = inputAction.MacroName;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.LuaMacroInputAction result = new InputConfig.LuaMacroInputAction();
            result.MacroName = MacroNameTextBox.Text.Trim();
            return result;
        }

    }


}
