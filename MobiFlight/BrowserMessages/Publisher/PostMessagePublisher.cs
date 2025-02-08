using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace MobiFlight.BrowserMessages.Publisher
{
    public class PostMessagePublisher : IMessagePublisher
    {
        private readonly ThreadSafeWebView2 _webView;
        private Action<object> _onMessageReceived;

        public PostMessagePublisher(ThreadSafeWebView2 webView)
        {
            _webView = webView;
            _webView.WebMessageReceived += WebView_WebMessageReceived;
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            if (_webView != null)
            {
                var message = new Message<TEvent>() { key = eventToPublish.GetType().Name, payload = eventToPublish };
                var jsonMessage = JsonConvert.SerializeObject(message);
                // Ensure the call is made on the UI thread
                _webView.PostWebMessageAsJsonThreadSafe(jsonMessage);
            }
        }

        public void OnMessageReceived(Action<string> callback)
        {
            _onMessageReceived = (message) => callback((string)message);
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            var message = args.WebMessageAsJson;
            _onMessageReceived?.Invoke(message);
        }
    }
}
