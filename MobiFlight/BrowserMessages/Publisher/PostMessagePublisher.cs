using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;

namespace MobiFlight.BrowserMessages.Publisher
{
    public class PostMessagePublisher : IMessagePublisher
    {
        private readonly CoreWebView2 _webView;
        private Action<object> _onMessageReceived;

        public PostMessagePublisher(CoreWebView2 webView)
        {
            _webView = webView;
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            if (_webView != null)
            {
                var jsonMessage = JsonConvert.SerializeObject(eventToPublish);
                _webView.PostWebMessageAsString(jsonMessage);
            }
        }

        public void OnMessageReceived<TEvent>(Action<TEvent> action)
        {
            _onMessageReceived = (message) => action((TEvent)message);
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            var message = args.TryGetWebMessageAsString();
            var eventType = typeof(object); // Determine the event type from the message
            var eventData = JsonConvert.DeserializeObject(message, eventType);
            _onMessageReceived?.Invoke(eventData);
        }
    }
}
