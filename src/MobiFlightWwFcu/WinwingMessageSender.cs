using HidSharp;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MobiFlightWwFcu
{
    internal class WinwingMessageSender : IWinwingMessageSender
    {
        private readonly int VendorId = 0x4098;
        private int ProductId = 0xBB10;
        private HidStream Stream { get; set; }
        private HidDevice Device { get; set; }

        private object StreamLock = new object();
        
        private int Counter = 0;                 

        public WinwingMessageSender(int productId)
        {
            ProductId = productId;
        }

        public bool IsConnected()
        { 
            return Stream != null; 
        }

        public void Connect()
        {
            Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
            if (Device == null) return;
            Stream = Device.Open();
            Stream.ReadTimeout = System.Threading.Timeout.Infinite;
        }

        public void Shutdown()
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

        internal byte[] GetNewMessage()
        {
            byte[] message = new byte[64];
            message[0] = 0xf0;

            // Explicitly wrap at 256 (keeps values 0-255)
            int nextCounter = System.Threading.Interlocked.Increment(ref Counter);
            message[2] = (byte)(nextCounter & 0xFF);  // Or: nextCounter % 256
            return message;
        }

        public void SendDisplayCommands(IList<byte[]> commands)
        {
            int indexHeaderEnd = 3;
            byte[] id = GetTimeAsBytes();
            byte[] message = GetNewMessage();
            int currentMessageIndex = indexHeaderEnd;

            for (int k = 0; k < commands.Count; k++)
            {
                byte[] command = commands[k];
                for (int i = 0; i < command.Length; i++)
                {
                    ++currentMessageIndex;
                    if (i == 8)
                    {
                        message[currentMessageIndex] = id[0];
                    }
                    else if (i == 9)
                    {
                        message[currentMessageIndex] = id[1];
                    }
                    else if (i == 10)
                    {
                        message[currentMessageIndex] = id[2];
                    }
                    else
                    {
                        message[currentMessageIndex] = command[i];
                    }

                    bool isOverallLastByte = (k == commands.Count - 1) && (i == command.Length - 1);
                    if (currentMessageIndex == 59 || isOverallLastByte)
                    {
                        // Message ready to send. Set message data length.
                        message[indexHeaderEnd] = (byte)(currentMessageIndex - indexHeaderEnd);
                        WriteStream(message, 0, 64);
                        message = GetNewMessage();
                        currentMessageIndex = indexHeaderEnd;                      
                    }                    
                }                
            }
        }

        public void SendDisplayCommandsCopilot(IList<byte[]> commands)
        {
            const int HEADER_SIZE = 3;
            const int MAX_PAYLOAD = 56; // 64 - header - length byte

            byte[] timeId = GetTimeAsBytes();
            byte[] message = GetNewMessage();
            int payloadIndex = 0; // Index within the current 64-byte message payload

            foreach (var command in commands)
            {
                for (int cmdByteIndex = 0; cmdByteIndex < command.Length; cmdByteIndex++)
                {
                    int messageIndex = HEADER_SIZE + 1 + payloadIndex; // +1 for length byte

                    // Insert time bytes at specific command positions
                    if (cmdByteIndex == 8)
                        message[messageIndex] = timeId[0];
                    else if (cmdByteIndex == 9)
                        message[messageIndex] = timeId[1];
                    else if (cmdByteIndex == 10)
                        message[messageIndex] = timeId[2];
                    else
                        message[messageIndex] = command[cmdByteIndex];

                    payloadIndex++;

                    bool isLastByte = (command == commands[commands.Count - 1]) &&
                                      (cmdByteIndex == command.Length - 1);

                    if (payloadIndex >= MAX_PAYLOAD || isLastByte)
                    {
                        message[HEADER_SIZE] = (byte)payloadIndex;
                        WriteStream(message, 0, 64);
                        message = GetNewMessage();
                        payloadIndex = 0;
                    }
                }
            }
        }


        public void SendCduDisplayBytes(byte[] byteList)
        {
            byte[] message = new byte[64];
            int counterCurrentMessage = 0;

            for (int i = 0; i < byteList.Length; i++)
            {
                counterCurrentMessage++;

                // Start of new message
                if (counterCurrentMessage == 1)
                {
                    message[0] = 0xf2;
                }

                message[counterCurrentMessage] = byteList[i];

                if ((counterCurrentMessage == 63) || (i == byteList.Length - 1))
                {
                    // Send message
                    WriteStream(message, 0, 64);

                    // Reset message
                    message = new byte[64];
                    counterCurrentMessage = 0;
                }
            }
        }


        /// <summary>
        /// Send a light control message
        /// </summary>
        /// <param name="destination">Destination device as 2 bytes</param>
        /// <param name="type">Type as byte</param>
        /// <param name="value">Value as byte</param>
        public void SendLightControlMessage(byte[] destination, byte type, byte value)
        {
            byte[] lightControlMessage = new byte[14] { 0x02, 0x10, 0xbb, 0, 0, 0x03, 0x49, 0x03, 0, 0, 0, 0, 0, 0 };

            // Update message
            lightControlMessage[1] = destination[0];
            lightControlMessage[2] = destination[1];
            lightControlMessage[7] = type;
            lightControlMessage[8] = value;

            // Send message
            WriteStream(lightControlMessage, 0, 14);
        }

        public void SetBrightness(byte[] destinationAddress, byte type, string brightness)
        {
            double bright = Convert.ToDouble(brightness, CultureInfo.InvariantCulture);
            SetBrightnessInternal(destinationAddress, type, bright);
        }

        public void SetBrightness(byte[] destinationAddress, byte type, int brightness)
        {          
            SetBrightnessInternal(destinationAddress, type, brightness);
        }

        private void SetBrightnessInternal(byte[] destinationAddress, byte type, double brightness)
        {
            // Input should be 0 to 100 percent - scale to 0..255
            int value = (int)Math.Round(brightness * 2.55);
            byte byteValue = value >= 255 ? (byte)255 : (byte)value;
            SendLightControlMessage(destinationAddress, type, byteValue);
        }

        public void SetVibration(byte[] destinationAddress, byte type, byte level)
        {
            // Input should be 0 to 100 percent - scale to 0..255
            int value = (int)Math.Round(level * 2.55);
            byte byteValue = value >= 255 ? (byte)255 : (byte)value;
            SendLightControlMessage(destinationAddress, type, byteValue);
        }

        public void SetPulseLight(byte[] destinationAddress, bool isOn)
        {
            byte[] pulseLightControlMessage = new byte[14] { 0x02, 0x20, 0xbb, 0, 0, 0x08, 0x06, 0xf8, 0, 0, 0, 0xff, 0xff, 0xff };

            // Update message
            pulseLightControlMessage[1] = destinationAddress[0];
            pulseLightControlMessage[2] = destinationAddress[1];
            pulseLightControlMessage[10] = Convert.ToByte(!isOn);

            // Send message
            WriteStream(pulseLightControlMessage, 0, 14);
        }

        public void SendHeartBeatMessage()
        {
            byte[] heartBeatMessage = new byte[14] { 0x02, 0x01, 0x00, 0, 0, 0x01, 0x00, 0x00, 0, 0, 0, 0, 0, 0 };
            WriteStream(heartBeatMessage, 0, 14);
        }

        public void SendRequestFirmwareMessage()
        {
            byte[] requestFirmwareMessage = new byte[14] { 0x02, 0x01, 0x00, 0, 0, 0x01, 0x02, 0x00, 0, 0, 0, 0, 0, 0 };
            WriteStream(requestFirmwareMessage, 0, 14);
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
