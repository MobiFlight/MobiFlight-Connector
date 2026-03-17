using CommandMessenger;

namespace MobiFlight
{
    public class MobiFlightCustomDevice : IConnectedDevice
    {
        public const string TYPE = "CustomDevice";
        public const int MESSAGE_STOP = -1;

        public CmdMessenger CmdMessenger { get; set; }
        public string Name { get; set; } = "Custom Device";
        public DeviceType TypeDeprecated { get; set; } = DeviceType.CustomDevice;
        public int DeviceNumber { get; set; }
        public CustomDevices.CustomDevice CustomDevice { get; set; }

        public void Display(int MessageType, string value)
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetCustomDevice);

            command.AddArgument(DeviceNumber);
            command.AddArgument(MessageType);            
            command.AddArgument(value);

            Log.Instance.log($"Command: SetCustomDevice <{(int)MobiFlightModule.Command.SetCustomDevice},{DeviceNumber},{MessageType},{value};>.", LogSeverity.Debug);

            // Send command
            System.Threading.Thread.Sleep(1);
            CmdMessenger.SendCommand(command);
        }

        // Blank the display when stopped
        public void Stop()
        {
            var command = new SendCommand((int)MobiFlightModule.Command.SetCustomDevice);

            command.AddArgument(DeviceNumber);
            command.AddArgument(MESSAGE_STOP);

            Log.Instance.log($"Command: SetCustomDevice (Stop) <{(int)MobiFlightModule.Command.SetCustomDevice},{DeviceNumber},{MESSAGE_STOP};>.", LogSeverity.Debug);

            // Send command
            System.Threading.Thread.Sleep(1);
            CmdMessenger.SendCommand(command);
        }
    }
}
