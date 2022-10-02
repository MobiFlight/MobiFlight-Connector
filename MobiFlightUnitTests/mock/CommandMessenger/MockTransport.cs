using CommandMessenger.TransportLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlightUnitTests.mock.CommandMessenger
{
    public class MockTransport : ITransport
    {
        public string DataRead = "";
        public string DataWrite = "";

        int ITransport.BytesInBuffer()
        {
            return DataRead.Length;
        }

        byte[] ITransport.Read()
        {
            return Encoding.UTF8.GetBytes(DataRead);
        }

        void ITransport.Poll()
        {
        }

        bool ITransport.Connect()
        {
            return true;
        }

        bool ITransport.Disconnect()
        {
            return true;
        }

        bool ITransport.IsConnected()
        {
            return true;
        }

        void ITransport.StartPolling()
        {
        }

        void ITransport.StopPolling()
        {
        }

        void ITransport.Write(byte[] buffer)
        {
            DataWrite = Encoding.UTF8.GetString(buffer);
        }

        event EventHandler ITransport.NewDataReceived
        {
            add
            {
                //
            }

            remove
            {
                //
            }
        }

        void IDisposable.Dispose()
        {
        }

        public void Clear()
        {
            DataRead = "";
            DataWrite = "";
        }
    }
}
