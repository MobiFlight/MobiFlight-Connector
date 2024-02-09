using Microsoft.Web.WebView2.Core;
using MobiFlight.BrowserMessages;
using MobiFlight.Frontend;
using Newtonsoft.Json;
using System;
using System.Linq;
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
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            RegisterMessageHandlers();
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = e.WebMessageAsJson;
                var decodedMessage = JsonConvert.DeserializeObject<BrowserMessages.Message<ConfigItem>>(message);
                Log.Instance.log(decodedMessage.key, LogSeverity.Debug);

                if (decodedMessage.key == "config.edit")
                {
                    MessageExchange.Instance.Publish(new Message<IConfigItem>(decodedMessage.key, decodedMessage.payload));
                }
            }
            catch(Exception ex)
            {
                Log.Instance.log(ex.Message, LogSeverity.Error);
            }   
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

            MessageExchange.Instance.Subscribe<Message<IConfigItem>>((message) =>
            {
                var forwardedMessages = new string[] {
                    "config.update"
                };

                if (!forwardedMessages.ToArray().Contains(message.key))
                    return;

                // do something with the config
                // convert config to JSON object
                var jsonEncodedMessage = JsonConvert.SerializeObject(message);
                webView.CoreWebView2.PostWebMessageAsString(jsonEncodedMessage);
            });
        }
    }
}
