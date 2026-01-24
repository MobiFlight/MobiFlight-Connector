namespace MobiFlight
{
    public interface IConnectionDetails
    {
        string Name { get; }
    }

    /// <summary>
    /// Provides raw information about detected ports and the device attached to the port
    /// </summary>
    public class PortDetails : IConnectionDetails
    {
        public Board Board { get; set; }
        public string HardwareId { get; set; }
        public string Name { get; set; }
    }

    public class UsbPortDetails : PortDetails 
    { 
        public string Path { get; set; }
    }

    public class HidConnectionDetails : IConnectionDetails
    {
        public string Name { get; set; }
        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public string Path { get; set; }
    }
}
