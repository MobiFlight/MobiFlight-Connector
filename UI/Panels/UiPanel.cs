using Microsoft.Web.WebView2.Core;
using MobiFlight.BrowserMessages;
using MobiFlight.Frontend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("mobiflight",
            "frontend/dist", CoreWebView2HostResourceAccessKind.DenyCors);
            webView.CoreWebView2.Navigate("https://mobiflight/index.html");
#endif
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            // webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            //webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            webView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            RegisterMessageHandlers();
        }

        private void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            var settings = new GlobalSettings(Properties.Settings.Default);
            MessageExchange.Instance.Publish(new Message<GlobalSettings>(settings));
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = e.WebMessageAsJson;
                var decodedMessage = JsonConvert.DeserializeObject<BrowserMessages.Message<object>>(message);
                Log.Instance.log(decodedMessage.key, LogSeverity.Debug);

                if (decodedMessage.key == "config.edit")
                {
                    var ConfigEditMessage = JsonConvert.DeserializeObject<BrowserMessages.Message<ConfigItem>>(message);
                    MessageExchange.Instance.Publish(new Message<ConfigItem>(ConfigEditMessage.key, ConfigEditMessage.payload));
                }

                if (decodedMessage.key == "ExecutionUpdate")
                {
                    var ExecutionUpdateRequest = JsonConvert.DeserializeObject<BrowserMessages.Message<ExecutionState>>(message);
                    MessageExchange.Instance.Publish(new FrontendRequest<ExecutionUpdate>()
                        {
                            Request = new ExecutionUpdate
                            {
                                State = ExecutionUpdateRequest.payload
                            }
                        });
                }

                if (decodedMessage.key == "GlobalSettingsUpdate")
                {
                    var GlobalSettingsUpdateRequest = JsonConvert.DeserializeObject<BrowserMessages.Message<GlobalSettings>>(message);
                    MessageExchange.Instance.Publish(new FrontendRequest<GlobalSettings>()
                    {
                        Request = GlobalSettingsUpdateRequest.payload
                    });
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

            MessageExchange.Instance.Subscribe<Message<GlobalSettings>>((message) =>
            {
                // do something with the config
                // convert config to JSON object
                var jsonEncodedMessage = JsonConvert.SerializeObject(message);
                webView.CoreWebView2.PostWebMessageAsString(jsonEncodedMessage);
            });

            MessageExchange.Instance.Subscribe<Message<LogMessage>>((message) =>
            {
                // Define a lambda expression that encapsulates the common logic
                System.Action processMessageAction = () =>
                {
                    // Convert config to JSON object
                    var jsonEncodedMessage = JsonConvert.SerializeObject(message);
                    webView.CoreWebView2.PostWebMessageAsString(jsonEncodedMessage);
                };

                // Check if invocation on the UI thread is required
                if (webView.InvokeRequired)
                {
                    // If yes, use Invoke to run the action on the UI thread
                    webView.Invoke(processMessageAction);
                    return;
                }
                // If not, run the action directly
                processMessageAction();
            });

            MessageExchange.Instance.Subscribe<Message<ExecutionUpdate>>((message) =>
            {
                // do something with the config
                // convert config to JSON object
                var jsonEncodedMessage = JsonConvert.SerializeObject(message);
                webView.CoreWebView2.PostWebMessageAsString(jsonEncodedMessage);
            });

            MessageExchange.Instance.Subscribe<Message<ConfigValueUpdate>>((message) => {
                // do something with the config
                // convert config to JSON object
                var jsonEncodedMessage = JsonConvert.SerializeObject(message);
                webView.CoreWebView2.PostWebMessageAsString(jsonEncodedMessage);
            });

            MessageExchange.Instance.Subscribe<Message<DeviceList>>((message) =>
            {
                // Define a lambda expression that encapsulates the common logic
                System.Action processMessageAction = () =>
                {
                    // Convert config to JSON object
                    var jsonEncodedMessage = JsonConvert.SerializeObject(message);
                    webView.CoreWebView2.PostWebMessageAsString(jsonEncodedMessage);
                };

                // Check if invocation on the UI thread is required
                if (webView.InvokeRequired)
                {
                    // If yes, use Invoke to run the action on the UI thread
                    webView.Invoke(processMessageAction);
                    return;
                }
                // If not, run the action directly
                processMessageAction();
            });
        }
    }
}
