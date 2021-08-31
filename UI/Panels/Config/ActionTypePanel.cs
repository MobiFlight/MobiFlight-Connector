using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
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
            ActionTypeComboBox.Items.Add(i18n._tr("none"));
            // --MSFS 2020 
            ActionTypeComboBox.Items.Add(InputConfig.MSFS2020EventIdInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.MSFS2020CustomInputAction.Label);

            // --MobiFlight
            ActionTypeComboBox.Items.Add(InputConfig.VariableInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.RetriggerInputAction.Label);

            // --FSUIPC
            ActionTypeComboBox.Items.Add(InputConfig.FsuipcOffsetInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.EventIdInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.PmdgEventIdInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.JeehellInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.LuaMacroInputAction.Label);

            // -- Misc
            ActionTypeComboBox.Items.Add(InputConfig.KeyInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.VJoyInputAction.Label);

            ActionTypeComboBox.SelectedIndex = 0;
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
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.FsuipcOffsetInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.KeyInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.KeyInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.EventIdInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.EventIdInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.PmdgEventIdInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.PmdgEventIdInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.JeehellInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, InputConfig.JeehellInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.VJoyInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, InputConfig.VJoyInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.LuaMacroInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.LuaMacroInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.RetriggerInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.RetriggerInputAction.Label);
                    break;
                case "MobiFlight.InputConfig.MSFS2020EventIdInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.MSFS2020EventIdInputAction.Label);
                    break;
                case "MobiFlight.InputConfig.MSFS2020CustomInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.MSFS2020CustomInputAction.Label);
                    break;
                case "MobiFlight.InputConfig.VariableInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.VariableInputAction.Label);
                    break;
            }
        }

        internal void syncToConfig(InputConfig.InputAction inputAction)
        {
            throw new NotImplementedException();
        }
    }
}
