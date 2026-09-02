using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages.Transport;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.BrowserMessages.Transport.Tests
{
    [TestClass]
    public class MessageServerTests
    {
        private const string AllowedOrigin = "http://allowed-origin.test";
        private MessageServer _server;

        [TestInitialize]
        public void Setup()
        {
            // Port 0: bind to an ephemeral port so tests never collide with each other or with a
            // real running instance of the app.
            _server = new MessageServer(0, AllowedOrigin);
            _server.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Stop();
        }

        private async Task<bool> WaitForConnectionCountAsync(int expected, int timeoutMs = 2000)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                if (_server.ConnectionCount >= expected) return true;
                await Task.Delay(10).ConfigureAwait(false);
            }
            return _server.ConnectionCount >= expected;
        }

        [TestMethod]
        public void Start_BindsToEphemeralPort()
        {
            Assert.IsGreaterThan(0, _server.Port);
            Assert.IsTrue(_server.IsRunning);
            Assert.AreEqual($"ws://127.0.0.1:{_server.Port}/", _server.Url);
        }

        [TestMethod]
        public async Task Client_CompletesHandshake_AndConnects()
        {
            using var client = await ConnectDirectAsync();
            Assert.AreEqual(WebSocketState.Open, client.State);
        }

        [TestMethod]
        public async Task Broadcast_ReachesConnectedClient()
        {
            using var client = await ConnectDirectAsync();
            Assert.IsTrue(await WaitForConnectionCountAsync(1), "Server should have registered the connection.");

            _server.Broadcast("{\"key\":\"Test\"}");

            var buffer = new byte[1024];
            var result = await client.ReceiveAsync(buffer, CancellationToken.None);
            Assert.AreEqual("{\"key\":\"Test\"}", Encoding.UTF8.GetString(buffer, 0, result.Count));
        }

        [TestMethod]
        public async Task Broadcast_ReachesAllConnectedClients()
        {
            using var client1 = await ConnectDirectAsync();
            using var client2 = await ConnectDirectAsync();
            Assert.IsTrue(await WaitForConnectionCountAsync(2), "Server should have registered both connections.");

            _server.Broadcast("hello-all");

            var buffer1 = new byte[1024];
            var buffer2 = new byte[1024];
            var r1 = await client1.ReceiveAsync(buffer1, CancellationToken.None);
            var r2 = await client2.ReceiveAsync(buffer2, CancellationToken.None);

            Assert.AreEqual("hello-all", Encoding.UTF8.GetString(buffer1, 0, r1.Count));
            Assert.AreEqual("hello-all", Encoding.UTF8.GetString(buffer2, 0, r2.Count));
        }

        [TestMethod]
        public async Task Broadcast_SkipsDeadClient_StillReachesOthers()
        {
            var client1 = await ConnectDirectAsync();
            using var client2 = await ConnectDirectAsync();
            Assert.IsTrue(await WaitForConnectionCountAsync(2));

            // Kill client1 without a close handshake - simulates a stalled/crashed client.
            client1.Abort();
            client1.Dispose();

            _server.Broadcast("still-alive");

            var buffer2 = new byte[1024];
            var r2 = await client2.ReceiveAsync(buffer2, CancellationToken.None);
            Assert.AreEqual("still-alive", Encoding.UTF8.GetString(buffer2, 0, r2.Count));
        }

        [TestMethod]
        public async Task ClientMessage_RaisesMessageReceived()
        {
            using var client = await ConnectDirectAsync();
            var tcs = new TaskCompletionSource<string>();
            _server.MessageReceived += msg => tcs.TrySetResult(msg);

            var payload = Encoding.UTF8.GetBytes("{\"key\":\"Hello\"}");
            await client.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(2000));
            Assert.AreSame(tcs.Task, completed, "MessageReceived should have fired.");
            Assert.AreEqual("{\"key\":\"Hello\"}", tcs.Task.Result);
        }

        [TestMethod]
        public async Task LargeClientMessage_IsReassembledAcrossFrames()
        {
            using var client = await ConnectDirectAsync();
            var tcs = new TaskCompletionSource<string>();
            _server.MessageReceived += msg => tcs.TrySetResult(msg);

            // Larger than the connection's 8KB receive buffer, to exercise the
            // EndOfMessage reassembly loop rather than a single ReceiveAsync call.
            var largePayload = new string('x', 50_000);
            await client.SendAsync(Encoding.UTF8.GetBytes(largePayload), WebSocketMessageType.Text, true, CancellationToken.None);

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(3000));
            Assert.AreSame(tcs.Task, completed, "MessageReceived should have fired.");
            Assert.AreEqual(largePayload, tcs.Task.Result);
        }

        [TestMethod]
        public async Task Connect_WithDisallowedOrigin_IsRejected()
        {
            using var client = new ClientWebSocket();
            client.Options.SetRequestHeader("Origin", "http://not-allowed.test");

            await Assert.ThrowsExactlyAsync<WebSocketException>(
                () => client.ConnectAsync(new Uri(_server.Url), CancellationToken.None));
        }

        [TestMethod]
        public async Task Connect_WithNoOriginHeader_IsAllowed()
        {
            // No Origin header at all - how a non-browser client (e.g. a future mobile companion
            // app) connects.
            using var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(_server.Url), CancellationToken.None);
            Assert.AreEqual(WebSocketState.Open, client.State);
        }

        [TestMethod]
        public void Stop_ClosesCleanly()
        {
            _server.Stop();
            Assert.IsFalse(_server.IsRunning);
        }

        private async Task<ClientWebSocket> ConnectDirectAsync()
        {
            var client = new ClientWebSocket();
            client.Options.SetRequestHeader("Origin", AllowedOrigin);
            await client.ConnectAsync(new Uri(_server.Url), CancellationToken.None).ConfigureAwait(false);
            return client;
        }
    }
}
