using Microsoft.Web.WebView2.Core;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Publisher;
using System;
using System.Management.Instrumentation;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class FrontendPanel : UserControl
    {
        CompositePublisher compositePublisher = new CompositePublisher();
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
            //compositePublisher.PausePublisher("auth");
            MessageExchange.Instance.SetPublisher(compositePublisher);
        }

        private void InitializeWebView(ThreadSafeWebView2 webView)
        {
#if DEBUG
            webView.Source = new Uri("http://localhost:5173/index.html");
#else
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("localhost",
            "frontend/dist", CoreWebView2HostResourceAccessKind.DenyCors);
            webView.CoreWebView2.Navigate("http://localhost/index.html");
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
#endif
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

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

        private void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {

            //var settings = new GlobalSettings(Properties.Settings.Default);
            //MessageExchange.Instance.Publish(new Message<GlobalSettings>(settings));
        }

        public void BeginAuthProcess(string url)
        {
            // Get ready to receive messages from the authentication webview
            // and forward them to the "frontend" webview (by the correct subscriber)
           // compositePublisher.ResumePublisher("auth");
            UserAuthenticationWebView.CoreWebView2.Navigate(url);
            UserAuthenticationWebView.Visible = true;
        }

        public void EndAuthProcess()
        {
            // Unregister the authentication webview as a publisher
            // to stop forwarding messages to the frontend
            UserAuthenticationWebView.Visible = false;
#if DEBUG
            UserAuthenticationWebView.CoreWebView2.Navigate("http://localhost:5173/auth");
#else
            UserAuthenticationWebView.CoreWebView2.Navigate("http://localhost/auth");
#endif
            // compositePublisher.PausePublisher("auth");
        }

        public bool FrontendWebViewVisible
        {
            get => FrontendWebView.Visible;
            set => FrontendWebView.Visible = value;
        }
    }
}
