using Microsoft.Web.WebView2.Core;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.WebView
{
    internal class AddCloseButtonHandlerOnNavigationCompleted
    {
        List<string> exclusionFilters = new List<string>();
        private IWebView2Adapter webViewAdapter;

        // Left arrow from tabler icons (IconChevronLeft)
        // https://tabler.io/icons/icon/chevron-left
        const string IconBack = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""32"" height=""32"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" class=""icon icon-tabler icons-tabler-outline icon-tabler-chevron-left""><path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""/><path d=""M15 6l-6 6l6 6"" /></svg>";

        internal void AddExclusionFilter(string urlSubstring)
        {
            exclusionFilters.Add(urlSubstring);
        }

        internal void RegisterWithWebView(IWebView2Adapter webView)
        {   
            webViewAdapter = webView;
            webView.NavigationCompleted += NavigationCompleted;
        }

        private async void NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (webViewAdapter == null) return;

            if (exclusionFilters.Any(filter => webViewAdapter.Source.Contains(filter)))
            {
                return;
            }

            // Inject close button
            var script = @"
                (function() {
                    // Create close button
                    const closeBtn = document.createElement('div');
                    closeBtn.innerHTML = `<div style=""margin-left: -5px;"">[[IconBack]]<div>`;
                    closeBtn.style.cssText = `
                        position: fixed;
                        top: 10px;
                        left: 10px;
                        width: 48px;
                        height: 48px;
                        color: rgb(255, 255, 255);
                        background-color: rgba(0, 0, 0, 0.5);
                        border: none;
                        border-radius: 50%;
                        font-size: 40px;
                        line-height: 48px;
                        text-align: center;
                        cursor: pointer;
                        z-index: 10000;
                        transition: background-color 0.2s;
                    `;
                    
                    // Add hover effect
                    closeBtn.onmouseover = () => closeBtn.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';
                    closeBtn.onmouseout = () => closeBtn.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
                    
                    // Add click handler
                    closeBtn.onclick = () => {
                        const message = { key: 'CommandUserAuthentication', payload: { flow: 'login', state: 'cancelled' }};
                        window.chrome.webview.postMessage(message);
                    };
                    
                    // Append to body
                    document.body.appendChild(closeBtn);
                })();
            ";

            script = script.Replace("[[IconBack]]", IconBack);
            await webViewAdapter.ExecuteScriptAsync(script);
        }
    }
}
