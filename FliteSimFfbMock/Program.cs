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

        // Start auto-control data sending if simulator is connected
        var autoSendTask = Task.Run(AutoSendLoop);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- FliteSim FFB Mock CLI ---");
            Console.WriteLine("1. Start Simulator");
            Console.WriteLine("2. Stop Simulator");
            Console.WriteLine("3. Handshake");
            Console.WriteLine("4. Send Control Data");
            Console.WriteLine("5. Toggle Auto Control");
            Console.WriteLine("6. Set Control Value");
            Console.WriteLine("7. Send Quit");
            Console.WriteLine("8. Status");
            Console.WriteLine("9. Exit");
            Console.Write("Select an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    simulator.Start();
                    break;
                case "2":
                    simulator.Stop();
                    break;
                case "3":
                    simulator.InitiateHandshake();
                    break;
                case "4":
                    simulator.SendControlData();
                    break;
                case "5":
                    simulator.ToggleAutoControl();
                    break;
                case "6":
                    SetControlValue();
                    break;
                case "7":
                    Console.Write("Enter quit reason (0=normal, 1=user abort, 2=error): ");
                    if (float.TryParse(Console.ReadLine(), out float reason))
                    {
                        simulator.SendQuit(reason);
                    }
                    else
                    {
                        simulator.SendQuit(0.0f);
                    }
                    break;
                case "8":
                    ShowStatus();
                    break;
                case "9":
                    running = false;
                    simulator.Stop();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            // Check for handshake timeouts
            simulator.CheckHandshakeTimeout();
        }

        Console.WriteLine("Exiting FliteSim FFB Mock CLI.");
    }

    static void SetControlValue()
    {
        Console.WriteLine("\nControl Channels:");
        Console.WriteLine("0-2: Override controls (Pitch, Roll, Heading)");
        Console.WriteLine("3-5: Control ratios (Pitch, Roll, Heading) [-1 to 1]");
        Console.WriteLine("6-8: Trim override controls");
        Console.WriteLine("9-11: Trim values (Elevator, Aileron, Rudder) [-1 to 1]");
        Console.WriteLine("12: Autopilot disconnect (0/1)");
        
        Console.Write("Enter channel (0-12): ");
        if (int.TryParse(Console.ReadLine(), out int channel))
        {
            Console.Write("Enter value: ");
            if (float.TryParse(Console.ReadLine(), out float value))
            {
                simulator?.SetControlValue(channel, value);
            }
            else
            {
                Console.WriteLine("Invalid value.");
            }
        }
        else
        {
            Console.WriteLine("Invalid channel.");
        }
    }

    static void ShowStatus()
    {
        Console.WriteLine("\n--- Status ---");
        Console.WriteLine($"Connected: {simulator?.IsConnected ?? false}");
        Console.WriteLine($"Auto Control: {simulator?.AutoControlEnabled ?? false}");
        Console.WriteLine($"Ports: Receive={49110}, Send={49111}");
        Console.WriteLine($"Target: 127.0.0.1");
    }

    static async Task AutoSendLoop()
    {
        while (true)
        {
            await Task.Delay(100); // Send at 10Hz when auto-control is enabled
            
            if (simulator?.IsConnected == true && simulator?.AutoControlEnabled == true)
            {
                simulator.SendControlData();
            }
        }
    }
}