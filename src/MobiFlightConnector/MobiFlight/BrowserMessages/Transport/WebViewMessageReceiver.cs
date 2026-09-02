using MobiFlight.WebView;
using System;

namespace MobiFlight.BrowserMessages.Transport
{
    /// <summary>Inbound-only bridge for a WebView2 that stays on postMessage (the auth WebView). Wire MessageReceived to MessageExchange.PublishReceivedMessage.</summary>
    public class WebViewMessageReceiver
    {
        public event Action<string> MessageReceived;

        public WebViewMessageReceiver(ThreadSafeWebView2 webView)
        {
            webView.WebMessageReceived += (sender, args) => MessageReceived?.Invoke(args.WebMessageAsJson);
        }
    }
}
