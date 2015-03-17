using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class ConnectorValue
    {
        public FSUIPCOffsetType type;
        /*public UInt64           Uint64;*/
        public Int64            Int64;
        public Double           Float64;
        public String           String;

        public override string ToString()
        {
            String result = null;
            switch (type)
            {
                case FSUIPCOffsetType.String:
                    result = String;
                    break;
                case FSUIPCOffsetType.Integer:
                    result = Int64.ToString();
                    break;
                case FSUIPCOffsetType.Float:
                    result = Float64.ToString();
                    break;
                /*case FSUIPCOffsetType.UnsignedInt:
                    result = Uint64.ToString();
                    break;*/
            }

            return result;
        }
    }
}
