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
        InputConfig.AnalogInputConfig _config;

        public AnalogPanel()
        {
            InitializeComponent();

            onChangeActionTypePanel.ActionTypeChanged += new MobiFlight.UI.Panels.Config.ActionTypePanel.ActionTypePanelSelectHandler(onChangeActionTypePanel_ActionTypeChanged);
            
        }

        // On Press Action
        private void onChangeActionTypePanel_ActionTypeChanged(object sender, String value)
        {
            Control panel = null;
            Panel owner = onChangeActionConfigPanel;

            owner.Controls.Clear();
            switch (value)
            {
                case "FSUIPC Offset":
                    panel = new Panels.Config.FsuipcConfigPanel();
                    (panel as Panels.Config.FsuipcConfigPanel).setMode(false);


                    if (_config != null && _config.onChange != null) 
                        (panel as Panels.Config.FsuipcConfigPanel).syncFromConfig(_config.onChange as FsuipcOffsetInputAction);
                    break;

                case "Key":
                    panel = new MobiFlight.UI.Panels.Action.KeyboardInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.KeyboardInputPanel).syncFromConfig(_config.onChange as KeyInputAction);

                    break;

                case "Event ID":
                    panel = new MobiFlight.UI.Panels.Action.EventIdInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.EventIdInputPanel).syncFromConfig(_config.onChange as EventIdInputAction);
  
                    break;

                case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).syncFromConfig(_config.onChange as PmdgEventIdInputAction);

                    break;

                case "Jeehell DataPipe":
                    panel = new MobiFlight.UI.Panels.Action.JeehellInputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.JeehellInputPanel).syncFromConfig(_config.onChange as JeehellInputAction);

                    break;

                case "vJoy virtual Joystick":
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

                case MobiFlight.InputConfig.MSFS2020EventIdInputAction.Label:
                    panel = new MobiFlight.UI.Panels.Action.MSFS2020InputPanel();
                    if ( _config != null && _config.onChange != null)
                        (panel as MobiFlight.UI.Panels.Action.MSFS2020InputPanel).syncFromConfig(_config.onChange as MSFS2020EventIdInputAction);

                    break;
            }

            if (panel != null)
            {
                panel.Padding = new Padding(0, 0, 0, 0);
                panel.Width = owner.Width;
                owner.Controls.Add(panel);
                panel.Height = owner.Height - 3;
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
                    case "FSUIPC Offset":
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Config.FsuipcConfigPanel).ToConfig();
                        break;

                    case "Key":
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.KeyboardInputPanel).ToConfig();
                        break;

                    case "Event ID":
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.EventIdInputPanel).ToConfig();
                        break;

                    case MobiFlight.InputConfig.PmdgEventIdInputAction.Label:
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.PmdgEventIdInputPanel).ToConfig();
                        break;

                    case "Jeehell DataPipe":
                        config.onChange = (onChangeActionConfigPanel.Controls[0] as MobiFlight.UI.Panels.Action.JeehellInputPanel).ToConfig();
                        break;

                    case "vJoy virtual Joystick":
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

                    default:
                        config.onChange = null;
                        break;
                }
            }         
        }
    }
}
