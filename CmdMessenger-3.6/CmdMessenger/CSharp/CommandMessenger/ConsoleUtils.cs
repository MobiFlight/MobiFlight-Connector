using System;
using System.Runtime.InteropServices;

namespace CommandMessenger
{
    public class ConsoleUtils
    {
        public static EventHandler ConsoleClose = delegate {};

        static ConsoleUtils()
        {
            _handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(_handler, true);
            Console.WriteLine("check");
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                ConsoleClose(null, EventArgs.Empty);                
            }
            ConsoleClose= null;
            _handler = null;
            return false;
        }

        static ConsoleEventDelegate _handler;   // Keeps it from getting garbage collected
       
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

    }
}
