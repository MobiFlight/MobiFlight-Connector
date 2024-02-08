using MobiFlight.BrowserMessages;
using MobiFlight.Config;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class UiPanel : UserControl
    {
        public new bool DesignMode
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");         
            }
        }
        public UiPanel()
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
                await webView.EnsureCoreWebView2Async(null);
            }
#if DEBUG
            webView.Source = new Uri("http://localhost:5173");
#else
            webView.Source = new Uri("http://localhost:5173");
#endif
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            RegisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            MessageExchange.Instance.Subscribe<Message<ConfigFile>>((config) =>
            {
                // do something with the config
                // convert config to JSON object
                var jsonEncodedConfig = JsonConvert.SerializeObject(config);
                webView.CoreWebView2.PostWebMessageAsString(jsonEncodedConfig);
            });

            MessageExchange.Instance.Subscribe<Message<StatusBarUpdate>>((message) =>
            {
                // do something with the config
                // convert config to JSON object
                var jsonEncodedMessage = JsonConvert.SerializeObject(message);
                webView.CoreWebView2.PostWebMessageAsString(jsonEncodedMessage);
            });
        }
    }
}
