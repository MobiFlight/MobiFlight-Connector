using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Transport;
using MobiFlight.WebView;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class FrontendPanel : UserControl
    {
        private string _frontendBaseUrl = "http://localhost:5173";
        private string _frontendDistPath;
        private string _presetPath;
#if DEBUG
        private bool IsRunningInProduction = false;
#else 
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

            _presetPath = Path.Combine(Application.StartupPath);

            InitializeComponent();
            if (!DesignMode)
                InitializeAsync();
        }

        async void InitializeAsync()
        {
            await FrontendWebView.EnsureCoreWebView2Async(null);
            await UserAuthenticationWebView.EnsureCoreWebView2Async(null);

            // MainForm's constructor already set the WebSocket publisher on MessageExchange.
            await InitializeWebView(FrontendWebView, "/start", injectWsUrl: true);

            // Auth WebView stays on postMessage - see docs/architecture/frontend-backend-messaging.md.
            await InitializeWebView(UserAuthenticationWebView, "/", injectWsUrl: false);
            new WebViewMessageReceiver(UserAuthenticationWebView).MessageReceived
                += MessageExchange.Instance.PublishReceivedMessage;
        }

        private async Task InitializeWebView(ThreadSafeWebView2 webView, string route, bool injectWsUrl)
        {
            if (IsRunningInProduction)
            {
                // Production: serve all files through WebResourceRequested
                Log.Instance.log($"Initializing WebView to serve from: {_frontendDistPath}", LogSeverity.Debug);

                // Add event handler
                var staticPageHandler = new StaticPageWebResourceRequestHandler(
                    "frontend",
                    _frontendBaseUrl,
                    _frontendDistPath,
                    new string[] { _frontendBaseUrl + "/presets" }
                );

                staticPageHandler.RegisterWithWebView(webView);

                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            }

            var staticPresetsHandler = new StaticPageWebResourceRequestHandler(
                "presets",
                _frontendBaseUrl + "/presets",
                _presetPath
            )
            {
                IndexFallback = false
            };
            staticPresetsHandler.RegisterWithWebView(webView);

            var addButtonHandler = new AddCloseButtonHandlerOnNavigationCompleted();
            addButtonHandler.AddExclusionFilter(_frontendBaseUrl);
            addButtonHandler.RegisterWithWebView(webView);

            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;

            // Inject the actual bound URL before any page script runs.
            if (injectWsUrl && MessageServer.Current != null)
            {
                await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                    $"window.__MOBIFLIGHT__ = {{ wsUrl: '{MessageServer.Current.Url}' }};");
            }

            // Navigate to start the app
            webView.CoreWebView2.Navigate($"{_frontendBaseUrl}{route}");

            if (_desiredZoomFactor != 0.0)
            {
                webView.ZoomFactor = _desiredZoomFactor;
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

        /// <summary>
        /// Called at the start of the authentication process, with the URL to navigate to for authentication
        /// The authentication process happens in a separate Webview
        /// The state in the actual frontend WebView is preserved
        /// Once authentication flow is completed, EndAuthProcess should be called to hide the authentication WebView
        /// </summary>
        /// <param name="url"></param>
        public void BeginAuthProcess(string url)
        {
            Log.Instance.log($"Starting authentication process, navigating to: {url}", LogSeverity.Debug);
            UserAuthenticationWebView.CoreWebView2.Navigate(url);
            UserAuthenticationWebView.Visible = true;
        }

        /// <summary>
        /// Called at the end of the authentication process
        /// The WebView is hidden and we navigate back to a default idle page
        /// </summary>
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

        /// <summary>
        /// Indicates whether the authentication process is currently in progress, 
        /// based on the visibility of the authentication WebView.
        /// </summary>
        public bool AuthProcessInProgress
        {
            get => UserAuthenticationWebView.Visible;
        }
    }
}