using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnectMSFS.Tests
{
    [TestClass()]
    public class WasmModuleUpdaterTests
    {
        private string testDirectory;
        private string testFile;
        private MethodInfo calculateMD5Method;

        [TestInitialize]
        public void Setup()
        {
            // Create a temporary directory for test files
            testDirectory = Path.Combine(Path.GetTempPath(), "WasmModuleUpdaterTests_" + Guid.NewGuid());
            Directory.CreateDirectory(testDirectory);
            
            // Create a test file with some content
            testFile = Path.Combine(testDirectory, "test.wasm");
            File.WriteAllText(testFile, "Test content for MD5 calculation");

            // Get reference to the private CalculateMD5 method once during setup
            calculateMD5Method = typeof(WasmModuleUpdater).GetMethod("CalculateMD5", 
                BindingFlags.NonPublic | BindingFlags.Static);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test directory
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        private string InvokeCalculateMD5(string filename)
        {
            return calculateMD5Method.Invoke(null, new object[] { filename }) as string;
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnValidHash_WhenFileExists()
        {
            // Arrange, Act
            var result = InvokeCalculateMD5(testFile);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(32, result.Length); // MD5 hash is 32 hex characters
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(result, "^[a-f0-9]{32}$"));
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnNull_WhenFileDoesNotExist()
        {
            // Arrange
            string nonExistentFile = Path.Combine(testDirectory, "nonexistent.wasm");

            // Act
            var result = InvokeCalculateMD5(nonExistentFile);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CalculateMD5_ShouldReturnNull_WhenFileIsLocked()
        {
            // Arrange, Act, Assert
            // Lock the file by opening it exclusively. We do not need a variable here;
            // the using statement ensures the FileStream remains open (and thus locked)
            // for the duration of this block and is disposed afterwards.
            using (new FileStream(testFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                var result = InvokeCalculateMD5(testFile);
                Assert.IsNull(result);
            }
        }

        [TestMethod()]
        public void WasmModulesAreDifferent_ShouldReturnTrue_WhenCommunityFolderIsNull()
        {
            // Arrange
            var updater = new WasmModuleUpdater();

            // Act
            var result = updater.WasmModulesAreDifferent(null);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void WasmModulesAreDifferent_ShouldReturnTrue_WhenCommunityFolderIsEmpty()
        {
            // Arrange
            var updater = new WasmModuleUpdater();

            // Act
            var result = updater.WasmModulesAreDifferent("");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void WasmModulesAreDifferent_ShouldReturnTrue_WhenWasmFileDoesNotExist()
        {
            // Arrange
            var updater = new WasmModuleUpdater();

            // Act
            var result = updater.WasmModulesAreDifferent(testDirectory);

            // Assert
            Assert.IsTrue(result);
        }

        #region UserCfg.opt parsing

        private string WriteUserCfg(params string[] lines)
        {
            var userCfg = Path.Combine(testDirectory, "UserCfg.opt");
            File.WriteAllLines(userCfg, lines);
            return userCfg;
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldReturnPathWithoutQuotes()
        {
            // Arrange
            var userCfg = WriteUserCfg(
                "{",
                "InstalledPackagesPath \"D:\\MSFS\"",
                "}");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.AreEqual(@"D:\MSFS", result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldIgnoreNextBootEntry()
        {
            // Arrange
            var userCfg = WriteUserCfg(
                "InstalledPackagesPathNextBoot \"E:\\Other\"",
                "InstalledPackagesPath \"D:\\MSFS\"");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.AreEqual(@"D:\MSFS", result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldHandleIndentedEntry()
        {
            // Arrange
            var userCfg = WriteUserCfg("    InstalledPackagesPath \"D:\\MSFS\"");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.AreEqual(@"D:\MSFS", result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldReturnNull_WhenEntryIsMissing()
        {
            // Arrange
            var userCfg = WriteUserCfg("{", "Something \"else\"", "}");

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        [DataRow("InstalledPackagesPath")]
        [DataRow("InstalledPackagesPath ")]
        [DataRow("InstalledPackagesPath \"\"")]
        public void ReadInstalledPackagesPath_ShouldReturnNull_WhenValueIsEmpty(string line)
        {
            // Arrange
            var userCfg = WriteUserCfg(line);

            // Act
            var result = WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void ReadInstalledPackagesPath_ShouldNotKeepFileLocked()
        {
            // Arrange
            var userCfg = WriteUserCfg("Something \"else\"");

            // Act
            WasmModuleUpdater.ReadInstalledPackagesPath(userCfg);

            // Assert
            using (new FileStream(userCfg, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // opening exclusively succeeds only if the reader was disposed
            }
        }

        #endregion

        #region DownloadSingleFile

        private const string DownloadPayload = "{\"presets\":[\"A32NX_EFIS_L_CHRONO_PUSHED\",\"A32NX_EFIS_R_CHRONO_PUSHED\"]}";

        // Serves DownloadPayload gzipped only if the client asks for gzip, and records the Accept-Encoding header.
        private static (HttpListener listener, Uri uri, Task<string> acceptEncoding) StartGzipServer()
        {
            var port = GetFreeTcpPort();
            var prefix = $"http://localhost:{port}/";
            var listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();

            var acceptEncoding = Task.Run(async () =>
            {
                var context = await listener.GetContextAsync();
                var requested = context.Request.Headers["Accept-Encoding"] ?? string.Empty;
                var body = Encoding.UTF8.GetBytes(DownloadPayload);

                if (requested.Contains("gzip"))
                {
                    using var compressed = new MemoryStream();
                    using (var gzip = new GZipStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
                    {
                        gzip.Write(body, 0, body.Length);
                    }
                    body = compressed.ToArray();
                    context.Response.AddHeader("Content-Encoding", "gzip");
                }

                context.Response.ContentLength64 = body.Length;
                await context.Response.OutputStream.WriteAsync(body);
                context.Response.Close();
                return requested;
            });

            return (listener, new Uri(prefix + "presets.json"), acceptEncoding);
        }

        private static int GetFreeTcpPort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }

        [TestMethod()]
        public async Task DownloadSingleFile_ShouldRequestGzipEncoding()
        {
            // Arrange
            var (listener, uri, acceptEncoding) = StartGzipServer();

            try
            {
                // Act
                var result = await WasmModuleUpdater.DownloadSingleFile(uri, "presets.json", testDirectory);

                // Assert
                Assert.IsTrue(result);
                Assert.Contains("gzip", await acceptEncoding);
            }
            finally
            {
                listener.Stop();
            }
        }

        [TestMethod()]
        public async Task DownloadSingleFile_ShouldWriteDecompressedContent_WhenResponseIsGzipped()
        {
            // Arrange
            var (listener, uri, acceptEncoding) = StartGzipServer();

            try
            {
                // Act
                var result = await WasmModuleUpdater.DownloadSingleFile(uri, "presets.json", testDirectory);
                await acceptEncoding;

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(DownloadPayload, File.ReadAllText(Path.Combine(testDirectory, "presets.json")));
            }
            finally
            {
                listener.Stop();
            }
        }

        #endregion
    }
}
