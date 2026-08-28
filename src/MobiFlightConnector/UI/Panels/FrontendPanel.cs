using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Publisher;
using MobiFlight.WebView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class FrontendPanel : UserControl
    {
        private readonly CompositePublisher _compositePublisher = new();
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

        private double _desiredZoomFactor;
        private Task _initializationTask;
        private bool _isStopping;
        private readonly List<(ThreadSafeWebView2 WebView, StaticPageWebResourceRequestHandler Handler)> _webResourceHandlers = new();
        private readonly List<AddCloseButtonHandlerOnNavigationCompleted> _navigationHandlers = new();
        private readonly List<PostMessagePublisher> _messagePublishers = new();

        public FrontendPanel()
        {
            if (IsRunningInProduction)
            {
                _frontendBaseUrl = "https://mobiflight.app";
                _frontendDistPath = Path.Combine(Application.StartupPath, "frontend", "dist");
            }

            _presetPath = Path.Combine(Application.StartupPath);

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode && _initializationTask == null)
            {
                _initializationTask = InitializeAsync();
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                if (_isStopping || IsDisposed || Disposing)
                {
                    return;
                }

                await FrontendWebView.EnsureCoreWebView2Async(null);

                if (_isStopping || IsDisposed || Disposing)
                {
                    return;
                }

                await UserAuthenticationWebView.EnsureCoreWebView2Async(null);

                if (_isStopping || IsDisposed || Disposing)
                {
                    return;
                }

                InitializeWebView(FrontendWebView, "/start");
                InitializeWebView(UserAuthenticationWebView);

                var frontendPublisher = new PostMessagePublisher(FrontendWebView);
                var authPublisher = new PostMessagePublisher(UserAuthenticationWebView);
                _messagePublishers.Add(frontendPublisher);
                _messagePublishers.Add(authPublisher);
                _compositePublisher.AddPublisher("frontend", frontendPublisher);
                _compositePublisher.AddPublisher("auth", authPublisher);

                MessageExchange.Instance.SetPublisher(_compositePublisher);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                Log.Instance.log(exception, "Unable to initialize frontend WebView2.", LogSeverity.Error);
            }
        }
        
        public void StopPublishing()
        {
            if (_isStopping) return;
            _isStopping = true;

            _compositePublisher.RemovePublisher("frontend");
            _compositePublisher.RemovePublisher("auth");

            if (ReferenceEquals(MessageExchange.Instance.GetPublisher(), _compositePublisher))
            {
                MessageExchange.Instance.SetPublisher(null);
            }

            DisposeWebViewCallbacks();
        }

        private void InitializeWebView(ThreadSafeWebView2 webView, string route = "/")
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

                RegisterWebResourceHandler(webView, staticPageHandler);

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
            RegisterWebResourceHandler(webView, staticPresetsHandler);

            var addButtonHandler = new AddCloseButtonHandlerOnNavigationCompleted();
            addButtonHandler.AddExclusionFilter(_frontendBaseUrl);
            RegisterNavigationHandler(webView, addButtonHandler);

            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            // Navigate to start the app
            webView.CoreWebView2.Navigate($"{_frontendBaseUrl}{route}");

            if (_desiredZoomFactor != 0.0)
            {
                webView.ZoomFactor = _desiredZoomFactor;
            }
        }

        private void RegisterWebResourceHandler(
            ThreadSafeWebView2 webView,
            StaticPageWebResourceRequestHandler handler)
        {
            handler.RegisterWithWebView(webView);
            _webResourceHandlers.Add((webView, handler));
        }

        private void RegisterNavigationHandler(
            ThreadSafeWebView2 webView,
            AddCloseButtonHandlerOnNavigationCompleted handler)
        {
            handler.RegisterWithWebView(webView);
            _navigationHandlers.Add(handler);
        }

        private void DisposeWebViewCallbacks()
        {
            foreach (var publisher in _messagePublishers)
            {
                publisher.Dispose();
            }
            _messagePublishers.Clear();

            foreach (var navigationHandler in _navigationHandlers)
            {
                navigationHandler.Unregister();
            }
            _navigationHandlers.Clear();

            foreach (var (webView, handler) in _webResourceHandlers)
            {
                handler.UnregisterFromWebView(webView);
            }
            _webResourceHandlers.Clear();
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

            if (UserAuthenticationWebView.CoreWebView2 == null || IsDisposed || Disposing)
            {
                Log.Instance.log("Authentication navigation was requested before WebView2 initialization completed.", LogSeverity.Warn);
                return;
            }

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

            if (UserAuthenticationWebView.CoreWebView2 == null || IsDisposed || Disposing)
            {
                return;
            }

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
