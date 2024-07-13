using HidSharp;
using System;
using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    internal class WinWingMessageSender
    {
        private readonly int VendorId = 0x4098;
        private int ProductId = 0xBB10;
        private HidStream Stream { get; set; }
        private HidDevice Device { get; set; }

        private object StreamLock = new object();

        private MsgEntry TimeBlock = new MsgEntry { StartPos = 12, Mask = new byte[3], Data = new byte[] { 0x01, 0x00, 0x00 } };
        private MsgEntry Counter = new MsgEntry { StartPos = 2, Mask = new byte[1], Data = new byte[] { 0x00 } };

        private byte[] LightControlMessage = new byte[14] { 0x02, 0x10, 0xbb, 0, 0, 3, 0x49, 3, 0, 0, 0, 0, 0, 0 };
        private byte[] HeartBeatMessage = new byte[14] { 0x02, 0x01, 0, 0, 0, 0x01, 0x00, 0, 0, 0, 0, 0, 0, 0 };
        private byte[] RequestFirmwareMessage = new byte[14] { 0x02, 0x01, 0, 0, 0, 0x01, 0x02, 0, 0, 0, 0, 0, 0, 0 };

        internal WinWingMessageSender(int productId)
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

        internal void SendDisplayMessage(List<byte> message)
        {
            Counter.Data[0]++;  
            TimeBlock.Data = GetTimeAsBytes();
            SetBytesDisplayMessage(TimeBlock, message);
            SetBytesDisplayMessage(Counter, message);

            WriteStream(message.ToArray(), 0, 64);
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

        internal void WriteStream(byte[] buffer, int offset, int count)
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

        private void SetBytesDisplayMessage(MsgEntry msgEntry, List<byte> message)
        {
            byte setPos = msgEntry.StartPos;
            for (int i = 0; i < msgEntry.Data.Length; i++)
            {
                message[setPos] &= msgEntry.Mask[i];
                message[setPos] |= msgEntry.Data[i];
                setPos++;
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
