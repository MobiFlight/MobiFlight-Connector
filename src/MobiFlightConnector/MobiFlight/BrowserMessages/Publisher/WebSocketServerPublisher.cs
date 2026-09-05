using MobiFlight.BrowserMessages.Transport;
using Newtonsoft.Json;
using System;

namespace MobiFlight.BrowserMessages.Publisher
{
    /// <summary>IMessagePublisher backed by MessageServer. Broadcasts to every connected client.</summary>
    public class WebSocketServerPublisher : IMessagePublisher
    {
        private readonly MessageServer _server;
        private Action<object> _onMessageReceived;

        public WebSocketServerPublisher(MessageServer server)
        {
            _server = server;
            _server.MessageReceived += message => _onMessageReceived?.Invoke(message);
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            var message = new Message<TEvent>(eventToPublish);
            var jsonMessage = JsonConvert.SerializeObject(message);
            _server.Broadcast(jsonMessage);
        }

        public void OnMessageReceived(Action<string> callback)
        {
            _onMessageReceived = (message) => callback((string)message);
        }
    }
}
