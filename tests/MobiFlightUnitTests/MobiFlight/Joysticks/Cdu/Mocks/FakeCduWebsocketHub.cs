using System.Collections.Generic;
using MobiFlight.Joysticks.Cdu;

namespace MobiFlight.Joysticks.Cdu.Tests.Mocks
{
    /// <summary>Records what was registered/broadcast, without touching a real websocket server.</summary>
    internal class FakeCduWebsocketHub : ICduWebsocketHub
    {
        public List<(string Path, ICduDataConsumer Consumer)> Registered { get; } = new();
        public List<(string Path, ICduDataConsumer Consumer)> Unregistered { get; } = new();

        public void Register(string path, ICduDataConsumer consumer) => Registered.Add((path, consumer));
        public void Unregister(string path, ICduDataConsumer consumer) => Unregistered.Add((path, consumer));
        public void BroadcastData(string path, string json) { }
        public void BroadcastFont(string path, string json) { }
    }
}
