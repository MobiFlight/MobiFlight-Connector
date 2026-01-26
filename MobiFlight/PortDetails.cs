using HidSharp;
using MobiFlight.Base;

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
        public string DevicePath { get; set; }

        static public HidConnectionDetails FromHidController(HidDevice device)
        {
            return new HidConnectionDetails
            {
                Name = device.GetProductName(),
                VendorId = device.VendorID,
                ProductId = device.ProductID,
                DevicePath = device.DevicePath
            };
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is HidConnectionDetails)) return false;
            var other = obj as HidConnectionDetails;

            return Name == other.Name &&
                   VendorId == other.VendorId &&
                   ProductId == other.ProductId &&
                   DevicePath == other.DevicePath;
        }
    }
}
