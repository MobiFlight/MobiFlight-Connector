using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.Design;
using CommandMessenger;
using MobiFlight.Base;
using MobiFlight.CustomDevices;
using Newtonsoft.Json.Linq;
using SharpDX.DirectInput;

namespace MobiFlight
{
    public class MobiFlightCustomDevice : IConnectedDevice
    {
        public const string TYPE = "CustomDevice";

        public CmdMessenger CmdMessenger { get; set; }
        public string Name { get; set; } = "Custom Device";
        public DeviceType Type { get; set; } = DeviceType.CustomDevice;
        public int DeviceNumber { get; set; }
        public CustomDevices.CustomDevice CustomDevice { get; set; }

        public void Display(string MessageType, string value)
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
        }
    }
}
