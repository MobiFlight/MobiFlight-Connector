namespace SendAndReceive
{
    class Program
    {
        static void Main()
        {
            // mimics Arduino calling structure
            var sendAndReceive = new SendAndReceive { RunLoop = true };
            sendAndReceive.Setup();
            while (sendAndReceive.RunLoop) sendAndReceive.Loop();
            sendAndReceive.Exit();
        }
    }
}
