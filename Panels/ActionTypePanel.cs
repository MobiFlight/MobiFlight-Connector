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
            ActionTypeComboBox.Items.Add("Event ID");
            ActionTypeComboBox.Items.Add("Jeehell DataPipe");
            ActionTypeComboBox.SelectedIndexChanged += new EventHandler(ActionTypeComboBox_SelectedIndexChanged);
        }

        void ActionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActionTypeChanged != null)
                ActionTypeChanged(this, (sender as ComboBox).SelectedItem.ToString());
        }

        internal void syncFromConfig(InputConfig.InputAction inputAction)
        {
            switch (inputAction.GetType().ToString()) {
                case "MobiFlight.InputConfig.FsuipcOffsetInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox,"FSUIPC Offset");
                    break;

                case "MobiFlight.InputConfig.KeyInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, "Key");
                    break;

                case "MobiFlight.InputConfig.EventIdInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, "Event ID");
                    break;
                    
                case "MobiFlight.InputConfig.JeehellInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, "Jeehell DataPipe");
                    break;
            }
        }

        internal void syncToConfig(InputConfig.InputAction inputAction)
        {
            throw new NotImplementedException();
        }
    }
}
