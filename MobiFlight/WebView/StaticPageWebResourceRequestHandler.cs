using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.IO;

namespace MobiFlight.WebView
{
    internal class StaticPageWebResourceRequestHandler
    {

        private Dictionary<string, string> MimeTypes;
        private string BaseUrl;
        private string RootPath;
        public StaticPageWebResourceRequestHandler(string baseUrl, string rootPath)
        {
            BaseUrl = baseUrl;
            RootPath = rootPath;

            // Initialize MIME types for common web files
            MimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".html", "text/html" },
                { ".css", "text/css" },
                { ".js", "application/javascript" },
                { ".json", "application/json" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".svg", "image/svg+xml" },
                { ".ico", "image/x-icon" },
                { ".woff", "font/woff" },
                { ".woff2", "font/woff2" },
                { ".ttf", "font/ttf" },
                { ".eot", "application/vnd.ms-fontobject" },
                { ".map", "application/json" },
                { ".txt", "text/plain" }
            };
        }
        public void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            try
            {
                var uri = new Uri(e.Request.Uri);
                var relativePath = uri.AbsolutePath.TrimStart('/');

                // Default to index.html if path is empty
                if (string.IsNullOrEmpty(relativePath))
                {
                    relativePath = "index.html";
                }

                var filePath = Path.Combine(RootPath, relativePath);

                // Security: ensure the file is within the dist folder
                var fullPath = Path.GetFullPath(filePath);
                if (!fullPath.StartsWith(RootPath, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Instance.log($"Security: Blocked path traversal attempt - {fullPath}", LogSeverity.Warn);
                    return;
                }

                // Check if file exists
                if (File.Exists(fullPath))
                {
                    // Serve the actual file
                    ServeFile(e, sender as CoreWebView2, fullPath);
                }
                else
                {
                    // File doesn't exist - serve index.html for SPA routing
                    var indexPath = Path.Combine(RootPath, "index.html");
                    if (File.Exists(indexPath))
                    {
                        Log.Instance.log($"SPA Route: {relativePath} -> serving index.html", LogSeverity.Debug);
                        ServeFile(e, sender as CoreWebView2, indexPath, "text/html");
                    }
                    else
                    {
                        Log.Instance.log($"Error: index.html not found at {indexPath}", LogSeverity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error handling web resource request: {ex.Message}", LogSeverity.Error);
            }
        }

        internal void RegisterWithWebView(WebView2 webView)
        {
            // Add filter to intercept ALL requests to localhost
            webView.CoreWebView2.AddWebResourceRequestedFilter(
                $"{BaseUrl}/*",
                CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        }

        private void ServeFile(CoreWebView2WebResourceRequestedEventArgs e, CoreWebView2 webView, string filePath, string forceContentType = null)
        {
            try
            {
                var content = File.ReadAllBytes(filePath);
                var stream = new MemoryStream(content);

                // Determine content type
                var contentType = forceContentType;
                if (contentType == null)
                {
                    var extension = Path.GetExtension(filePath);
                    if (!MimeTypes.TryGetValue(extension, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }
                }

                e.Response = webView.Environment.CreateWebResourceResponse(
                    stream,
                    200,
                    "OK",
                    $"Content-Type: {contentType}");
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error serving file {filePath}: {ex.Message}", LogSeverity.Error);
            }
        }
    }
}
