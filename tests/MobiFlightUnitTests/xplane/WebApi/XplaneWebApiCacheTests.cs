using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.xplane.WebApi;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.xplane.WebApi.Tests
{
    [TestClass]
    public class XplaneWebApiCacheTests
    {
        private const string DatarefsJson = "{\"data\":[" +
            "{\"id\":1,\"name\":\"sim/time/total_running_time_sec\",\"value_type\":\"float\"}," +
            "{\"id\":2,\"name\":\"sim/foo/bar\",\"value_type\":\"float\"}]}";

        private const string CommandsJson = "{\"data\":[" +
            "{\"id\":5,\"name\":\"sim/flight_controls/flaps_down\",\"description\":\"Flaps down\"}]}";

        private static XplaneWebApiCache CreateCache(FakeHttpMessageHandler handler, TimeSpan? probeTimeout = null)
        {
            return new XplaneWebApiCache("127.0.0.1", 18086, probeTimeout ?? TimeSpan.FromMilliseconds(300), handler);
        }

        [TestMethod]
        public void Connect_ReturnsFalse_WhenDatarefsRequestThrowsConnectionRefused()
        {
            var handler = new FakeHttpMessageHandler((request, token) =>
                throw new HttpRequestException("Connection refused"));

            var cache = CreateCache(handler);

            var result = cache.Connect();

            Assert.IsFalse(result);
            Assert.IsFalse(cache.IsConnected());
        }

        [TestMethod]
        public void Connect_ReturnsFalse_WhenDatarefsResponseIsNonSuccess()
        {
            var handler = new FakeHttpMessageHandler((request, token) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

            var cache = CreateCache(handler);

            var result = cache.Connect();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Connect_ReturnsFalse_OnSimulatedTimeout()
        {
            var handler = new FakeHttpMessageHandler(async (request, token) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                return JsonResponse(DatarefsJson);
            });

            var cache = CreateCache(handler, TimeSpan.FromMilliseconds(100));

            var result = cache.Connect();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Connect_PopulatesDatarefAndCommandIdMaps_EvenThoughOverallConnectFailsWithoutALiveWebSocketServer()
        {
            var handler = new FakeHttpMessageHandler((request, token) =>
            {
                if (request.RequestUri.AbsolutePath == XplaneWebApiConstants.DatarefsPath)
                    return Task.FromResult(JsonResponse(DatarefsJson));

                if (request.RequestUri.AbsolutePath == XplaneWebApiConstants.CommandsPath)
                    return Task.FromResult(JsonResponse(CommandsJson));

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            });

            var cache = CreateCache(handler);

            var result = cache.Connect();

            Assert.IsFalse(result);
            Assert.IsTrue(cache.TryResolveDatarefId("sim/foo/bar", out var datarefId));
            Assert.AreEqual(2, datarefId);
            Assert.IsTrue(cache.TryResolveCommandId("sim/flight_controls/flaps_down", out var commandId));
            Assert.AreEqual(5, commandId);
        }

        [TestMethod]
        public void Connect_DoesNotThrow_WhenCommandsDiscoveryFails()
        {
            var handler = new FakeHttpMessageHandler((request, token) =>
            {
                if (request.RequestUri.AbsolutePath == XplaneWebApiConstants.DatarefsPath)
                    return Task.FromResult(JsonResponse(DatarefsJson));

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            });

            var cache = CreateCache(handler);

            var result = cache.Connect();

            Assert.IsFalse(result);
            Assert.IsTrue(cache.TryResolveDatarefId("sim/foo/bar", out _));
            Assert.IsFalse(cache.TryResolveCommandId("sim/flight_controls/flaps_down", out _));
        }

        [TestMethod]
        public void Connect_PaginatesDatarefDiscovery_AcrossMultiplePages()
        {
            var pageSize = XplaneWebApiCache.DiscoveryPageSize;

            var handler = new FakeHttpMessageHandler((request, token) =>
            {
                if (request.RequestUri.AbsolutePath != XplaneWebApiConstants.DatarefsPath)
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));

                var start = GetQueryInt(request.RequestUri, "start", 0);

                var count = start == 0 ? pageSize : 1;
                var entries = Enumerable.Range(start, count)
                    .Select(i => $"{{\"id\":{i + 1},\"name\":\"sim/test/dataref{i}\",\"value_type\":\"float\"}}");

                return Task.FromResult(JsonResponse($"{{\"data\":[{string.Join(",", entries)}]}}"));
            });

            var cache = CreateCache(handler);

            cache.Connect();

            Assert.IsTrue(cache.TryResolveDatarefId("sim/test/dataref0", out var firstId));
            Assert.AreEqual(1, firstId);
            Assert.IsTrue(cache.TryResolveDatarefId($"sim/test/dataref{pageSize}", out var secondPageId));
            Assert.AreEqual(pageSize + 1, secondPageId);
        }

        private static int GetQueryInt(Uri uri, string name, int fallback)
        {
            foreach (var pair in uri.Query.TrimStart('?').Split('&'))
            {
                var parts = pair.Split('=');
                if (parts.Length == 2 && parts[0] == name && int.TryParse(parts[1], out var value))
                    return value;
            }

            return fallback;
        }

        private static HttpResponseMessage JsonResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _handler(request, cancellationToken);
            }
        }
    }
}
