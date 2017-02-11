namespace SimpleWatchdog
{
    class Program
    {
        static void Main()
        {
            // mimics Arduino calling structure
            var simpleWatchdog = new SimpleWatchdog { RunLoop = true };
            simpleWatchdog.Setup();
            while (simpleWatchdog.RunLoop) simpleWatchdog.Loop();
            simpleWatchdog.Exit();
        }

    }
}
