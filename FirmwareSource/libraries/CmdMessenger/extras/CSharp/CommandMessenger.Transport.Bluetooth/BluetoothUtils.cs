using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace CommandMessenger.Transport.Bluetooth
{
    public class BluetoothUtils
    {
        public static BluetoothEndPoint LocalEndpoint { get; private set; }
        public static BluetoothClient LocalClient { get; private set; }
        public static BluetoothRadio PrimaryRadio { get; private set; }
        private static readonly Guid Guid = Guid.NewGuid(); 

        private static readonly List<BluetoothDeviceInfo> DeviceList;

        private static readonly List<string> CommonDevicePins = new List<string>
            {
                "0000",
                "1111",
                "1234"
            };
        private static NetworkStream _stream;

        public struct SerialPort
        {
            public string Port;
            public string DeviceId;

        }       

        static BluetoothUtils()
        {
            // Define common Pin codes for Bluetooth devices

            PrimaryRadio = BluetoothRadio.PrimaryRadio;
            if (PrimaryRadio == null) {
                //Console.WriteLine("No radio hardware or unsupported software stack");
                return;
            }

            // Local bluetooth MAC address 
            var mac = PrimaryRadio.LocalAddress;
            if (mac == null) {
                //Console.WriteLine("No local Bluetooth MAC address found");
                return;
            }
            DeviceList = new List<BluetoothDeviceInfo>();
            // mac is mac address of local bluetooth device
            //LocalEndpoint = new BluetoothEndPoint(mac, BluetoothService.SerialPort);
            LocalEndpoint = new BluetoothEndPoint(mac, Guid);             
            // client is used to manage connections
            LocalClient = new BluetoothClient(LocalEndpoint);
        }

        public static BluetoothDeviceInfo DeviceByAdress(string address)
        {
            try
            {
                return new BluetoothDeviceInfo(BluetoothAddress.Parse(address));
            }
            catch
            {
                return null;
            }
        }

        public static void PrintPairedDevices()
        {
            DeviceList.AddRange(LocalClient.DiscoverDevices(255, true, true, false, false));
            PrintDevices();
        }

        public static void PrintAllDevices()
        {
            DeviceList.AddRange(LocalClient.DiscoverDevices(65536, true, true, true,true));
            PrintDevices();
        }

        private static BluetoothAddress LocalAddress()
        {
            if (PrimaryRadio == null)
            {
                Console.WriteLine("No radio hardware or unsupported software stack");
                return null;
            }
            // Note that LocalAddress is null if the radio is powered-off.
            //Console.WriteLine("* Radio, address: {0:C}", primaryRadio.LocalAddress);
            return PrimaryRadio.LocalAddress;
        }

        public static void UpdateClient()
        {
            if (LocalClient != null)
            {
                LocalClient.Close();
                LocalClient = new BluetoothClient(LocalEndpoint);
            }
        }

        public static string StripBluetoothAdress(string bluetoothAdress)
        {
            var charsToRemove = new string[] {":", "-"};
            return charsToRemove.Aggregate(bluetoothAdress, (current, c) => current.Replace(c, string.Empty));
        }


        public static bool PairDevice(BluetoothDeviceInfo device)
        {
            if (device.Authenticated) return true;
            // loop through common PIN numbers to see if they pair
            foreach (var devicePin in CommonDevicePins)
            {
                var isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin);
                if (isPaired) break;
            }

            device.Update();
            return device.Authenticated;
        }

        public static void AutoPairDevices()
        {
            // get a list of all paired devices
            var paired = LocalClient.DiscoverDevices(255, false, true, false, false);
            // check every discovered device if it is already paired 
            foreach (var device in DeviceList)
            {
                var isPaired = paired.Any(t => device.Equals(t));

                // if the device is not paired, try to pair it
                if (!isPaired)
                {
                    // loop through common PIN numbers to see if they pair
                    foreach (var devicePin in CommonDevicePins)
                    {
                        isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin);
                        if (isPaired) break;
                    }
                }
            }
        }


        public static void ConnectDevice(BluetoothDeviceInfo device, string devicePin)
        {
            // set pin of device to connect with
            if (devicePin != null) LocalClient.SetPin(devicePin);

            // check if device is paired
            if (device.Authenticated)
            {
                // synchronous connection method
                LocalClient.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                if (LocalClient.Connected)
                {
                    _stream = LocalClient.GetStream();
                    _stream.ReadTimeout = 500;
                }                
            }            
        }

        public void ConnectDevice(int deviceId)
        {

            var device = DeviceList[deviceId];
            ConnectDevice(device, null);
        }

        public static List<SerialPort> GetSerialPorts()
        {
            var portIdentifiers = new[]
                {
                    "bthenum","btmodem","btport"
                };
            var portList = new List<SerialPort>();
            const string win32SerialPort = "Win32_SerialPort";
            var query = new SelectQuery(win32SerialPort);
            var managementObjectSearcher = new ManagementObjectSearcher(query);
            var portslist = managementObjectSearcher.Get();
            foreach (var port in portslist)
            {
                var managementObject = (ManagementObject) port;
                var deviceId = managementObject.GetPropertyValue("DeviceID").ToString();
                var pnpDeviceId = managementObject.GetPropertyValue("PNPDeviceID").ToString().ToLower();

                if (portIdentifiers.Any(pnpDeviceId.Contains))
                {
                    portList.Add(new SerialPort { Port = deviceId, DeviceId = pnpDeviceId });
                }
            } 
            return portList; 
        }

        public static void PrintSerialPorts()
        {
            var portList = GetSerialPorts();
            foreach (var port in portList)
            {                
                Console.WriteLine("Port: {0}, name: {1}", port.Port, port.DeviceId );
            }
        }

        public static void PrintLocalAddress()
        {
            var localBluetoothAddress = LocalAddress();

            if (localBluetoothAddress == null) return;
            Console.WriteLine("{0:C}", localBluetoothAddress);
        }

        public static void PrintDevice(BluetoothDeviceInfo device)
        {
            // log and save all found devices

            Console.Write(device.DeviceName + " (" + device.DeviceAddress + "): Device is ");
            Console.Write(device.Remembered ? "remembered" : "not remembered");
            Console.Write(device.Authenticated ? ", paired" : ", not paired");
            Console.WriteLine(device.Connected ? ", connected" : ", not connected");
        }

        private static void PrintDevices()
        {
            // log and save all found devices
            foreach (var t in DeviceList)
            {
                PrintDevice(t);
            }
        }

    }
}
