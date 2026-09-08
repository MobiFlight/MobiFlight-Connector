using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Threading.Tasks;

namespace MobiFlight.WebView
{
    public class WebView2Adapter : WebView2, IWebView2Adapter
    {
        // IWebView2Adapter implementation
        string IWebView2Adapter.Source => CoreWebView2?.Source ?? string.Empty;

        async Task<string> IWebView2Adapter.ExecuteScriptAsync(string script)
        {
            return await CoreWebView2?.ExecuteScriptAsync(script);
        }

        event EventHandler<CoreWebView2NavigationCompletedEventArgs> IWebView2Adapter.NavigationCompleted
        {
            add => CoreWebView2.NavigationCompleted += value;
            remove => CoreWebView2.NavigationCompleted -= value;
        }
    }
}