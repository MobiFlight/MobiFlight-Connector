using WebSocketSharp;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Cdu
{
    /// <summary>
    /// One websocket connection on a CDU path. Just enough parsing to route the message -
    /// "Display" vs "Font" is the wire contract's own field value, unchanged from what the
    /// Winwing scripts already send - then hands it to the hub to fan out.
    /// </summary>
    /// <example>
    /// { "Target": "Display", "Data": [ [], [], ... ] }
    /// { "Target": "Font", "Data": "Airbus" }
    /// </example>
    internal class CduWebsocketBehavior : WebSocketBehavior
    {
        internal ICduWebsocketHub Hub { get; set; }
        internal string Path { get; set; }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (!e.IsText) return;

            if (e.Data.Contains("Display"))
            {
                Hub.BroadcastData(Path, e.Data);
            }
            else if (e.Data.Contains("Font"))
            {
                Hub.BroadcastFont(Path, e.Data);
            }
        }
    }
}
