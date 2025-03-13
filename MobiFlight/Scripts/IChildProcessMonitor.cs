using System.Diagnostics;

namespace MobiFlight.Scripts
{
    internal interface IChildProcessMonitor
    {
        void AddChildProcess(Process process);
    }
}
