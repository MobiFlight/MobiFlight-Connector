using Microsoft.Web.WebView2.Core;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Publisher;
using System;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class FrontendPanel : UserControl
    {
        public new bool DesignMode
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
            }
        }

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
            }
#if DEBUG
            FrontendWebView.Source = new Uri("http://localhost:5173/index.html");
#else
            FrontendWebView.CoreWebView2.SetVirtualHostNameToFolderMapping("mobiflight",
            "frontend/dist", CoreWebView2HostResourceAccessKind.DenyCors);
            FrontendWebView.CoreWebView2.Navigate("http://mobiflight/index.html");
            FrontendWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            FrontendWebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
#endif
            FrontendWebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            FrontendWebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            FrontendWebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            FrontendWebView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

            MessageExchange.Instance.SetPublisher(new PostMessagePublisher(FrontendWebView));
        }

        private void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            //var settings = new GlobalSettings(Properties.Settings.Default);
            //MessageExchange.Instance.Publish(new Message<GlobalSettings>(settings));
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
        }
    }
}
