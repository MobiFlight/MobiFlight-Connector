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

namespace MobiFlight.UI.Panels.Input
{
    public partial class AnalogPanel : UserControl
    {
        public event EventHandler<EventArgs> OnPanelChanged;
        InputConfig.AnalogInputConfig _config;
        Dictionary<String, MobiFlightVariable> Variables = new Dictionary<String, MobiFlightVariable>();

        public new bool Enabled
        {
            get { return onChangeActionTypePanel.Enabled; }
            set
            {
                onChangeActionTypePanel.Enabled = value;
                onChangeActionConfigPanel.Enabled = value;
            }
        }

        public AnalogPanel()
        {
            InitializeComponent();

            onChangeActionTypePanel.ActionTypeChanged += new MobiFlight.UI.Panels.Config.ActionTypePanel.ActionTypePanelSelectHandler(onChangeActionTypePanel_ActionTypeChanged);
            onChangeActionTypePanel.CopyPasteFeatureActive(false);
        }

        // On Press Action
        private void onChangeActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Control panel = null;
            Panel owner = onChangeActionConfigPanel;

            owner.Controls.Clear();
            switch (value)
            {
                case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                    panel = new Panels.Config.FsuipcConfigPanel();
                    (panel as Panels.Config.FsuipcConfigPanel).setMode(false);


                    if (_config != null && _config.onChange != null) 
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(_config.onChange as FsuipcOffsetInputAction);
                    break;

                case InputConfig.KeyInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.KeyboardInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.KeyboardInputPanel).syncFromConfig(_config.onChange as KeyInputAction);

                    break;

                case MobiFlight.InputConfig.EventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.EventIdInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.EventIdInputPanel).syncFromConfig(_config.onChange as EventIdInputAction);
  
                    break;

                case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).syncFromConfig(_config.onChange as PmdgEventIdInputAction);

                    break;

                case InputConfig.JeehellInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.JeehellInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.JeehellInputPanel).syncFromConfig(_config.onChange as JeehellInputAction);

                    break;

                case InputConfig.VJoyInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.VJoyInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.VJoyInputPanel).syncFromConfig(_config.onChange as VJoyInputAction);
                    break;

                case MobiFlight.InputConfig.LuaMacroInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.LuaMacroInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).syncFromConfig(_config.onChange as LuaMacroInputAction);

                    break;

                case MobiFlight.InputConfig.RetriggerInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.RetriggerInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.RetriggerInputPanel).syncFromConfig(_config.onChange as RetriggerInputAction);

                    break;

                case MobiFlight.InputConfig.VariableInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.VariableInputPanel();
                    (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).SetVariableReferences(Variables);
                    if (_config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.VariableInputPanel).syncFromConfig(_config.onChange as VariableInputAction);

                    break;

                // For backward compatibility this is now combined and MSFS2020EventIdInputAction was removed
                case MobiFlight.InputConfig.MSFS2020CustomInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel();
                    if (_config != null && _config.onChange != null)
                    {
                        if (_config.onChange is MSFS2020EventIdInputAction)
                            (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(_config.onChange as MSFS2020EventIdInputAction);
                        else if (_config.onChange is MSFS2020CustomInputAction)
                            (panel as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).syncFromConfig(_config.onChange as MSFS2020CustomInputAction);
                    }
                    break;

                case MobiFlight.InputConfig.XplaneInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.XplaneInputPanel();
                    if (_config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.XplaneInputPanel).syncFromConfig(_config.onChange as XplaneInputAction);

                    break;
            }

            if (panel != null)
            {
                panel.Padding = new Padding(2, 0, 2, 0);
                panel.Dock = DockStyle.Top;
                owner.Controls.Add(panel);
                OnPanelChanged?.Invoke(panel, EventArgs.Empty);
            }
        }

        public void syncFromConfig(InputConfig.AnalogInputConfig config)
        {
            if (config == null) return;

            _config = config;

            if (_config.onChange != null)
            {
                onChangeActionTypePanel.syncFromConfig(_config.onChange);
            }
        }

        public void ToConfig(InputConfig.AnalogInputConfig config)
        {
            // for on press check the action type
            if (onChangeActionTypePanel.ActionTypeComboBox.SelectedItem != null)
            {
                switch (onChangeActionTypePanel.ActionTypeComboBox.SelectedItem.ToString())
                {
                    case MobiFlight.InputConfig.FsuipcOffsetInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Config.FsuipcConfigPanel).ToConfig();
                        break;

                    case InputConfig.KeyInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.KeyboardInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.EventIdInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.EventIdInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).ToConfig();
                        break;

                    case InputConfig.JeehellInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.JeehellInputPanel).ToConfig();
                        break;

                    case InputConfig.VJoyInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.VJoyInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.LuaMacroInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.LuaMacroInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.RetriggerInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.RetriggerInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.MSFS2020EventIdInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.MSFS2020InputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.MSFS2020CustomInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.MSFS2020CustomInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.VariableInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.VariableInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.XplaneInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.XplaneInputPanel).ToConfig();
                        break;

                    default:
                        config.onChange = null;
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
