using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.WebView;
using System;
using System.IO;

namespace MobiFlightUnitTests.WebView
{
    [TestClass()]
    public class StaticPageWebResourceRequestHandlerTests
    {
        private StaticPageWebResourceRequestHandler _handler;
        private string _testRootPath;
        private const string BaseUrl = "http://localhost:5173";

        [TestInitialize]
        public void SetUp()
        {
            _testRootPath = Path.Combine(Path.GetTempPath(), "Test_" + Guid.NewGuid());
            Directory.CreateDirectory(_testRootPath);
            _handler = new StaticPageWebResourceRequestHandler(BaseUrl, _testRootPath);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists(_testRootPath))
                Directory.Delete(_testRootPath, true);
        }

        [TestMethod()]
        public void GetFileResponse_EmptyPath_ReturnsIndexHtml()
        {
            // Arrange
            CreateFile("index.html", "<html>Home</html>");

            // Act
            var response = _handler.GetFileResponse($"{BaseUrl}/");

            // Assert
            Assert.IsTrue(response.ShouldServe);
            Assert.AreEqual("text/html", response.ContentType);
        }

        [TestMethod()]
        public void GetFileResponse_CssFile_ReturnsCorrectMimeType()
        {
            // Arrange
            CreateFile("style.css", "body{}");

            // Act
            var response = _handler.GetFileResponse($"{BaseUrl}/style.css");

            // Assert
            Assert.IsTrue(response.ShouldServe);
            Assert.AreEqual("text/css", response.ContentType);
        }

        [TestMethod()]
        public void GetFileResponse_PathTraversal_Blocks()
        {
            // Act
            var response = _handler.GetFileResponse($"{BaseUrl}/../secret.txt");

            // Assert
            Assert.IsFalse(response.ShouldServe);
        }

        [TestMethod()]
        public void GetFileResponse_SpaRoute_FallsBackToIndex()
        {
            // Arrange
            CreateFile("index.html", "<html>SPA</html>");

            // Act
            var response = _handler.GetFileResponse($"{BaseUrl}/user/profile");

            // Assert
            Assert.IsTrue(response.ShouldServe);
            Assert.AreEqual("text/html", response.ContentType);
        }

        [TestMethod()]
        public void GetContentType_UnknownExtension_ReturnsOctetStream()
        {
            // Assert
            Assert.AreEqual("application/octet-stream",
                _handler.GetContentType("file.xyz"));
        }

        private void CreateFile(string path, string content)
        {
            var full = Path.Combine(_testRootPath, path);
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllText(full, content);
        }
    }
}