using MobiFlightWwFcu;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace MobiFlight
{
    public class WinwingDisplayControl
    {
        private int ProductId = 0xBB10;

        private WinwingMessageSender MessageSender = null;     
        private List<IWinwingDevice> WinwingDevices = new List<IWinwingDevice>();

        private Dictionary<string, IWinwingDevice> LedNameToDeviceMapping;
        private Dictionary<string, List<IWinwingDevice>> DisplayNameToDeviceMapping;

        private Thread HeartbeatThread = null;
        private volatile bool DoExecuteHeartbeat = false;

        public event EventHandler<string> ErrorMessageCreated;

              
        public WinwingDisplayControl()
        {
            Init();
        }

        public WinwingDisplayControl(int productId)
        {
            ProductId = productId;
            Init();
        }

        private void AddDevice(IWinwingDevice device)
        {
            WinwingDevices.Add(device);
            foreach (var ledName in device.GetLedNames())
            {
                LedNameToDeviceMapping.Add(ledName, device);
            }
            foreach (var displayName in device.GetDisplayNames())
            {
                if (!DisplayNameToDeviceMapping.ContainsKey(displayName))
                {
                    DisplayNameToDeviceMapping.Add(displayName, new List<IWinwingDevice>() { device });
                }
                else
                {
                    DisplayNameToDeviceMapping[displayName].Add(device);
                }
            }
        }

        private void Init()
        {
            LedNameToDeviceMapping = new Dictionary<string, IWinwingDevice>();
            DisplayNameToDeviceMapping = new Dictionary<string, List<IWinwingDevice>>();

            MessageSender = new WinwingMessageSender(ProductId);
            AddDevice(new WinwingFcu(MessageSender));

            if (ProductId == WinwingConstants.PRODUCT_ID_FCU_EFISL)
            {
                AddDevice(new WinwingEfis(MessageSender, "Left"));
            }
        }


        public void Connect()
        {
            MessageSender.Connect();
            foreach (var device in WinwingDevices) 
            {
                device.Connect();
            }           
            StartHeartbeat();            
        }

        public void Shutdown()
        {
            try
            {
                if (MessageSender.IsConnected())
                {
                    StopHeartbeat();
                    foreach (var device in WinwingDevices)
                    {
                        device.Shutdown();
                    }                   
                    MessageSender.Shutdown();
                }
            }
            catch
            {
                // do nothing if issue on shutdown
            }
        }

        private void StartHeartbeat()
        {
            if (HeartbeatThread == null)
            {
                Thread thread = new Thread(ExecuteHeartbeat);
                thread.IsBackground = true;
                thread.Start();
            }
            DoExecuteHeartbeat = true;                  
        }

        private void StopHeartbeat()
        {
            DoExecuteHeartbeat = false;
        }


        private void ExecuteHeartbeat()
        {
            while (true)
            {
                if (DoExecuteHeartbeat)
                {
                    // Do the pattern like in recording
                    MessageSender.SendHeartBeatMessage();                  
                    Thread.Sleep(450);
                    MessageSender.SendHeartBeatMessage();
                }
                Thread.Sleep(2550);
            }
        }
     
        public void SetLed(string led, byte state)
        {
            try
            {
                if (!string.IsNullOrEmpty(led))
                {
                    LedNameToDeviceMapping[led].SetLed(led, state);
                }
            }
            catch
            {
                ErrorMessageCreated?.Invoke(this, $"Error setting Winwing FCU LED name='{led}' to value='{state}'. Please check input.");
            }
        }
              
        public void SendRequestFirmware()
        {
            MessageSender.SendRequestFirmwareMessage();           
        }

        public List<string> GetLedNames()
        {
            List<string> ledNames = new List<string>();
            foreach (var device in WinwingDevices)
            {
                ledNames.AddRange(device.GetLedNames());
            }            
            return ledNames;
        }

        public List<string> GetDisplayNames()
        {
            List<string> displayNames = new List<string>(); 
            foreach (var device in WinwingDevices)
            {
                displayNames.AddRange(device.GetDisplayNames());
            }            
            return displayNames;
        }

        public void SetDisplay(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    foreach (var device in DisplayNameToDeviceMapping[name])
                    {
                        device.SetDisplay(name, value);
                    }                
                }
                catch
                {
                    ErrorMessageCreated?.Invoke(this, $"Error setting Winwing FCU display name='{name}' to value='{value}'. Probably value not in a valid number format.");
                }
            }
        }
    }
}
