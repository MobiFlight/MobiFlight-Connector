using MobiFlight;
using MobiFlight.FSUIPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlightUnitTests.mock.FSUIPC
{
    class FSUIPCCacheMock : FSUIPCCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;

        public List<FSUIPCMockOffset> Writes = new List<FSUIPCMockOffset>();
        public List<FSUIPCMockOffset> Reads = new List<FSUIPCMockOffset>();

        public void Clear()
        {
            return;
        }

        public bool connect()
        {
            return true;
        }

        public bool disconnect()
        {
            return true;
        }

        public double getFloatValue(int offset, byte size)
        {
            return double.MaxValue;
        }

        public long getLongValue(int offset, byte size)
        {
            return long.MaxValue;
        }

        public double getDoubleValue(int offset, byte size)
        {
            return double.MaxValue;
        }

        public string getStringValue(int offset, byte size)
        {
            return "String";
        }

        public long getValue(int offset, byte size)
        {
            return long.MaxValue;
        }

        public bool isConnected()
        {
            return true;
        }

        public void setOffset(int offset, byte value)
        {
            Writes.Add(new FSUIPCMockOffset() { Offset = offset, Value = value.ToString() });
            return;
        }

        public void setOffset(int offset, short value)
        {
            Writes.Add(new FSUIPCMockOffset() { Offset = offset, Value = value.ToString() });
            return;
        }

        public void setOffset(int offset, int value)
        {
            Writes.Add(new FSUIPCMockOffset() { Offset = offset, Value = value.ToString() });
            return;
        }

        public void ForceUpdate()
        {
            Writes.Add(new FSUIPCMockOffset() { Offset = 0x0, Value = "ForceUpdate" });
            return;
        }
    }

    class FSUIPCMockOffset
    {
        public int Offset;
        public String Value;
    }
}
