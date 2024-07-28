using HidSharp;
using System;

namespace MobiFlightWwFcu
{
    internal class WinwingMessageSender
    {
        private readonly int VendorId = 0x4098;
        private int ProductId = 0xBB10;
        private HidStream Stream { get; set; }
        private HidDevice Device { get; set; }

        private object StreamLock = new object();
        
        private byte Counter = 0;
        private byte[] LightControlMessage = new byte[14] { 0x02, 0x10, 0xbb, 0, 0, 3, 0x49, 3, 0, 0, 0, 0, 0, 0 };
        private byte[] HeartBeatMessage = new byte[14] { 0x02, 0x01, 0, 0, 0, 0x01, 0x00, 0, 0, 0, 0, 0, 0, 0 };
        private byte[] RequestFirmwareMessage = new byte[14] { 0x02, 0x01, 0, 0, 0, 0x01, 0x02, 0, 0, 0, 0, 0, 0, 0 };

        internal WinwingMessageSender(int productId)
        {
            ProductId = productId;
        }

        internal bool IsConnected()
        { 
            return Stream != null; 
        }

        internal void Connect()
        {
            Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
            if (Device == null) return;
            Stream = Device.Open();
            Stream.ReadTimeout = System.Threading.Timeout.Infinite;
        }

        internal void Shutdown()
        {
            try
            {
                if (IsConnected()) 
                { 
                    Stream.Close();
                    Stream = null;
                }
            }
            catch
            {
                // do nothing if issue on shutdown
            }
        }

        /// <summary>
        /// Send display message
        /// </summary>
        /// <param name="message">Message with 64 bytes. First byte report id 0xf0</param>
        internal void SendDisplayMessage(byte[] message)
        {
            byte[] time = GetTimeAsBytes();
            message[2] = ++Counter;
            message[12] = time[0];
            message[13] = time[1];
            message[14] = time[2];        
            WriteStream(message, 0, 64);
        }

        internal void SendTwoDisplayCommandsInOneMessage(byte[] message)
        {
            byte[] time = GetTimeAsBytes();
            message[2] = ++Counter;
            message[12] = time[0];
            message[13] = time[1];
            message[14] = time[2];

            int startData = 21;
            int dataLength = message[17]; // data length command 1
            int startNextCommand = startData + dataLength; // start command 2
            int timeOffsetCommandTwo = startNextCommand + 8;

            // Use same message id for message 2
            message[timeOffsetCommandTwo] = time[0];
            message[timeOffsetCommandTwo + 1] = time[1];
            message[timeOffsetCommandTwo + 2] = time[2];

            WriteStream(message, 0, 64);
        }

        /// <summary>
        /// Send a light control message
        /// </summary>
        /// <param name="destination">Destination device as 2 bytes</param>
        /// <param name="data">Data as 2 bytes</param>
        internal void SendLightControlMessage(byte[] destination, byte[] data) 
        {
            // Update message
            LightControlMessage[1] = destination[0];
            LightControlMessage[2] = destination[1];
            LightControlMessage[7] = data[0];
            LightControlMessage[8] = data[1];

            // Send message
            WriteStream(LightControlMessage, 0, 14);
        }

        internal void SendHeartBeatMessage()
        {
            WriteStream(HeartBeatMessage, 0, 14);
        }

        internal void SendRequestFirmwareMessage()
        {
            WriteStream(RequestFirmwareMessage, 0, 14);
        }

        private void WriteStream(byte[] buffer, int offset, int count)
        {
            if (Stream == null)
            {
                throw new ApplicationException("WinwingDisplayControl cannot send data. Not connected to device. Stream is null.");
            }
            lock (StreamLock)
            {
                Stream.Write(buffer, offset, count);
            }
        }

        private byte[] GetTimeAsBytes()
        {
            DateTime time = DateTime.Now;
            byte[] timeBytes = new byte[3];
            timeBytes[0] = (byte)(time.Millisecond / 4);
            timeBytes[1] = (byte)(time.Second * 3);
            timeBytes[2] = (byte)time.Minute;
            return timeBytes;
        }
    }
}
