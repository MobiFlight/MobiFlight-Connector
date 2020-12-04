using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
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
            MacroValueTextBox.Text = inputAction.MacroValue.ToString();
        }

        internal InputConfig.InputAction ToConfig()
        {

            MobiFlight.InputConfig.LuaMacroInputAction result = new InputConfig.LuaMacroInputAction();
            result.MacroName = MacroNameTextBox.Text.Trim();
            result.MacroValue = Int32.Parse(MacroValueTextBox.Text);
            return result;
        }
    }


}
