namespace MobiFlight.Joysticks.Cdu
{
    /// <summary>
    /// Fans one CDU websocket path (e.g. "/winwing/cdu-captain") out to every consumer
    /// registered on it, reference-counting registrations per path so unplugging one
    /// device sharing a seat path doesn't tear the endpoint out from under another.
    /// </summary>
    internal interface ICduWebsocketHub
    {
        void Register(string path, ICduDataConsumer consumer);
        void Unregister(string path, ICduDataConsumer consumer);
        void BroadcastData(string path, string json);
        void BroadcastFont(string path, string json);
    }
}
