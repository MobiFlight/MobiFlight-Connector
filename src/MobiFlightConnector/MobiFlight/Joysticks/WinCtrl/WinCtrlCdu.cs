using MobiFlight.Joysticks.Cdu;
using MobiFlightWwFcu;
using System;

namespace MobiFlight.Joysticks.WinCtrl
{
    internal class WinCtrlCdu : WinCtrlBaseController, ICduDataConsumer
    {
        public WinCtrlCdu(SharpDX.DirectInput.Joystick joystick, JoystickDefinition def, int productId, ICduWebsocketHub hub) : base(joystick, def, productId, hub)
        {
            // ctor logic is in base class
        }

        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            if (!string.IsNullOrEmpty(DisplayControl.CduWebsocketPath))
            {
                Hub.Register(DisplayControl.CduWebsocketPath, this);
            }
        }

        public override void Shutdown()
        {
            if (!string.IsNullOrEmpty(DisplayControl.CduWebsocketPath))
            {
                Hub.Unregister(DisplayControl.CduWebsocketPath, this);
            }
            base.Shutdown();
        }

        public void OnCduData(string json) => DisplayControl.HandleCduData(json);
        public void OnCduFont(string json) => DisplayControl.HandleCduFont(json);

        public override void ShowUserMessage(int messageCode, params string[] parameters)
        {
            try
            {
                SetLcdDisplay(WinCtrlConstants.CDU_DATA, CduUserMessageFormatter.Format(messageCode, parameters));
            }
            catch (Exception ex)
            {
                Log.Instance.log($"WinCtrlCdu - Error on show user message: {ex.Message}", LogSeverity.Error);
            }
        }
    }
}