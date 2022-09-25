using EmbedIO.WebSockets;
using MobiFlight.RestWebSocketApi.Messages;
using Newtonsoft.Json;
using Swan;
using Swan.Formatters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.RestWebSocketApi
{

    internal class WebSocketApiServer : EmbedIO.WebSockets.WebSocketModule
    {

        private readonly MessageProcessor messageProcessor;

        public WebSocketApiServer(string urlPath, bool enableConnectionWatchdog, MessageProcessor aMessageProcessor): base(urlPath, enableConnectionWatchdog)
        {
            messageProcessor = aMessageProcessor;
        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            string msg = Encoding.GetString(buffer);
            return SendAsync(context, messageProcessor.ProcessJsonMessage(msg));
        }

        public void OutputChanged(string name, string value)
        {
            BroadcastAsync(new OutputChangedMessage(name, value).Serialize());
        }
    }    
}
