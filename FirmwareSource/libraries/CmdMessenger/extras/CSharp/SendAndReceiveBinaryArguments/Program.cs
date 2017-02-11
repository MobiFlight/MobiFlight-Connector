namespace SendAndReceiveBinaryArguments
{
    class Program
    {
        static void Main()
        {
            // mimics Arduino calling structure
            var sendAndReceiveBinaryArguments = new SendAndReceiveBinaryArguments { RunLoop = true };
            sendAndReceiveBinaryArguments.Setup();
            while (sendAndReceiveBinaryArguments.RunLoop) sendAndReceiveBinaryArguments.Loop();
            sendAndReceiveBinaryArguments.Exit();
        }

    }
}
