using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Cdu
{
    /// <summary>
    /// Reference-counts <see cref="ICduDataConsumer"/> registrations per websocket path, so
    /// e.g. unplugging a Winwing CDU doesn't tear the endpoint out from under a MOZA panel
    /// sharing the same seat path. The websocket route (and the server itself) is added on
    /// the first consumer for a path and removed only when the last one leaves.
    /// </summary>
    internal class CduWebsocketHub : ICduWebsocketHub
    {
        private readonly WebSocketServer Server;
        private readonly object Lock = new();
        private readonly Dictionary<string, List<ICduDataConsumer>> ConsumersByPath = [];

        public CduWebsocketHub(WebSocketServer server)
        {
            Server = server;
        }

        public void Register(string path, ICduDataConsumer consumer)
        {
            lock (Lock)
            {
                if (!ConsumersByPath.TryGetValue(path, out var consumers))
                {
                    consumers = [];
                    ConsumersByPath[path] = consumers;
                    Server.AddWebSocketService<CduWebsocketBehavior>(path, s =>
                    {
                        s.Hub = this;
                        s.Path = path;
                    });
                }
                consumers.Add(consumer);

                if (!Server.IsListening)
                {
                    Server.Start();
                }
            }
        }

        public void Unregister(string path, ICduDataConsumer consumer)
        {
            lock (Lock)
            {
                if (!ConsumersByPath.TryGetValue(path, out var consumers)) return;

                consumers.Remove(consumer);
                if (consumers.Count == 0)
                {
                    ConsumersByPath.Remove(path);
                    Server.RemoveWebSocketService(path);
                }
            }
        }

        public void BroadcastData(string path, string json) => Broadcast(path, c => c.OnCduData(json));
        public void BroadcastFont(string path, string json) => Broadcast(path, c => c.OnCduFont(json));

        private void Broadcast(string path, Action<ICduDataConsumer> deliver)
        {
            List<ICduDataConsumer> snapshot;
            lock (Lock)
            {
                if (!ConsumersByPath.TryGetValue(path, out var consumers)) return;
                snapshot = [.. consumers];
            }

            foreach (var consumer in snapshot)
            {
                try
                {
                    deliver(consumer);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"CDU consumer on '{path}' failed to handle incoming data: {ex.Message}", LogSeverity.Error);
                }
            }
        }
    }
}
