using MobiFlight.WebView;
using System.IO;
using System.Windows.Forms;

namespace MobiFlight.OpenAttitude
{
    public partial class OpenAttitudeWindow : Form
    {
#if DEBUG
        private bool IsRunningInProduction = false;
#else 
        private bool IsRunningInProduction = true;
#endif
        private readonly string _frontendBaseUrl = "http://localhost:5174";
        private readonly string _frontendDistPath;
        private string WebsocketUrl { get; set; }

        public new bool DesignMode
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
            }
        }

        public OpenAttitudeWindow(string websocketUrl)
        {
            WebsocketUrl = websocketUrl;
            if (IsRunningInProduction)
            {
                _frontendBaseUrl = "https://oa.mobiflight.app";
                _frontendDistPath = Path.Combine(Application.StartupPath, "openattitude", "dist");
            }

            InitializeComponent();
            if (!DesignMode)
                InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await WebView.EnsureCoreWebView2Async(null);
            InitializeWebView(WebView, $"/?fgfs={WebsocketUrl}");
        }

        private void InitializeWebView(ThreadSafeWebView2 webView, string route = "/")
        {
            if (IsRunningInProduction)
            {
                // Production: serve all files through WebResourceRequested
                Log.Instance.log($"Initializing OpenAttitude WebView to serve from: {_frontendDistPath}", LogSeverity.Debug);

                // Add event handler
                var staticPageHandler = new StaticPageWebResourceRequestHandler(
                    _frontendBaseUrl,
                    _frontendDistPath
                );

                staticPageHandler.RegisterWithWebView(webView);

                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            }

            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            // Navigate to start the app
            webView.CoreWebView2.Navigate($"{_frontendBaseUrl}{route}");
        }
    }
}