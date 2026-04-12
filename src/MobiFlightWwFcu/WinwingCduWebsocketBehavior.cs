using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MobiFlightWwFcu
{
    internal class WinwingCduWebsocketBehavior : WebSocketBehavior
    {
        internal IWinwingDevice Device { get; set; }
        internal Action<string> ErrorMessageHandler;
        internal FontLoader Loader { get;  set; }

        //{ "Target": "Display",
        //  "Data": [ [], [], [], [] ] }

        // { "Target": "Font",
        //   "Data": "Airbus" }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                string displayName = string.Empty;
                try
                {
                    // Check for Data or for Font                    
                    if (e.Data.Contains("Display"))
                    {
                        displayName = WinwingConstants.CDU_DATA;
                        Device.SetDisplay(displayName, e.Data);
                    }
                    else if (e.Data.Contains("Font"))
                    {
                        displayName = WinwingConstants.FONT_DATA;
                        Loader.LoadFont(Device, e.Data);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessageHandler($"Error setting {Device.Name} display name='{displayName}'. {ex.Message}.");
                }
            }
        }
    }
}

