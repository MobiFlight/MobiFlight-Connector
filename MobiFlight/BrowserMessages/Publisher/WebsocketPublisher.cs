using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.BrowserMessages.Publisher
{

    public class WebSocketPublisher : IMessagePublisher
    {
        private readonly ClientWebSocket _webSocket;
        private Action<object> _onMessageReceived;

        public WebSocketPublisher(string uri)
        {
            _webSocket = new ClientWebSocket();
            Task.Run(() => ConnectAsync(uri));
        }

        private async Task ConnectAsync(string uri)
        {
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            await ReceiveMessagesAsync();
        }

        public void OnMessageReceived(Action<string> action)
        {
            _onMessageReceived = (message) => action((string) message);
        }

        public async void Publish<TEvent>(TEvent eventToPublish)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                
                var jsonMessage = JsonConvert.SerializeObject(eventToPublish);
                var bytes = Encoding.UTF8.GetBytes(jsonMessage);
                var buffer = new ArraySegment<byte>(bytes);

                await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _onMessageReceived?.Invoke(message);
                }
            }
        }
    }
}