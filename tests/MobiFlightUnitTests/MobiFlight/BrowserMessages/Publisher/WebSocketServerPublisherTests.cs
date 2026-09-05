using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages.Publisher;
using MobiFlight.BrowserMessages.Transport;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.BrowserMessages.Publisher.Tests
{
    [TestClass]
    public class WebSocketServerPublisherTests
    {
        private MessageServer _server;
        private WebSocketServerPublisher _publisher;

        [TestInitialize]
        public void Setup()
        {
            _server = new MessageServer(0);
            _server.Start();
            _publisher = new WebSocketServerPublisher(_server);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Stop();
        }

        private class Test
        {
            public string Property1 { get; set; }
        }

        [TestMethod]
        public async Task Publish_WrapsPayloadInMessageEnvelope()
        {
            // This is exactly the bug the old (deleted) WebsocketPublisher had: publishing the
            // bare payload with no "key", which MessageExchange.PublishReceivedMessage routes on -
            // silently breaking delivery. Assert on the literal wire string, not just that
            // something was sent.
            using var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(_server.Url), CancellationToken.None);
            Assert.IsTrue(await WaitForConnectionAsync());

            _publisher.Publish(new Test { Property1 = "TestValue" });

            var buffer = new byte[1024];
            var result = await client.ReceiveAsync(buffer, CancellationToken.None);
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Assert.AreEqual("{\"key\":\"Test\",\"payload\":{\"Property1\":\"TestValue\"}}", json);
        }

        [TestMethod]
        public async Task OnMessageReceived_ForwardsIncomingClientText()
        {
            using var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(_server.Url), CancellationToken.None);

            var tcs = new TaskCompletionSource<string>();
            _publisher.OnMessageReceived(message => tcs.TrySetResult(message));

            var payload = Encoding.UTF8.GetBytes("{\"key\":\"Test\",\"payload\":{}}");
            await client.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(2000));
            Assert.AreSame(tcs.Task, completed, "OnMessageReceived callback should have fired.");
            Assert.AreEqual("{\"key\":\"Test\",\"payload\":{}}", tcs.Task.Result);
        }

        private async Task<bool> WaitForConnectionAsync(int timeoutMs = 2000)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                if (_server.ConnectionCount >= 1) return true;
                await Task.Delay(10).ConfigureAwait(false);
            }
            return _server.ConnectionCount >= 1;
        }
    }
}
