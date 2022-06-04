using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;
using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Config;
using MobiFlight.UI.Panels.Action;

namespace MobiFlight.UI.Panels.Input
{
    public partial class EncoderPanel : UserControl
    {
        InputConfig.EncoderInputConfig _config;
        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();


        private void clipBoardActionChanged(InputAction action)
        {
            onLeftActionTypePanel.OnClipBoardChanged(action);
            onLeftFastActionTypePanel.OnClipBoardChanged(action);
            onRightActionTypePanel.OnClipBoardChanged(action);
            onRightFastActionTypePanel.OnClipBoardChanged(action);
        }

        public EncoderPanel()
        {
            InitializeComponent();
            onLeftActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onLeftFastActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onRightActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);
            onRightFastActionTypePanel.ActionTypeChanged += new ActionTypePanel.ActionTypePanelSelectHandler(onPressActionTypePanel_ActionTypeChanged);

            List<ActionTypePanel> panels = new List<ActionTypePanel>() { onLeftActionTypePanel, onLeftFastActionTypePanel, onRightActionTypePanel, onRightFastActionTypePanel };
            panels.ForEach(action =>
            {
                action.CopyButtonPressed += Action_CopyButtonPressed;
                action.PasteButtonPressed += Action_PasteButtonPressed;
            });

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

        private void Action_PasteButtonPressed(object sender, EventArgs e)
        {
            Panel owner = onLeftActionConfigPanel;
            
            (sender as ActionTypePanel).syncFromConfig(Clipboard.Instance.InputAction);
            
            bool isLeft = ((sender as ActionTypePanel) == onLeftActionTypePanel) || ((sender as ActionTypePanel) == onLeftFastActionTypePanel);
            bool isFast = ((sender as ActionTypePanel) == onLeftFastActionTypePanel) || ((sender as ActionTypePanel) == onRightFastActionTypePanel);

            InputConfig.EncoderInputConfig config = new EncoderInputConfig();
            if (isLeft) {
                if (isFast) 
                { 
                    owner = onLeftFastActionConfigPanel; config.onLeftFast = Clipboard.Instance.InputAction; 
                }
                else
                {
                    owner = onLeftActionConfigPanel; config.onLeft = Clipboard.Instance.InputAction;
                }
            } else {
                if (isFast)
                {
                    owner = onRightFastActionConfigPanel; config.onRightFast = Clipboard.Instance.InputAction;
                }
                else
                {
                    owner = onRightActionConfigPanel; config.onRight = Clipboard.Instance.InputAction;
                }
            }

            String value = null;
            String type = Clipboard.Instance.InputAction.GetType().ToString();

            if (type == "MobiFlight.InputConfig.FsuipcOffsetInputAction") value = MobiFlight.InputConfig.FsuipcOffsetInputAction.Label;
            else if (type == "MobiFlight.InputConfig.KeyInputAction") value = MobiFlight.InputConfig.KeyInputAction.Label;
            else if (type == "MobiFlight.InputConfig.EventIdInputAction") value = MobiFlight.InputConfig.EventIdInputAction.Label;
            else if (type == "MobiFlight.InputConfig.PmdgEventIdInputAction") value = MobiFlight.InputConfig.PmdgEventIdInputAction.Label;
            else if (type == "MobiFlight.InputConfig.JeehellInputAction") value = MobiFlight.InputConfig.JeehellInputAction.Label;
            else if (type == "MobiFlight.InputConfig.VJoyInputAction") value = MobiFlight.InputConfig.VJoyInputAction.Label;
            else if (type == "MobiFlight.InputConfig.LuaMacroInputAction") value = MobiFlight.InputConfig.LuaMacroInputAction.Label;
            else if (type == "MobiFlight.InputConfig.RetriggerInputAction") value = MobiFlight.InputConfig.RetriggerInputAction.Label;
            else if (type == "MobiFlight.InputConfig.MSFS2020EventIdInputAction") value = MobiFlight.InputConfig.MSFS2020CustomInputAction.Label;
            else if (type == "MobiFlight.InputConfig.MSFS2020CustomInputAction") value = MobiFlight.InputConfig.MSFS2020CustomInputAction.Label;
            else if (type == "MobiFlight.InputConfig.VariableInputAction") value = MobiFlight.InputConfig.VariableInputAction.Label;
            else if (type == "MobiFlight.InputConfig.XplaneInputAction") value = MobiFlight.InputConfig.XplaneInputAction.Label;

            UpdatePanelWithAction(owner, config, value , isLeft, isFast);
        }
         
        private void Action_CopyButtonPressed(object sender, EventArgs e)
        {
            InputConfig.EncoderInputConfig config = new EncoderInputConfig();
            ToConfig(config);
            bool isLeft = ((sender as ActionTypePanel) == onLeftActionTypePanel) || ((sender as ActionTypePanel) == onLeftFastActionTypePanel);
            bool isFast = ((sender as ActionTypePanel) == onLeftFastActionTypePanel) || ((sender as ActionTypePanel) == onRightFastActionTypePanel);
            if (isLeft)
            {
                if (isFast) Clipboard.Instance.InputAction = config.onLeftFast;
                else Clipboard.Instance.InputAction = config.onLeft;
            }
            else
            {
                if (isFast) Clipboard.Instance.InputAction = config.onRightFast;
                else Clipboard.Instance.InputAction = config.onRight;
            }
        }

        // On Press Action
        private void onPressActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Panel owner = onLeftActionConfigPanel;
            bool isLeft = ((sender as ActionTypePanel) == onLeftActionTypePanel) || ((sender as ActionTypePanel) == onLeftFastActionTypePanel);
            bool isFast = ((sender as ActionTypePanel) == onLeftFastActionTypePanel) || ((sender as ActionTypePanel) == onRightFastActionTypePanel);

            if (isLeft && isFast) owner = onLeftFastActionConfigPanel;
            else if (!isLeft)
            {
                owner = onRightActionConfigPanel;
                if (isFast) owner = onRightFastActionConfigPanel;
            }

            UpdatePanelWithAction(owner, _config, value, isLeft, isFast);
        }

        private void UpdatePanelWithAction(Panel owner, EncoderInputConfig config, String value, bool isLeft, bool isFast)
        {
            this.SuspendLayout();
            Control panel = null;
            owner.Controls.Clear();

            switch (value)
            {
                case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                    panel = new Panels.Config.FsuipcConfigPanel();
                    (panel as Panels.Config.FsuipcConfigPanel).setMode(false);

                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(config.onLeft as FsuipcOffsetInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(config.onLeftFast as FsuipcOffsetInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(config.onRight as FsuipcOffsetInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(config.onRightFast as FsuipcOffsetInputAction);
                    break;

                case InputConfig.KeyInputAction.Label:
                    panel = new KeyboardInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as KeyboardInputPanel).syncFromConfig(config.onLeft as KeyInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as KeyboardInputPanel).syncFromConfig(config.onLeftFast as KeyInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as KeyboardInputPanel).syncFromConfig(config.onRight as KeyInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as KeyboardInputPanel).syncFromConfig(config.onRightFast as KeyInputAction);
                    break;

                case MobiFlight.InputConfig.EventIdInputAction.Label:
                    panel = new EventIdInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as EventIdInputPanel).syncFromConfig(config.onLeft as EventIdInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as EventIdInputPanel).syncFromConfig(config.onLeftFast as EventIdInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as EventIdInputPanel).syncFromConfig(config.onRight as EventIdInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as EventIdInputPanel).syncFromConfig(config.onRightFast as EventIdInputAction);
                    break;

                case PmdgEventIdInputAction.Label:
                    panel = new PmdgEventIdInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as PmdgEventIdInputPanel).syncFromConfig(config.onLeft as PmdgEventIdInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as PmdgEventIdInputPanel).syncFromConfig(config.onLeftFast as PmdgEventIdInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as PmdgEventIdInputPanel).syncFromConfig(config.onRight as PmdgEventIdInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as PmdgEventIdInputPanel).syncFromConfig(config.onRightFast as PmdgEventIdInputAction);
                    break;

                case InputConfig.JeehellInputAction.Label:
                    panel = new JeehellInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as JeehellInputPanel).syncFromConfig(config.onLeft as JeehellInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as JeehellInputPanel).syncFromConfig(config.onLeftFast as JeehellInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as JeehellInputPanel).syncFromConfig(config.onRight as JeehellInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as JeehellInputPanel).syncFromConfig(config.onRightFast as JeehellInputAction);
                    break;

                case LuaMacroInputAction.Label:
                    panel = new LuaMacroInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as LuaMacroInputPanel).syncFromConfig(config.onLeft as LuaMacroInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as LuaMacroInputPanel).syncFromConfig(config.onLeftFast as LuaMacroInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as LuaMacroInputPanel).syncFromConfig(config.onRight as LuaMacroInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as LuaMacroInputPanel).syncFromConfig(config.onRightFast as LuaMacroInputAction);
                    break;

                case RetriggerInputAction.Label:
                    panel = new RetriggerInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as RetriggerInputPanel).syncFromConfig(config.onLeft as RetriggerInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as RetriggerInputPanel).syncFromConfig(config.onLeftFast as RetriggerInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as RetriggerInputPanel).syncFromConfig(config.onRight as RetriggerInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as RetriggerInputPanel).syncFromConfig(config.onRightFast as RetriggerInputAction);
                    break;

                case VariableInputAction.Label:
                    panel = new VariableInputPanel();
                    
                    (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).SetVariableReferences(Variables);
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as VariableInputPanel).syncFromConfig(config.onLeft as VariableInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as VariableInputPanel).syncFromConfig(config.onLeftFast as VariableInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as VariableInputPanel).syncFromConfig(config.onRight as VariableInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as VariableInputPanel).syncFromConfig(config.onRightFast as VariableInputAction);
                    break;

                // For backward compatibility this is now combined and MSFS2020EventIdInputAction was removed
                case MSFS2020CustomInputAction.Label:
                    panel = new MSFS2020CustomInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                    {
                        if (config.onLeft is MSFS2020CustomInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onLeft as MSFS2020CustomInputAction);
                        else if (config.onLeft is MSFS2020EventIdInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onLeft as MSFS2020EventIdInputAction);
                    }
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                    {
                        if (config.onLeftFast is MSFS2020CustomInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onLeftFast as MSFS2020CustomInputAction);
                        else if (config.onLeftFast is MSFS2020EventIdInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onLeftFast as MSFS2020EventIdInputAction);
                    }
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                    {
                        if (config.onRight is MSFS2020CustomInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onRight as MSFS2020CustomInputAction);
                        else if (config.onRight is MSFS2020EventIdInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onRight as MSFS2020EventIdInputAction);
                    }
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                    {
                        if (config.onRightFast is MSFS2020CustomInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onRightFast as MSFS2020CustomInputAction);
                        else if (config.onRightFast is MSFS2020EventIdInputAction)
                            (panel as MSFS2020CustomInputPanel).syncFromConfig(config.onRightFast as MSFS2020EventIdInputAction);
                    }
                    break;

                case XplaneInputAction.Label:
                    panel = new XplaneInputPanel();
                    if (isLeft && !isFast && config != null && config.onLeft != null)
                        (panel as XplaneInputPanel).syncFromConfig(config.onLeft as XplaneInputAction);
                    else if (isLeft && isFast && config != null && config.onLeftFast != null)
                        (panel as XplaneInputPanel).syncFromConfig(config.onLeftFast as XplaneInputAction);
                    else if (!isLeft && !isFast && config != null && config.onRight != null)
                        (panel as XplaneInputPanel).syncFromConfig(config.onRight as XplaneInputAction);
                    else if (!isLeft && isFast && config != null && config.onRightFast != null)
                        (panel as XplaneInputPanel).syncFromConfig(config.onRightFast as XplaneInputAction);
                    break;
            }

            if (panel != null)
            {
                panel.Padding = new Padding(2, 0, 2, 0);
                panel.Dock = DockStyle.Fill;
                owner.Controls.Add(panel);
            }

            this.ResumeLayout(true);
        }

        public void syncFromConfig(InputConfig.EncoderInputConfig config)
        {
            if (config == null) return;

            _config = config;

            if (_config.onLeft != null)
            {
                onLeftActionTypePanel.syncFromConfig(_config.onLeft);
            }

            if (_config.onLeftFast != null)
            {
                onLeftFastActionTypePanel.syncFromConfig(_config.onLeftFast);
            }

            if (_config.onRight != null)
            {
                onRightActionTypePanel.syncFromConfig(_config.onRight);
            }

            if (_config.onRightFast != null)
            {
                onRightFastActionTypePanel.syncFromConfig(_config.onRightFast);
            }
        }

        public void ToConfig(InputConfig.EncoderInputConfig config)
        {
            // for on press check the action type
            if (onLeftActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onLeftActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case InputConfig.KeyInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as KeyboardInputPanel).ToConfig();
                        break;
                    case MobiFlight.InputConfig.EventIdInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as EventIdInputPanel).ToConfig();
                        break;
                    case PmdgEventIdInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as PmdgEventIdInputPanel).ToConfig();
                        break;
                    case InputConfig.JeehellInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as JeehellInputPanel).ToConfig();
                        break;
                    case LuaMacroInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as LuaMacroInputPanel).ToConfig();
                        break;
                    case RetriggerInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as RetriggerInputPanel).ToConfig();
                        break;
                    case MSFS2020EventIdInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as MSFS2020InputPanel).ToConfig();
                        break;
                    case MSFS2020CustomInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as MSFS2020CustomInputPanel).ToConfig();
                        break;
                    case VariableInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as VariableInputPanel).ToConfig();
                        break;
                    case XplaneInputAction.Label:
                        config.onLeft = (onLeftActionConfigPanel.Controls[0] as XplaneInputPanel).ToConfig();
                        break;
                    default:
                        config.onLeft = null;
                        break;
                }
            }

            // for on fast press check the action type
            if (onLeftFastActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onLeftFastActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case InputConfig.KeyInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as KeyboardInputPanel).ToConfig();
                        break;
                    case MobiFlight.InputConfig.EventIdInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as EventIdInputPanel).ToConfig();
                        break;
                    case PmdgEventIdInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as PmdgEventIdInputPanel).ToConfig();
                        break;
                    case InputConfig.JeehellInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as JeehellInputPanel).ToConfig();
                        break;
                    case LuaMacroInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as LuaMacroInputPanel).ToConfig();
                        break;

                    case RetriggerInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as RetriggerInputPanel).ToConfig();
                        break;
                    case MSFS2020EventIdInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as MSFS2020InputPanel).ToConfig();
                        break;

                    case MSFS2020CustomInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as MSFS2020CustomInputPanel).ToConfig();
                        break;

                    case VariableInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as VariableInputPanel).ToConfig();
                        break;

                    case XplaneInputAction.Label:
                        config.onLeftFast = (onLeftFastActionConfigPanel.Controls[0] as XplaneInputPanel).ToConfig();
                        break;

                    default:
                        config.onLeftFast = null;
                        break;
                }
            }

            if (onRightActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onRightActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case InputConfig.KeyInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as KeyboardInputPanel).ToConfig();
                        break;
                    case MobiFlight.InputConfig.EventIdInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as EventIdInputPanel).ToConfig();
                        break;
                    case PmdgEventIdInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as PmdgEventIdInputPanel).ToConfig();
                        break;
                    case InputConfig.JeehellInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as JeehellInputPanel).ToConfig();
                        break;
                    case LuaMacroInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as LuaMacroInputPanel).ToConfig();
                        break;
                    case RetriggerInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as RetriggerInputPanel).ToConfig();
                        break;
                    case MSFS2020EventIdInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as MSFS2020InputPanel).ToConfig();
                        break;
                    case MSFS2020CustomInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as MSFS2020CustomInputPanel).ToConfig();
                        break;
                    case VariableInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as VariableInputPanel).ToConfig();
                        break;
                    case XplaneInputAction.Label:
                        config.onRight = (onRightActionConfigPanel.Controls[0] as XplaneInputPanel).ToConfig();
                        break;

                    default:
                        config.onRight = null;
                        break;
                }
            }

            if (onRightFastActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onRightFastActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as FsuipcConfigPanel).ToConfig();
                        break;
                    case InputConfig.KeyInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as KeyboardInputPanel).ToConfig();
                        break;
                    case MobiFlight.InputConfig.EventIdInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as EventIdInputPanel).ToConfig();
                        break;
                    case PmdgEventIdInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as PmdgEventIdInputPanel).ToConfig();
                        break;
                    case InputConfig.JeehellInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as JeehellInputPanel).ToConfig();
                        break;
                    case LuaMacroInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as LuaMacroInputPanel).ToConfig();
                        break;
                    case RetriggerInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as RetriggerInputPanel).ToConfig();
                        break;
                    case MSFS2020EventIdInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as MSFS2020InputPanel).ToConfig();
                        break;
                    case MSFS2020CustomInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as MSFS2020CustomInputPanel).ToConfig();
                        break;
                    case VariableInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as VariableInputPanel).ToConfig();
                        break;
                    case XplaneInputAction.Label:
                        config.onRightFast = (onRightFastActionConfigPanel.Controls[0] as XplaneInputPanel).ToConfig();
                        break;

                    default:
                        config.onRightFast = null;
                        break;
                }
            }
        }

        public void SetVariableReferences(Dictionary<String, MobiFlightVariable> variables)
        {
            Variables = variables;
        }

    }
}
