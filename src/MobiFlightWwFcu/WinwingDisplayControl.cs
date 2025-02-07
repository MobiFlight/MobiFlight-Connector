using MobiFlightWwFcu;
using System;
using System.Collections.Generic;
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

            switch (ProductId)
            {
                case WinwingConstants.PRODUCT_ID_FCU_ONLY:
                    AddDevice(new WinwingFcu(MessageSender));
                    break;
                case WinwingConstants.PRODUCT_ID_FCU_EFISL:
                    AddDevice(new WinwingFcu(MessageSender));
                    AddDevice(new WinwingEfis(MessageSender, WinwingConstants.EFISL_NAME));
                    break;
                case WinwingConstants.PRODUCT_ID_FCU_EFISR:
                    AddDevice(new WinwingFcu(MessageSender));
                    AddDevice(new WinwingEfis(MessageSender, WinwingConstants.EFISR_NAME));
                    break;
                case WinwingConstants.PRODUCT_ID_FCU_EFISL_EFISR:
                    AddDevice(new WinwingFcu(MessageSender));
                    AddDevice(new WinwingEfis(MessageSender, WinwingConstants.EFISL_NAME));
                    AddDevice(new WinwingEfis(MessageSender, WinwingConstants.EFISR_NAME));
                    break;
                case WinwingConstants.PRODUCT_ID_MCDU_CPT:
                case WinwingConstants.PRODUCT_ID_MCDU_FO:
                case WinwingConstants.PRODUCT_ID_MCDU_OBS:
                    AddDevice(new WinwingCduDevice(MessageSender, WinwingCduType.MCDU));
                    break;
                case WinwingConstants.PRODUCT_ID_PFP3N_CPT:
                case WinwingConstants.PRODUCT_ID_PFP3N_FO:
                case WinwingConstants.PRODUCT_ID_PFP3N_OBS:
                    AddDevice(new WinwingCduDevice(MessageSender, WinwingCduType.PFP3N));
                    break;
                default:
                    break;
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
                Thread thread = new Thread(ExecuteHeartbeat)
                {
                    IsBackground = true
                };
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
            try
            {
                while (true)
                {
                    if (DoExecuteHeartbeat)
                    {                        
                        MessageSender.SendHeartBeatMessage();
                        Thread.Sleep(450);
                        MessageSender.SendHeartBeatMessage();
                    }
                    Thread.Sleep(2550);
                }
            }
            catch (Exception ex) 
            {                
                ErrorMessageCreated?.Invoke(this, $"Error on Winwing FCU Heartbeat. Exception: {ex.Message}");
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
            var ledNames = new List<string>();
            foreach (var device in WinwingDevices)
            {
                ledNames.AddRange(device.GetLedNames());
            }            
            return ledNames;
        }

        public List<string> GetDisplayNames()
        {
            var displayDict = new Dictionary<string, string>();
            foreach (var device in WinwingDevices)
            {
                foreach (var name in device.GetDisplayNames())
                {
                    if (!displayDict.ContainsKey(name))
                    {
                        displayDict.Add(name, name);
                    }
                }
            }            
            return displayDict.Keys.ToList();
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
                catch (Exception ex) 
                {
                    ErrorMessageCreated?.Invoke(this, $"Error setting WinWing display name='{name}' to value='{value}'. Probably value not in a valid number format.");
                }
            }
        }
    }
}
