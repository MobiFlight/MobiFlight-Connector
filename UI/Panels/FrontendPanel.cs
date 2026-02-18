using Microsoft.Web.WebView2.Core;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Publisher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class FrontendPanel : UserControl
    {
        CompositePublisher compositePublisher = new CompositePublisher();
        private string _frontendBaseUrl = "http://localhost:5173";
        private string _frontendDistPath;
        private Dictionary<string, string> _mimeTypes;
        private bool IsRunningInProduction = true;
#if RELEASE
        private bool IsRunningInProduction = true;
#endif

        public new bool DesignMode
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
            }
        }

        double _desiredZoomFactor = 0.0;

        public FrontendPanel()
        {
            if (IsRunningInProduction)
            {
                _frontendBaseUrl = "https://mobiflight.app";
                _frontendDistPath = Path.Combine(Application.StartupPath, "frontend", "dist");
            }

            // Initialize MIME types for common web files
            _mimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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

            InitializeComponent();
            if (!DesignMode)
                InitializeAsync();
        }

        async void InitializeAsync()
        {
            if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
            {
                //Design time, no init due to DllNotFound Exception of VisualStudio
            }
            else
            {
                await FrontendWebView.EnsureCoreWebView2Async(null);
                await UserAuthenticationWebView.EnsureCoreWebView2Async(null);
            }

            InitializeWebView(FrontendWebView);
            InitializeWebView(UserAuthenticationWebView);

            // We only have to publish messages to the frontend
            // not the authentication webview
            compositePublisher.AddPublisher("frontend", new PostMessagePublisher(FrontendWebView));
            compositePublisher.AddPublisher("auth", new PostMessagePublisher(UserAuthenticationWebView));

            MessageExchange.Instance.SetPublisher(compositePublisher);
        }

        private void InitializeWebView(ThreadSafeWebView2 webView)
        {
            if (IsRunningInProduction)
            {
                // Production: serve all files through WebResourceRequested
                Log.Instance.log($"Initializing WebView to serve from: {_frontendDistPath}", LogSeverity.Debug);

                // Add filter to intercept ALL requests to localhost
                webView.CoreWebView2.AddWebResourceRequestedFilter(
                    $"{_frontendBaseUrl}/*",
                    CoreWebView2WebResourceContext.All);

                // Add event handler
                webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

                // Navigate to start the app
                webView.CoreWebView2.Navigate($"{_frontendBaseUrl}/index.html");

                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            }
            
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            // Navigate to start the app
            webView.CoreWebView2.Navigate($"{_frontendBaseUrl}/index.html");

            if (_desiredZoomFactor != 0.0)
            {
                webView.ZoomFactor = _desiredZoomFactor;
            }
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
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

                var filePath = Path.Combine(_frontendDistPath, relativePath);

                // Security: ensure the file is within the dist folder
                var fullPath = Path.GetFullPath(filePath);
                if (!fullPath.StartsWith(_frontendDistPath, StringComparison.OrdinalIgnoreCase))
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
                    var indexPath = Path.Combine(_frontendDistPath, "index.html");
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
                    if (!_mimeTypes.TryGetValue(extension, out contentType))
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

        public void SetZoomFactor(double zoomFactor)
        {
            if (zoomFactor < 0.1 || zoomFactor > 5.0)
            {
                zoomFactor = Math.Max(Math.Min(5.0, zoomFactor), 1.0);
            }

            if (FrontendWebView.CoreWebView2 != null)
            {
                FrontendWebView.ZoomFactor = zoomFactor;
            }
            else
            {
                _desiredZoomFactor = zoomFactor;
            }
        }

        public double GetZoomFactor()
        {
            if (FrontendWebView.CoreWebView2 != null)
            {
                return FrontendWebView.ZoomFactor;
            }
            return 0.0;
        }

        public void BeginAuthProcess(string url)
        {
            Log.Instance.log($"Starting authentication process, navigating to: {url}", LogSeverity.Debug);
            UserAuthenticationWebView.CoreWebView2.Navigate(url);
            UserAuthenticationWebView.Visible = true;
        }

        public void EndAuthProcess()
        {
            UserAuthenticationWebView.Visible = false;
            UserAuthenticationWebView.CoreWebView2.Navigate($"{_frontendBaseUrl}/auth");
        }

        public bool FrontendWebViewVisible
        {
            get => FrontendWebView.Visible;
            set => FrontendWebView.Visible = value;
        }
    }
}