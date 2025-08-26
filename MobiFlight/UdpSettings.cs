namespace MobiFlight
{
    /// <summary>
    /// Configuration settings for UDP communication
    /// </summary>
    public class UdpSettings
    {
        public string LocalIp { get; set; }
        public int LocalPort { get; set; }
        public string RemoteIp { get; set; }
        public int RemotePort { get; set; }

        public UdpSettings()
        {
        }

        public UdpSettings(string localIp, int localPort, string remoteIp, int remotePort)
        {
            LocalIp = localIp;
            LocalPort = localPort;
            RemoteIp = remoteIp;
            RemotePort = remotePort;
        }
    }
}