using FliteSimFfbMock;

class Program
{
    static FfbUdpSimulator? simulator;

    static void Main(string[] args)
    {
        int receivePort = 49110;
        int sendPort = 49111;
        string targetIp = "127.0.0.1";

        simulator = new FfbUdpSimulator(receivePort, sendPort, targetIp);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- FliteSim FFB Mock CLI ---");
            Console.WriteLine("1. Start Simulator");
            Console.WriteLine("2. Stop Simulator");
            Console.WriteLine("3. Send Value");
            Console.WriteLine("4. Trigger Handshake");
            Console.WriteLine("5. Trigger End Message");
            Console.WriteLine("6. Exit");
            Console.Write("Select an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    simulator.Start();
                    Console.WriteLine("Simulator started.");
                    break;
                case "2":
                    simulator.Stop();
                    Console.WriteLine("Simulator stopped.");
                    break;
                case "3":
                    Console.Write("Enter value to send: ");
                    if (float.TryParse(Console.ReadLine(), out float value))
                    {
                        Console.Write("Enter channel (0-9): ");
                        if (int.TryParse(Console.ReadLine(), out int channel))
                        {
                            simulator.SendValue(value, channel);
                        }
                        else
                        {
                            Console.WriteLine("Invalid channel.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid value.");
                    }
                    break;
                case "4":
                    simulator.TriggerHandshake();
                    break;
                case "5":
                    simulator.TriggerEndMessage();
                    break;
                case "6":
                    running = false;
                    simulator.Stop();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        Console.WriteLine("Exiting FliteSim FFB Mock CLI.");
    }
}