using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Action;
using MobiFlight.UI.Panels.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Input
{
    public partial class ButtonPanel : UserControl
    {
        public event EventHandler<EventArgs> OnPanelChanged;
        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();
        ButtonInputConfig _config;

        Dictionary<ActionTypePanel, string> ActionTypePanelsToActionNames = new Dictionary<ActionTypePanel, string>();
        Dictionary<ActionTypePanel, Panel> ActionTypePanelsToOwnerPanels = new Dictionary<ActionTypePanel, Panel>();

        private void clipBoardActionChanged(InputAction action)
        {
            foreach (ActionTypePanel panel in ActionTypePanelsToActionNames.Keys)
            {
                panel.OnClipBoardChanged(action);
            }
        }

        public new bool Enabled {
            get { return onPressActionConfigPanel.Enabled;  }
            set 
            {
                foreach (var actionTypePanel in ActionTypePanelsToOwnerPanels.Keys)
                {
                    actionTypePanel.Enabled = value;
                    ActionTypePanelsToOwnerPanels[actionTypePanel].Enabled = value;
                }
            }
        }
        public ButtonPanel()
        {
            InitializeComponent();

            longPressDelayLabel.Text = i18n._tr("uiLabelDelay");
            longReleaseDelayLabel.Text = i18n._tr("uiLabelDelay");
            repeatLabel.Text = i18n._tr("uiLabelRepeatPress");

            ActionTypePanelsToActionNames.Add(onPressActionTypePanel, "onPress");
            ActionTypePanelsToActionNames.Add(onReleaseActionTypePanel, "onRelease");
            ActionTypePanelsToActionNames.Add(onLongReleaseActionTypePanel, "onLongRelease");
            ActionTypePanelsToActionNames.Add(onLongPressActionTypePanel, "onLongPress");

            ActionTypePanelsToOwnerPanels.Add(onPressActionTypePanel, onPressActionConfigPanel);
            ActionTypePanelsToOwnerPanels.Add(onReleaseActionTypePanel, onReleaseActionConfigPanel);
            ActionTypePanelsToOwnerPanels.Add(onLongReleaseActionTypePanel, onLongRelActionConfigPanel);
            ActionTypePanelsToOwnerPanels.Add(onLongPressActionTypePanel, onLongPressActionConfigPanel);

            foreach (ActionTypePanel panel in ActionTypePanelsToActionNames.Keys)
            {
                panel.ActionTypeChanged += onPressActionTypePanel_ActionTypeChanged;
                panel.CopyButtonPressed += Action_CopyButtonPressed;
                panel.PasteButtonPressed += Action_PasteButtonPressed;
            }

            Clipboard.Instance.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName != "InputAction") return;

                clipBoardActionChanged(Clipboard.Instance.InputAction);
            };

            // activates the paste button
            // in case that we have something in the clipboard
            if (Clipboard.Instance.InputAction != null)
            {
                clipBoardActionChanged(Clipboard.Instance.InputAction);
            }
        }

        private void Action_CopyButtonPressed(object sender, EventArgs e)
        {
            ButtonInputConfig config = new ButtonInputConfig();
            ToConfig(config);
            string inputActionName = ActionTypePanelsToActionNames[(ActionTypePanel)sender];
            Clipboard.Instance.InputAction = config.GetInputActionByName(inputActionName);
        }

        private void Action_PasteButtonPressed(object sender, EventArgs e)
        {            
            ActionTypePanel actionTypePanel = (ActionTypePanel)sender;
            Panel owner = ActionTypePanelsToOwnerPanels[actionTypePanel];            

            // Set the selected item from config
            actionTypePanel.syncFromConfig(Clipboard.Instance.InputAction);            

            // Create new button config
            ButtonInputConfig config = new ButtonInputConfig();
            string inputActionName = ActionTypePanelsToActionNames[actionTypePanel];
            config.SetInputActionByName(inputActionName, Clipboard.Instance.InputAction);
            
            // Create the right config panel and fill with data
            Type inputActionType = Clipboard.Instance.InputAction.GetType();          
            String inputLabel = InputActionMapping.InputActionTypesToInputLabels[inputActionType];
            UpdatePanelWithAction(owner, config, inputLabel, inputActionName);
        }


        private void UpdatePanelWithAction(Panel owner, ButtonInputConfig config, string inputLabel, string inputActionName)
        {
            this.SuspendLayout();            
            owner.Controls.Clear();  

            // In case "None" is selected, then key is not in dictionary
            if (InputActionMapping.InputLabelsToConfigPanelTypes.ContainsKey(inputLabel))
            {
                Type configPanelType = InputActionMapping.InputLabelsToConfigPanelTypes[inputLabel];

                // Create config panel
                Control panel = (Control)Activator.CreateInstance(configPanelType);

                // Special cases
                if (inputLabel == FsuipcOffsetInputAction.Label)
                {
                    ((dynamic)panel).setMode(false);
                }
                else if (inputLabel == VariableInputAction.Label)
                {
                    ((dynamic)panel).SetVariableReferences(Variables);
                }

                // Sync data from config for inputAction
                InputAction inputAction = config.GetInputActionByName(inputActionName);
                if (config != null && inputAction != null)
                {
                    // Here always the current active inputAction is given as an argument.
                    // When switching, the inputAction probably is of wrong type. That case is handled in each InputPanel.
                    ((IPanelConfigSync)panel).syncFromConfig(inputAction);
                }

                panel.Padding = new Padding(2, 0, 2, 0);
                panel.Dock = DockStyle.Top;

                // Add config panel to ownwer panel
                owner.Controls.Add(panel);
                owner.Dock = DockStyle.Top;
                OnPanelChanged?.Invoke(panel, EventArgs.Empty);
            }
                       
            this.ResumeLayout(true);
        }

        public void SetVariableReferences(Dictionary<String, MobiFlightVariable> variables)
        {
            Variables = variables;
        }

        // On Press Action
        private void onPressActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            string inputLabel = value;
            ActionTypePanel actionTypePanel = (ActionTypePanel)sender;
            Panel owner = ActionTypePanelsToOwnerPanels[actionTypePanel];            
            string inputActionName = ActionTypePanelsToActionNames[actionTypePanel];
            UpdatePanelWithAction(owner, _config, inputLabel, inputActionName);
        }

        public void syncFromConfig(ButtonInputConfig config)
        {
            if (config == null) return;

            _config = config;
            longPressDelayTextBox.Text = config.LongPressDelay.ToString();
            repeatTextBox.Text = config.RepeatDelay.ToString();
            longReleaseTextBox.Text = config.LongReleaseDelay.ToString();

            // Iterate through all ActionTypePanels and read config
            foreach (var actionTypePanel in ActionTypePanelsToOwnerPanels.Keys)
            {
                string inputActionName = ActionTypePanelsToActionNames[actionTypePanel];
                InputAction inputAction = config.GetInputActionByName(inputActionName);
                if (inputAction != null)
                {
                    actionTypePanel.syncFromConfig(inputAction);
                }
            }
        }

        public void ToConfig(ButtonInputConfig config)
        {
            if (!string.IsNullOrEmpty(longPressDelayTextBox.Text))
            {
                _config.LongPressDelay = int.Parse(longPressDelayTextBox.Text);
            }

            if (!string.IsNullOrEmpty(repeatTextBox.Text))
            {
                _config.RepeatDelay = int.Parse(repeatTextBox.Text);
            }

            if (!string.IsNullOrEmpty(longReleaseTextBox.Text))
            {
                _config.LongReleaseDelay = int.Parse(longReleaseTextBox.Text);
            }

            // Iterate through all ActionTypePanels and set config
            foreach (var actionTypePanel in ActionTypePanelsToOwnerPanels.Keys)
            {
                if (actionTypePanel.ActionTypeComboBox.SelectedItem != null)
                {
                    var ownerPanel = ActionTypePanelsToOwnerPanels[actionTypePanel];
                    string inputActionName = ActionTypePanelsToActionNames[actionTypePanel];

                    // Does the ownerPanel contain an action config panel?
                    if (ownerPanel.Controls.Count > 0)
                    {
                        InputAction inputAction = ((IPanelConfigSync)ownerPanel.Controls[0]).ToConfig();                        
                        config.SetInputActionByName(inputActionName, inputAction);
                    }
                    // case "None", reset to Null
                    else
                    {
                        config.SetInputActionByName(inputActionName, null);
                    }
                }
            }
        }

        // Allow only digits
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
