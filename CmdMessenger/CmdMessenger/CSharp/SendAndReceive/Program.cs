namespace SendAndReceive
{
    using CommandMessenger;
    class Program
    {
        static void Main()
        {
            
            // mimics Arduino calling structure
            var sendAndReceive = new SendAndReceive { RunLoop = true };
            ConsoleUtils.ConsoleClose += (o, i) => sendAndReceive.Exit();      
            sendAndReceive.Setup();
            while (sendAndReceive.RunLoop) sendAndReceive.Loop();
            sendAndReceive.Exit();
            
        }
    }
}
