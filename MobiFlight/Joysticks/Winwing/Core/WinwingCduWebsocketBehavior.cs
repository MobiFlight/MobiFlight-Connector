using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using MobiFlightWwFcu;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingCduWebsocketBehavior : WebSocketBehavior
    {
        internal WinwingCduDevice Device { get; set; }
        internal Action<string> ErrorMessageHandler;

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                try
                {
                    Device.SetDisplay(WinwingConstants.CDU_DATA, e.Data);                    
                }
                catch (Exception ex)
                {
                    ErrorMessageHandler($"Error setting {Device.Name} display name='{WinwingConstants.CDU_DATA}'. {ex.Message}.");
                }
            }
        }
    }
}
