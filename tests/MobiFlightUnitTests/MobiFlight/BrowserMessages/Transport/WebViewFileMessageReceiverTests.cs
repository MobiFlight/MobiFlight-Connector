using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.BrowserMessages.Transport;
using Newtonsoft.Json;

namespace MobiFlightUnitTests.MobiFlight.BrowserMessages.Transport
{
    [TestClass]
    public class WebViewFileMessageReceiverTests
    {
        private const string FilePath = @"C:\Users\Pilot\Documents\My Project.mfproj";

        #region AddFilePath

        [TestMethod]
        public void AddFilePath_ShouldAddOptionsWithFilePath_WhenPayloadHasNoOptions()
        {
            // Arrange
            var messageJson = "{\"key\":\"CommandMainMenu\",\"payload\":{\"action\":\"file.open\"}}";

            // Act
            var result = WebViewFileMessageReceiver.AddFilePath(messageJson, FilePath);

            // Assert
            var message = JsonConvert.DeserializeObject<Message<CommandMainMenu>>(result);
            Assert.AreEqual("CommandMainMenu", message.key);
            Assert.AreEqual(CommandMainMenuAction.file_open, message.payload.Action);
            Assert.AreEqual(FilePath, message.payload.Options.FilePath);
        }

        [TestMethod]
        public void AddFilePath_ShouldReplaceFilePath_WhenPayloadHasOptions()
        {
            // Arrange
            var messageJson = "{\"key\":\"CommandMainMenu\",\"payload\":{\"action\":\"file.open\",\"options\":{\"filePath\":\"ignored.mfproj\"}}}";

            // Act
            var result = WebViewFileMessageReceiver.AddFilePath(messageJson, FilePath);

            // Assert
            var message = JsonConvert.DeserializeObject<Message<CommandMainMenu>>(result);
            Assert.AreEqual(FilePath, message.payload.Options.FilePath);
        }

        [TestMethod]
        public void AddFilePath_ShouldThrowJsonException_WhenMessageHasNoPayload()
        {
            // Arrange
            var messageJson = "{\"key\":\"CommandMainMenu\"}";

            // Act & Assert
            Assert.Throws<JsonException>(() => WebViewFileMessageReceiver.AddFilePath(messageJson, FilePath));
        }

        [TestMethod]
        public void AddFilePath_ShouldThrowJsonException_WhenMessageIsNoObject()
        {
            // Arrange
            var messageJson = "\"file.open\"";

            // Act & Assert
            Assert.Throws<JsonException>(() => WebViewFileMessageReceiver.AddFilePath(messageJson, FilePath));
        }

        #endregion

        #region IsAllowedSource

        [TestMethod]
        [DataRow("https://mobiflight.app/config", "https://mobiflight.app")]
        [DataRow("https://mobiflight.app/home?tab=projects", "https://mobiflight.app")]
        [DataRow("http://localhost:5173/config", "http://localhost:5173")]
        public void IsAllowedSource_ShouldReturnTrue_WhenSourceHasAllowedOrigin(string source, string allowedOrigin)
        {
            // Act
            var result = WebViewFileMessageReceiver.IsAllowedSource(source, allowedOrigin);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("https://mobiflight.app.example.com/config", "https://mobiflight.app")]
        [DataRow("https://login.example.com/authorize", "https://mobiflight.app")]
        [DataRow("http://mobiflight.app/config", "https://mobiflight.app")]
        [DataRow("http://localhost:5174/config", "http://localhost:5173")]
        [DataRow("file:///C:/Users/Pilot/Documents/My%20Project.mfproj", "https://mobiflight.app")]
        [DataRow("", "https://mobiflight.app")]
        [DataRow(null, "https://mobiflight.app")]
        public void IsAllowedSource_ShouldReturnFalse_WhenSourceHasOtherOrigin(string source, string allowedOrigin)
        {
            // Act
            var result = WebViewFileMessageReceiver.IsAllowedSource(source, allowedOrigin);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion
    }
}
