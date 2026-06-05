using MobiFlight.WebView;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WpfScreenHelper;

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

            Load += OpenAttitudeWindow_Load;

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

        private void OpenAttitudeWindow_Load(object sender, System.EventArgs e)
        {
            // Set initial position and size
            var screenName = FindDefaultScreenByResolution(1920, 720);
            var screen = WpfScreenHelper.Screen.AllScreens.FirstOrDefault(s => s.DeviceName == screenName);
            if (screen != null)
            {
                this.Left = (int)screen.WpfBounds.Left;
                this.Top = (int)screen.WpfBounds.Top;
                this.Width = (int)screen.WpfBounds.Width;
                this.Height = (int)screen.WpfBounds.Height;
            }
        }

        private string FindDefaultScreenByResolution(int width, int height)
        {
            // Look for specific resolution
            var resolutionScreen = WpfScreenHelper.Screen.AllScreens.FirstOrDefault(s => s.WpfBounds.Width == width && s.WpfBounds.Height == height);
            if (resolutionScreen != null) return resolutionScreen.DeviceName;

            // Fallback to primary screen
            return WpfScreenHelper.Screen.PrimaryScreen.DeviceName;
        }
    }
}