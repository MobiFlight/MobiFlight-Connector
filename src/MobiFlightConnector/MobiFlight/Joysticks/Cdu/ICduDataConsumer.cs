namespace MobiFlight.Joysticks.Cdu
{
    /// <summary>
    /// One device driven by a CDU websocket path (e.g. a Winwing CDU, or a MOZA MCDU
    /// sharing the same seat path). <see cref="CduWebsocketHub"/> fans incoming messages
    /// out to every consumer registered on a path.
    /// </summary>
    internal interface ICduDataConsumer
    {
        void OnCduData(string json);
        void OnCduFont(string json);
    }
}
