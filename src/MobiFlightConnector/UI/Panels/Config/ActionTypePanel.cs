using MobiFlight.Base;
using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class ActionTypePanel : UserControl
    {
        public delegate void ActionTypePanelSelectHandler(object sender, String selectedValue);
        [Browsable(true)]
        public event ActionTypePanelSelectHandler ActionTypeChanged;
        public event EventHandler CopyButtonPressed;
        public event EventHandler PasteButtonPressed;

        private ProjectInfo _projectInfo;
        public ProjectInfo ProjectInfo
        {
            get { return _projectInfo; }
            set
            {
                if (_projectInfo == value) return;
                _projectInfo = value;
                InitActionTypeComboBox(_projectInfo);
            }
        }

        public InputConfigItem CurrentConfig { 
            get; 
            set; 
        }

        public ActionTypePanel()
        {
            InitializeComponent();
            InitActionTypeComboBox();
        }

        private void InitActionTypeComboBox(ProjectInfo projectInfo = null)
        {

            ActionTypeComboBox.Items.Clear();
            ActionTypeComboBox.Items.Add(i18n._tr("none"));

            var sim = projectInfo?.Sim?.Trim().ToLower();
            var showAllOptions = projectInfo == null;

            var currentConfigUsesFsuipc = ConfigFile.ContainsConfigOfSourceType(new List<IConfigItem>() { CurrentConfig }, new FsuipcSource());
            var currentConfigUsesMsfs = ConfigFile.ContainsConfigOfSourceType(new List<IConfigItem>() { CurrentConfig }, new SimConnectSource());
            var currentConfigUsesXplane = ConfigFile.ContainsConfigOfSourceType(new List<IConfigItem>() { CurrentConfig }, new XplaneSource());
            var currentConfigUsesProsim = ConfigFile.ContainsConfigOfSourceType(new List<IConfigItem>() { CurrentConfig }, new ProSimSource());

            var showFsuipcOptions = (projectInfo?.Features?.FSUIPC ?? false) || sim == "fsx" || sim == "p3d" || currentConfigUsesFsuipc;


            if (showAllOptions || sim == "msfs" || currentConfigUsesMsfs)
            {
                // --MSFS 2020 
                ActionTypeComboBox.Items.Add(InputConfig.MSFS2020CustomInputAction.Label);
            }

            if (showAllOptions || sim == "xplane" || currentConfigUsesXplane)
            {
                // -- Xplane
                ActionTypeComboBox.Items.Add(InputConfig.XplaneInputAction.Label);
            }

            // --MobiFlight
            ActionTypeComboBox.Items.Add(InputConfig.VariableInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.RetriggerInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.KeyInputAction.Label);
            ActionTypeComboBox.Items.Add(InputConfig.VJoyInputAction.Label);

            if (showAllOptions || showFsuipcOptions)
            {
                // --FSUIPC
                ActionTypeComboBox.Items.Add(InputConfig.FsuipcOffsetInputAction.Label);
                ActionTypeComboBox.Items.Add(InputConfig.EventIdInputAction.Label);
                ActionTypeComboBox.Items.Add(InputConfig.PmdgEventIdInputAction.Label);
                ActionTypeComboBox.Items.Add(InputConfig.JeehellInputAction.Label);
                ActionTypeComboBox.Items.Add(InputConfig.LuaMacroInputAction.Label);
            }

            if (showAllOptions || (projectInfo?.Features?.ProSim ?? false) || currentConfigUsesProsim)
            {
                ActionTypeComboBox.Items.Add(InputConfig.ProSimInputAction.Label);
            }

            ActionTypeComboBox.SelectedIndex = 0;
            ActionTypeComboBox.SelectedIndexChanged += new EventHandler(ActionTypeComboBox_SelectedIndexChanged);
        }

        public void CopyPasteFeatureActive(bool value)
        {
            CopyButton.Visible = value;
            PasteButton.Visible = value;
        }

        void ActionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActionTypeChanged != null)
                ActionTypeChanged(this, (sender as ComboBox).SelectedItem.ToString());
        }

        internal void syncFromConfig(InputConfig.InputAction inputAction)
        {
            switch (inputAction.GetType().ToString())
            {
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
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.MSFS2020CustomInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.MSFS2020CustomInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.MSFS2020CustomInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.VariableInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.VariableInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.XplaneInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.XplaneInputAction.Label);
                    break;

                case "MobiFlight.InputConfig.ProSimInputAction":
                    ComboBoxHelper.SetSelectedItem(ActionTypeComboBox, MobiFlight.InputConfig.ProSimInputAction.Label);
                    break;
            }
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            CopyButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            PasteButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        public void OnClipBoardChanged(InputAction value)
        {
            PasteButton.Enabled = value != null;
        }
    }
}
