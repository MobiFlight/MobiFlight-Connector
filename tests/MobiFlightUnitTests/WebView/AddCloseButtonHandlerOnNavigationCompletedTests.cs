using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.WebView2.Core;
using MobiFlight.WebView;
using Moq;
using System;
using System.Threading.Tasks;

namespace MobiFlightUnitTests.WebView
{
    [TestClass()]
    public class AddCloseButtonHandlerOnNavigationCompletedTests
    {
        private AddCloseButtonHandlerOnNavigationCompleted _handler;
        private Mock<IWebView2Adapter> _mockWebView;

        [TestInitialize]
        public void SetUp()
        {
            _handler = new AddCloseButtonHandlerOnNavigationCompleted();
            _mockWebView = new Mock<IWebView2Adapter>();
        }

        [TestMethod()]
        public void RegisterWithWebView_SubscribesToNavigationCompletedEvent()
        {
            // Arrange
            bool eventHandlerAdded = false;
            _mockWebView.SetupAdd(w => w.NavigationCompleted += It.IsAny<EventHandler<CoreWebView2NavigationCompletedEventArgs>>())
                .Callback(() => eventHandlerAdded = true);

            // Act
            _handler.RegisterWithWebView(_mockWebView.Object);

            // Assert
            Assert.IsTrue(eventHandlerAdded, "Should subscribe to NavigationCompleted event");
        }

        [TestMethod()]
        public async Task NavigationCompleted_WithoutExclusionFilter_InjectsCloseButton()
        {
            // Arrange
            string executedScript = null;
            _mockWebView.Setup(w => w.Source).Returns("https://login.provider.com");
            _mockWebView.Setup(w => w.ExecuteScriptAsync(It.IsAny<string>()))
                .Callback<string>(script => executedScript = script)
                .ReturnsAsync("null");

            _handler.RegisterWithWebView(_mockWebView.Object);

            // Act - Trigger the NavigationCompleted event
            _mockWebView.Raise(w => w.NavigationCompleted += null, _mockWebView.Object, (CoreWebView2NavigationCompletedEventArgs)null);
            
            await Task.Delay(100); // Give async operation time to complete

            // Assert
            Assert.IsNotNull(executedScript, "Script should have been executed");
            Assert.Contains("closeBtn", executedScript, "Should inject close button");
            Assert.Contains("CommandUserAuthentication", executedScript, "Should handle authentication command");
            Assert.Contains("icon-tabler-chevron-left", executedScript, "Should include chevron-left icon");
        }

        [TestMethod()]
        public async Task NavigationCompleted_WithNonMatchingExclusionFilter_InjectsCloseButton()
        {
            // Arrange
            string executedScript = null;
            _mockWebView.Setup(w => w.Source).Returns("https://login.provider.com");
            _handler.AddExclusionFilter("mobiflight.app");

            _mockWebView.Setup(w => w.ExecuteScriptAsync(It.IsAny<string>()))
                .Callback<string>(script => executedScript = script)
                .ReturnsAsync("null");

            _handler.RegisterWithWebView(_mockWebView.Object);

            // Act - Trigger the NavigationCompleted event
            _mockWebView.Raise(w => w.NavigationCompleted += null, _mockWebView.Object, (CoreWebView2NavigationCompletedEventArgs)null);

            await Task.Delay(100); // Give async operation time to complete

            // Assert
            Assert.IsNotNull(executedScript, "Script should have been executed");
            Assert.Contains("closeBtn", executedScript, "Should inject close button");
            Assert.Contains("CommandUserAuthentication", executedScript, "Should handle authentication command");
            Assert.Contains("icon-tabler-chevron-left", executedScript, "Should include chevron-left icon");
        }

        [TestMethod()]
        public async Task NavigationCompleted_WithMatchingExclusionFilter_DoesNotInjectButton()
        {
            // Arrange
            _mockWebView.Setup(w => w.Source).Returns("https://mobiflight.app/config");
            _handler.AddExclusionFilter("mobiflight.app");

            int scriptExecutionCount = 0;
            _mockWebView.Setup(w => w.ExecuteScriptAsync(It.IsAny<string>()))
                .Callback(() => scriptExecutionCount++)
                .ReturnsAsync("null");

            _handler.RegisterWithWebView(_mockWebView.Object);

            // Act
            _mockWebView.Raise(w => w.NavigationCompleted += null, _mockWebView.Object, (CoreWebView2NavigationCompletedEventArgs)null);
            
            await Task.Delay(100);

            // Assert
            Assert.AreEqual(0, scriptExecutionCount, "Should not execute script when URL matches exclusion filter");
        }
    }
}