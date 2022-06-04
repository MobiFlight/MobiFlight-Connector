using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlaneConnector;

namespace MobiFlight.xplane
{
    public interface XplaneCacheInterface : Base.CacheInterface
    {
        void Start();

        void Stop();

        float readDataRef(string dataRefPath);

        void writeDataRef(string dataRefPath, float value);

        void sendCommand(string command);
    }
}
