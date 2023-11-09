using MobiFlight.SimConnectMSFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlightUnitTests.mock.SimConnectMSFS
{
    class SimConnectCacheMock : SimConnectCacheInterface
    {
        public List<String> Writes = new List<String>();
        public List<String> Reads = new List<String>();

        public event EventHandler Closed { add { } remove { } }
        public event EventHandler Connected { add { } remove { } }
        public event EventHandler ConnectionLost { add { } remove { } }

        public void Clear()
        {
            return;
        }

        public bool Connect()
        {
            return true;
        }

        public bool Disconnect()
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

        public bool IsConnected()
        {
            return true;
        }

        public void setOffset(int offset, byte value)
        {
           
            return;
        }

        public void setOffset(int offset, short value)
        {
           
            return;
        }

        public void setOffset(int offset, int value, bool writeOnly)
        {
           
            return;
        }

        public void setOffset(int offset, float value)
        {
            Writes.Add(value.ToString());
            return;
        }

        public void setOffset(int offset, double value)
        {
            Writes.Add(value.ToString());
            return;
        }

        public void setOffset(int offset, string value)
        {
            Writes.Add(value.ToString());
            return;
        }

        public void executeMacro(string macroName, int paramValue)
        { 
            //FIXME: need to mock this correctly.
            return;
        }

        public void setEventID(int eventID, int param)
        {
            Writes.Add("SetEventID>" + eventID + ">" + param);
            return;
        }

        public void setEventID(string eventID)
        {
            Writes.Add("SetEventID>" + eventID);
            return;
        }

        public void Write()
        {
            Writes.Add("Write");
            return;
        }

        public void SetSimVar(string SimVarCode)
        {
            Writes.Add(SimVarCode);
            return;
        }
    }

    class FSUIPCMockOffset
    {
        public int Offset = 0;
        public String Value = null;
    }
}
