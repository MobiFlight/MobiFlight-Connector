using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MobiFlight.WebView
{
    /// <summary>
    /// Adapter interface for WebView2 to enable testability
    /// </summary>
    public interface IWebView2Adapter
    {
        /// <summary>
        /// Gets the current source URL
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Execute JavaScript in the WebView
        /// </summary>
        Task<string> ExecuteScriptAsync(string script);

        /// <summary>
        /// Event raised when navigation completes
        /// </summary>
        event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;
    }
}