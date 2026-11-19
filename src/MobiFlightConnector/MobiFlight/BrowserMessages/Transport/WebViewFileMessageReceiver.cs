using Microsoft.Web.WebView2.Core;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.WebView;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace MobiFlight.BrowserMessages.Transport
{
    /// <summary>
    /// Inbound bridge for messages the frontend posts together with a file through
    /// <c>chrome.webview.postMessageWithAdditionalObjects</c>, e.g. a project file dropped onto the window.
    /// The browser doesn't expose file system paths, so the path of the attached file is added to the
    /// message payload as <c>options.filePath</c>. Wire MessageReceived to MessageExchange.PublishReceivedMessage.
    /// </summary>
    public class WebViewFileMessageReceiver
    {
        public event Action<string> MessageReceived;

        public WebViewFileMessageReceiver(WebView2Adapter webView, string allowedOrigin)
        {
            webView.WebMessageReceived += (sender, args) =>
            {
                // Messages without a file are not meant for this bridge.
                var file = args.AdditionalObjects?.OfType<CoreWebView2File>().FirstOrDefault();
                if (file == null) return;

                if (!IsAllowedSource(args.Source, allowedOrigin))
                {
                    Log.Instance.log($"Ignoring file message from {args.Source}.", LogSeverity.Warn);
                    return;
                }

                try
                {
                    MessageReceived?.Invoke(AddFilePath(args.WebMessageAsJson, file.Path));
                }
                catch (JsonException ex)
                {
                    Log.Instance.log($"Unable to process file message: {ex.Message}", LogSeverity.Error);
                }
            };
        }

        /// <summary>
        /// Checks that a message comes from the app's own origin, so that a foreign page shown in the WebView can't open files.
        /// </summary>
        internal static bool IsAllowedSource(string source, string allowedOrigin)
        {
            if (!Uri.TryCreate(source, UriKind.Absolute, out var sourceUri)) return false;
            if (!Uri.TryCreate(allowedOrigin, UriKind.Absolute, out var allowedUri)) return false;

            return string.Equals(
                sourceUri.GetLeftPart(UriPartial.Authority),
                allowedUri.GetLeftPart(UriPartial.Authority),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the <see cref="CommandMainMenu"/> message with <paramref name="filePath"/> set as <c>payload.options.filePath</c>.
        /// </summary>
        /// <exception cref="JsonException">The message is not a valid <see cref="CommandMainMenu"/> message.</exception>
        internal static string AddFilePath(string messageJson, string filePath)
        {
            var message = JsonConvert.DeserializeObject<Message<CommandMainMenu>>(messageJson);

            if (message?.payload == null)
            {
                throw new JsonSerializationException("The message has no CommandMainMenu payload.");
            }

            message.payload.Options ??= new CommandMainMenuOptions();
            message.payload.Options.FilePath = filePath;

            return JsonConvert.SerializeObject(message, Formatting.None);
        }
    }
}
