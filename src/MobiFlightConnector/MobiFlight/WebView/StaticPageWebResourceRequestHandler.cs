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

        internal void RegisterWithWebView(WebView2 webView)
        {
            webView.CoreWebView2.AddWebResourceRequestedFilter($"{BaseUrl}/*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        }

        public void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            try
            {
                var requestUri = e.Request.Uri;
                var result = GetFileResponse(requestUri);

                if (result.ShouldServe)
                {
                    var webView = sender as CoreWebView2;
                    var stream = new MemoryStream(result.Content);
                    e.Response = webView.Environment.CreateWebResourceResponse(stream, 200, "OK", $"Content-Type: {result.ContentType}");
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error handling web resource request: {ex.Message}", LogSeverity.Error);
            }
        }

        // Pure business logic - easily testable without WebView2
        internal FileResponse GetFileResponse(string requestUri)
        {
            var uri = new Uri(requestUri);
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
                return FileResponse.Blocked();
            }

            // Check if file exists
            if (File.Exists(fullPath))
            {
                return CreateFileResponse(fullPath);
            }
            else
            {
                // File doesn't exist - serve index.html for SPA routing
                var indexPath = Path.Combine(RootPath, "index.html");
                if (File.Exists(indexPath))
                {
                    Log.Instance.log($"SPA Route: {relativePath} -> serving index.html", LogSeverity.Debug);
                    return CreateFileResponse(indexPath, "text/html");
                }
                else
                {
                    Log.Instance.log($"Error: index.html not found at {indexPath}", LogSeverity.Error);
                    return FileResponse.NotFound();
                }
            }
        }

        private FileResponse CreateFileResponse(string filePath, string forceContentType = null)
        {
            var content = File.ReadAllBytes(filePath);
            var contentType = forceContentType ?? GetContentType(filePath);
            return new FileResponse(content, contentType);
        }

        internal string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (MimeTypes.TryGetValue(extension, out string contentType))
            {
                return contentType;
            }
            return "application/octet-stream";
        }
    }

    // Simple response object
    internal class FileResponse
    {
        public byte[] Content { get; }
        public string ContentType { get; }
        public bool ShouldServe { get; }

        public FileResponse(byte[] content, string contentType)
        {
            Content = content;
            ContentType = contentType;
            ShouldServe = true;
        }

        private FileResponse(bool shouldServe)
        {
            ShouldServe = shouldServe;
        }

        public static FileResponse Blocked() => new FileResponse(false);
        public static FileResponse NotFound() => new FileResponse(false);
    }
}