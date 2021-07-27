using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class MSFS2020CustomInputPanel : UserControl
    {
        ErrorProvider errorProvider = new ErrorProvider();

        public MSFS2020CustomInputPanel()
        {
            InitializeComponent();
        }       

        internal void syncFromConfig(InputConfig.MSFS2020CustomInputAction inputAction)
        {
            if (inputAction == null) return;
            if (inputAction.Command == null) return;

            CommandTextBox.Text = inputAction.Command;
        }

        internal InputConfig.InputAction ToConfig()
        {
            MobiFlight.InputConfig.MSFS2020CustomInputAction result = new InputConfig.MSFS2020CustomInputAction();
            result.Command = CommandTextBox.Text;
            return result;
        }
    }
}
