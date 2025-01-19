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
            _webView.WebMessageReceived += WebView_WebMessageReceived;
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            if (_webView != null)
            {
                var jsonMessage = JsonConvert.SerializeObject(eventToPublish);
                _webView.PostWebMessageAsString(jsonMessage);
            }
        }

        public void OnMessageReceived(Action<Message<object>> callback)
        {
            _onMessageReceived = (message) => callback((Message<object>)message);
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            var message = args.WebMessageAsJson;
            var decodedMessage = JsonConvert.DeserializeObject<Message<object>>(message);
            Log.Instance.log(decodedMessage.key, LogSeverity.Debug);
            _onMessageReceived?.Invoke(decodedMessage);
        }
    }
}
