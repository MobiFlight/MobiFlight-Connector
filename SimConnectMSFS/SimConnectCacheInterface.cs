using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.SimConnectMSFS
{
    public interface SimConnectCacheInterface : Base.WriteCacheInterface
    {
        void SetSimVar(String SimVarCode);
    }
}