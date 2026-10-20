using System;

namespace CommandMessenger.Transport.Serial
{
    public class ConnectionFailedEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ConnectionFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}