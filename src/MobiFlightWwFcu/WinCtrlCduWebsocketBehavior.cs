using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MobiFlightWwFcu
{
    internal class WinCtrlCduWebsocketBehavior : WebSocketBehavior
    {
        internal IWinCtrlController Controller { get; set; }
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
                        displayName = WinCtrlConstants.CDU_DATA;
                        Controller.SetDisplay(displayName, e.Data);
                    }
                    else if (e.Data.Contains("Font"))
                    {
                        displayName = WinCtrlConstants.FONT_DATA;
                        Loader.LoadFont(Controller, e.Data);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessageHandler($"Error setting {Controller.Name} display name='{displayName}'. {ex.Message}.");
                }
            }
        }
    }
}

