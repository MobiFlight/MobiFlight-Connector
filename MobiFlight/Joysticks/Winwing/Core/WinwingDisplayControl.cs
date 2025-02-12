using MobiFlightWwFcu;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Winwing
{
    public class WinwingDisplayControl
    {
        private int ProductId = 0xBB10;

        private WinwingMessageSender MessageSender = null;     
        private List<IWinwingDevice> WinwingCoupledDevices = new List<IWinwingDevice>();

        private Dictionary<string, IWinwingDevice> LedNameToDeviceMapping;
        private Dictionary<string, List<IWinwingDevice>> DisplayNameToDeviceMapping;

        private Thread HeartbeatThread = null;
        private volatile bool DoExecuteHeartbeat = false;

        public event EventHandler<string> ErrorMessageCreated;

        private WebSocketServer Server;
        private string WebSocketPath = string.Empty;


        public WinwingDisplayControl(int productId, WebSocketServer server)
        {           
            Init(productId, server);
        }

        private void AddToCoupledDevices(IWinwingDevice device)
        {
            WinwingCoupledDevices.Add(device);
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

        private void ErrorMessageHandler(string message)
        {
            ErrorMessageCreated?.Invoke(this, message);
        }
        private void AddCduDevice(string path, WinwingCduType type)
        {            
            WebSocketServiceHost host;
            var device = new WinwingCduDevice(MessageSender, type);
            if (!Server.WebSocketServices.TryGetServiceHost(path, out host))
            {
                Server.AddWebSocketService<WinwingCduWebsocketBehavior>(path, s => 
                { 
                    s.Device = device;
                    s.ErrorMessageHandler = this.ErrorMessageHandler;
                });
                WebSocketPath = path;
            }
            AddToCoupledDevices(device);
        }

        private void Init(int productId, WebSocketServer server)
        {
            Server = server;
            ProductId = productId;
            LedNameToDeviceMapping = new Dictionary<string, IWinwingDevice>();
            DisplayNameToDeviceMapping = new Dictionary<string, List<IWinwingDevice>>();
            MessageSender = new WinwingMessageSender(ProductId);             

            switch (ProductId)
            {
                case WinwingConstants.PRODUCT_ID_FCU_ONLY:
                    AddToCoupledDevices(new WinwingFcuDevice(MessageSender));
                    break;
                case WinwingConstants.PRODUCT_ID_FCU_EFISL:
                    AddToCoupledDevices(new WinwingFcuDevice(MessageSender));
                    AddToCoupledDevices(new WinwingEfisDevice(MessageSender, WinwingConstants.EFISL_NAME));
                    break;
                case WinwingConstants.PRODUCT_ID_FCU_EFISR:
                    AddToCoupledDevices(new WinwingFcuDevice(MessageSender));
                    AddToCoupledDevices(new WinwingEfisDevice(MessageSender, WinwingConstants.EFISR_NAME));
                    break;
                case WinwingConstants.PRODUCT_ID_FCU_EFISL_EFISR:
                    AddToCoupledDevices(new WinwingFcuDevice(MessageSender));
                    AddToCoupledDevices(new WinwingEfisDevice(MessageSender, WinwingConstants.EFISL_NAME));
                    AddToCoupledDevices(new WinwingEfisDevice(MessageSender, WinwingConstants.EFISR_NAME));
                    break;
                case WinwingConstants.PRODUCT_ID_MCDU_CPT:
                    AddCduDevice("/winwing/cdu-captain", WinwingCduType.MCDU);                    
                    break;
                case WinwingConstants.PRODUCT_ID_MCDU_FO:
                    AddCduDevice("/winwing/cdu-co-pilot", WinwingCduType.MCDU);
                    break;
                case WinwingConstants.PRODUCT_ID_MCDU_OBS:
                    AddCduDevice("/winwing/cdu-observer", WinwingCduType.MCDU);
                    break;
                case WinwingConstants.PRODUCT_ID_PFP3N_CPT:
                    AddCduDevice("/winwing/cdu-captain", WinwingCduType.PFP3N);
                    break;
                case WinwingConstants.PRODUCT_ID_PFP3N_FO:
                    AddCduDevice( "/winwing/cdu-co-pilot", WinwingCduType.PFP3N);
                    break;
                case WinwingConstants.PRODUCT_ID_PFP3N_OBS:
                    AddCduDevice("/winwing/cdu-observer", WinwingCduType.PFP3N);
                    break;     
                default:
                    break;
            }        
        }


        public void Connect()
        {
            MessageSender.Connect();
            foreach (var device in WinwingCoupledDevices) 
            {
                device.Connect();
            }           
            StartHeartbeat();

            // Start websocket server if necessary and not already running
            if (!Server.IsListening && Server.WebSocketServices.Count > 0)
            {
                Server.Start();
            }
        }

        public void Shutdown()
        {
            try
            {  
                if (!string.IsNullOrEmpty(WebSocketPath))
                {
                    Server.RemoveWebSocketService(WebSocketPath);
                }
                if (MessageSender.IsConnected())
                {
                    StopHeartbeat();
                    foreach (var device in WinwingCoupledDevices)
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
            foreach (var device in WinwingCoupledDevices)
            {
                ledNames.AddRange(device.GetLedNames());
            }            
            return ledNames;
        }

        public List<string> GetDisplayNames()
        {
            var displayDict = new Dictionary<string, string>();
            foreach (var device in WinwingCoupledDevices)
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
