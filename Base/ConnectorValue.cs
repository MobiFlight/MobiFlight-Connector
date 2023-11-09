using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace MobiFlight
{
    public class ConnectorValue : ICloneable
    {
        public FSUIPCOffsetType type;
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
                    result = Math.Round(Float64).ToString();
                    break;
                case FSUIPCOffsetType.Float:
                    result = Float64.ToString("R");
                    break;
                /*case FSUIPCOffsetType.UnsignedInt:
                    result = Uint64.ToString();
                    break;*/
            }

            return result;
        }

        public object Clone()
        {
            ConnectorValue c = new ConnectorValue();
            
            // clone properties
            c.type = this.type;
            c.Float64 = this.Float64;
            c.String = this.String;

            return c;
        }

        public bool Equals(ConnectorValue other)
        {
            return
                type == other.type &&
                Float64 == other.Float64 &&
                String == other.String;
        }
    }
}
