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
    public partial class ActionTypePanel : UserControl
    {
        public delegate void ActionTypePanelSelectHandler(object sender, String selectedValue);
        [Browsable(true)]
        public event ActionTypePanelSelectHandler ActionTypeChanged;

        public ActionTypePanel()
        {
            InitializeComponent();
            ActionTypeComboBox.Items.Clear();
            ActionTypeComboBox.Items.Add(MainForm._tr("none"));
            ActionTypeComboBox.Items.Add("FSUIPC Offset");
            ActionTypeComboBox.SelectedIndex = 0;
            //ActionTypeComboBox.Items.Add("FSUIPC Macro");
            ActionTypeComboBox.Items.Add("Key");
            ActionTypeComboBox.SelectedIndexChanged += new EventHandler(ActionTypeComboBox_SelectedIndexChanged);
        }

        void ActionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActionTypeChanged != null)
                ActionTypeChanged(this, (sender as ComboBox).SelectedItem.ToString());
        }

        internal void syncFromConfig(MobiFlight.InputConfig.InputAction inputAction)
        {
            switch (inputAction.GetType().ToString()) {
                case "MobiFlight.InputConfig.FsuipcOffsetInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox,"FSUIPC Offset");
                    break;

                case "MobiFlight.InputConfig.KeyInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, "Key");
                    break;
            }
        }

        internal void syncToConfig(MobiFlight.InputConfig.InputAction inputAction)
        {
            throw new NotImplementedException();
        }
    }
}
