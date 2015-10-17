using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace CommandMessenger.Bluetooth
{
    public class BluetoothSettings 
    {
        /// <summary> Default constructor. </summary>
        public BluetoothSettings()
            
        {
        }

        public BluetoothDeviceInfo BluetoothDeviceInfo;
        public string Pin { get; set; }
    }
}
