using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.Base
{
    public interface ReadCacheInterface
    {
        long getValue(Int32 offset, byte size);

        //ulong getUValue(Int32 offset, byte size);

        long getLongValue(Int32 offset, byte size);
        
        double getFloatValue(Int32 offset, byte size);

        double getDoubleValue(int offset, byte size);

        string getStringValue(Int32 offset, byte size);
    }
}
