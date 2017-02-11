using System;
using System.Windows.Forms;

namespace CommandMessenger.ConnectionManager2
{
    public interface IConnectionManager : IDisposable
    {
        void SetControlToInvokeOn(Control controlToInvokeOn);
        void StartScan();
        void StopScan();
        void StartWatchDog(long watchdogTimeOut);
        void StopWatchDog();

        event EventHandler ConnectionTimeout;
        event EventHandler ConnectionFound;
        event EventHandler<ConnectionManagerProgressEventArgs> Progress;
    }
}
