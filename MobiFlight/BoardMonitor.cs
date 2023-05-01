using HidSharp.Utility;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MobiFlight
{
    public class BoardMonitor
    {
        public event EventHandler<PortDetails> PortAvailable;
        public event EventHandler<PortDetails> PortUnavailable;
        public event EventHandler<MobiFlightModuleInfo> BoardDetected;
        public event EventHandler<MobiFlightModuleInfo> BoardConnected;
        public event EventHandler<MobiFlightModuleInfo> BoardDisconnected;

        protected Timer _timer = new Timer() { Interval = 1000 };
        protected List<PortDetails> discoveredPorts = new List<PortDetails>();
        protected List<MobiFlightModuleInfo> discoveredBoards = new List<MobiFlightModuleInfo>();

        public void Start()
        {
            PortAvailable += BoardMonitor_PortAvailable;
            _timer.Tick += Check;
            _timer.Start();
        }

        private async void BoardMonitor_PortAvailable(object sender, PortDetails port)
        {
            var board = await Task.Run(() =>
            {
                MobiFlightModule tmp = new MobiFlightModule(port.Name, port.Board);
                // ModuleConnecting?.Invoke(this, "Scanning modules", progressValue);
                tmp.Connect();
                MobiFlightModuleInfo devInfo = tmp.GetInfo() as MobiFlightModuleInfo;
                // Store the hardware ID for later use
                devInfo.HardwareId = port.HardwareId;

                tmp.Disconnect();
                // ModuleConnecting?.Invoke(this, "Scanning modules", progressValue + 5);

                // result.Add(devInfo);

                return devInfo;
            });
            discoveredBoards.Add(board);
            BoardDetected?.Invoke(this, board);
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Tick -= Check;
        }

        private void Check(object sender, EventArgs e)
        {
            var connectedPorts = SerialPort.GetPortNames();
            var supportedPorts = GetSupportedPorts();
            var ignoredPorts = GetIgnoredPorts();

            for (var i = 0; i != supportedPorts.Count; i++)
            {
                var port = supportedPorts.ElementAt(i);

                if (!connectedPorts.Contains(port.Name))
                {
                    // Log.Instance.log($"Port not connected {port.Name}.", LogSeverity.Debug);
                    continue;
                }
                if (discoveredPorts.Where(p => p.Name == port.Name).Count() > 0)
                {
                    // Log.Instance.log($"Port already connecting {port.Name}.", LogSeverity.Debug);
                    continue;
                }
                if (ignoredPorts.Contains(port.Name))
                {
                    // Log.Instance.log($"Skipping {port.Name} since it is in the list of ports to ignore.", LogSeverity.Info);
                    discoveredBoards.Add(new MobiFlightModuleInfo()
                    {
                        Port = port.Name,
                        Type = "Ignored",
                        Name = $"Ignored Device at Port {port.Name}",
                        Board = port.Board,
                        HardwareId = port.HardwareId
                    });
                    // we don't invoke BoardDetected
                    continue;
                }

                discoveredPorts.Add(port); 
                PortAvailable?.Invoke(this, port);
            }

            // test for disconnected boards
            discoveredPorts.Where(port => !connectedPorts.Any(availablePort => availablePort == port.Name))
                           .ToList()
                           .ForEach(disconnectedPort =>
                           {
                               discoveredPorts.Remove(disconnectedPort);
                               PortUnavailable?.Invoke(this, disconnectedPort);
                           });
        }

        public static List<string> GetIgnoredPorts()
        {
            List<String> ports = new List<string>();
            if (Properties.Settings.Default.IgnoreComPorts)
            {
                ports = Properties.Settings.Default.IgnoredComPortsList.Split(',').ToList();
            }
            return ports;
        }

        public static List<PortDetails> GetSupportedPorts()
        {
            var portNameRegEx = "\\(.*\\)";
            var result = new List<PortDetails>();
            var regex = new Regex(@"(?<id>VID_\S*)"); // Pattern to match the VID/PID of the connected devices

            // Code from https://stackoverflow.com/questions/45165299/wmi-get-list-of-all-serial-com-ports-including-virtual-ports
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    // At this point we have a list of possibly valid connected devices. Since everything at this point
                    // depends on the VID/PID extract that to start. USB devices seem to consistently have two hardwareID
                    // entries, in this order:
                    //
                    // USB\VID_1B4F&PID_9206&REV_0100&MI_00
                    // USB\VID_1B4F&PID_9206&MI_00
                    //
                    // Either will work with how the BoardDefinitions class and existing board definition files do regular expression
                    // lookups so just grab the first one in the array every time. Note the use of '?' to handle the (never seen)
                    // case where no hardware IDs are available.
                    var rawHardwareID = (queryObj["HardwareID"] as string[])?[0];

                    if (String.IsNullOrEmpty(rawHardwareID))
                    {
                        Log.Instance.log($"Skipping module with no available VID/PID.", LogSeverity.Debug);
                        continue;
                    }

                    // Historically MobiFlight expects a straight VID/PID string without a leading USB\ or FTDI\ or \COMPORT so get
                    // pick that out of the raw hardware ID.
                    var match = regex.Match(rawHardwareID);
                    if (!match.Success)
                    {
                        Log.Instance.log($"Skipping device with no available VID/PID ({rawHardwareID}).", LogSeverity.Debug);
                        continue;
                    }

                    // Get the matched hardware ID and use it going forward to identify the board.
                    var hardwareId = match.Groups["id"].Value;

                    Log.Instance.log($"Checking for compatible module: {hardwareId}.", LogSeverity.Debug);
                    var board = BoardDefinitions.GetBoardByHardwareId(hardwareId);

                    // If no matching board definition is found at this point then it's an incompatible board and just keep going.
                    if (board == null)
                    {
                        Log.Instance.log($"Incompatible module skipped: {hardwareId}.", LogSeverity.Debug);
                        continue;
                    }

                    // The board is a known type so grab the COM port for it. Every USB device seen so far has the
                    // COM port in the full name of the device surrounded by (), for example:
                    //
                    // USB Serial Device (COM22)
                    var portNameMatch = Regex.Match(queryObj["Caption"].ToString(), portNameRegEx); // Find the COM port.
                    var portName = portNameMatch?.Value.Trim(new char[] { '(', ')' }); // Remove the surrounding ().

                    if (portName == null)
                    {
                        Log.Instance.log($"Device has no port information: {hardwareId}.", LogSeverity.Error);
                        continue;
                    }

                    // Safety check to ensure duplicate entires in the registry don't result in duplicate entires in the list.
                    if (result.Any(p => p.Name == portName))
                    {
                        Log.Instance.log($"Duplicate entry for port: {board.Info.FriendlyName} {portName}.", LogSeverity.Error);
                        continue;
                    }

                    result.Add(new PortDetails
                    {
                        Board = board,
                        HardwareId = hardwareId,
                        Name = portName
                    });

                    Log.Instance.log($"Found potentially compatible module ({board.Info.FriendlyName}): {hardwareId}@{portName}.", LogSeverity.Debug);
                }
            }
            catch (ManagementException ex)
            {
                // Issue #1122: A corrupted WMI registry caused exceptions when attempting to enumerate connected devices with searcher.Get().
                // Running "winmgmt /resetrepository" fixed it.
                Log.Instance.log($"Unable to read connected devices. This is usually caused by a corrupted WMI registry. Run 'winmgmt /resetrepository' from an elevated command line to resolve the issue. ({ex.Message})", LogSeverity.Error);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to read connected devices: {ex.Message}", LogSeverity.Error);
            }
            return result;
        }
    }
}
