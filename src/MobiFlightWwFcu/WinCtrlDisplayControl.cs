using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WebSocketSharp.Server;

namespace MobiFlightWwFcu
{
    public class WinCtrlDisplayControl
    {
        private int ProductId = 0xBB10;

        private WinCtrlMessageSender MessageSender = null;
        private List<IWinCtrlController> CoupledControllers = new List<IWinCtrlController>();

        private Dictionary<string, IWinCtrlController> LedNameToControllerMapping;
        private Dictionary<string, List<IWinCtrlController>> DisplayNameToControllerMapping;

        private Thread HeartbeatThread = null;
        private volatile bool DoExecuteHeartbeat = false;

        public event EventHandler<string> ErrorMessageCreated;

        private WebSocketServer Server;
        private string WebSocketPath = string.Empty;

        public WinCtrlDisplayControl(int productId, WebSocketServer server)
        {
            Init(productId, server);
        }

        private void AddToCoupledControllers(IWinCtrlController controller)
        {
            CoupledControllers.Add(controller);
            foreach (var ledName in controller.GetLedNames())
            {
                LedNameToControllerMapping.Add(ledName, controller);
            }
            foreach (var displayName in controller.GetDisplayNames())
            {
                if (!DisplayNameToControllerMapping.ContainsKey(displayName))
                {
                    DisplayNameToControllerMapping.Add(displayName, new List<IWinCtrlController>() { controller });
                }
                else
                {
                    DisplayNameToControllerMapping[displayName].Add(controller);
                }
            }
            foreach (var internalDisplayName in controller.GetInternalDisplayNames())
            {
                if (!DisplayNameToControllerMapping.ContainsKey(internalDisplayName))
                {
                    DisplayNameToControllerMapping.Add(internalDisplayName, new List<IWinCtrlController>() { controller });
                }
                else
                {
                    DisplayNameToControllerMapping[internalDisplayName].Add(controller);
                }
            }
        }

        private void ErrorMessageHandler(string message)
        {
            ErrorMessageCreated?.Invoke(this, message);
        }
        // Der WebSocket-Pfad bleibt bewusst auf /winwing/..., obwohl die Firma
        // inzwischen WinCtrl heisst: er ist ein Contract mit externen Clients,
        // die sonst ins Leere verbinden wuerden.
        private void AddCduController(string path, WinCtrlCduType type)
        {
            WebSocketServiceHost host;
            var controller = new WinCtrlCduController(MessageSender, type);
            if (!Server.WebSocketServices.TryGetServiceHost(path, out host))
            {
                Server.AddWebSocketService<WinCtrlCduWebsocketBehavior>(path, s =>
                {
                    s.Controller = controller;
                    s.ErrorMessageHandler = this.ErrorMessageHandler;
                    s.Loader = new FontLoader();
                });
                WebSocketPath = path;
            }
            AddToCoupledControllers(controller);
        }

        private void Init(int productId, WebSocketServer server)
        {
            Server = server;
            ProductId = productId;
            LedNameToControllerMapping = new Dictionary<string, IWinCtrlController>();
            DisplayNameToControllerMapping = new Dictionary<string, List<IWinCtrlController>>();
            MessageSender = new WinCtrlMessageSender(ProductId);

            switch (ProductId)
            {
                case WinCtrlConstants.PRODUCT_ID_FCU_ONLY:
                    AddToCoupledControllers(new WinCtrlFcuController(MessageSender));
                    break;
                case WinCtrlConstants.PRODUCT_ID_FCU_EFISL:
                    AddToCoupledControllers(new WinCtrlFcuController(MessageSender));
                    AddToCoupledControllers(new WinCtrlEfisController(MessageSender, WinCtrlConstants.EFISL_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_FCU_EFISR:
                    AddToCoupledControllers(new WinCtrlFcuController(MessageSender));
                    AddToCoupledControllers(new WinCtrlEfisController(MessageSender, WinCtrlConstants.EFISR_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_FCU_EFISL_EFISR:
                    AddToCoupledControllers(new WinCtrlFcuController(MessageSender));
                    AddToCoupledControllers(new WinCtrlEfisController(MessageSender, WinCtrlConstants.EFISL_NAME));
                    AddToCoupledControllers(new WinCtrlEfisController(MessageSender, WinCtrlConstants.EFISR_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_PAP3:
                    AddToCoupledControllers(new WinCtrlPap3Controller(MessageSender));
                    break;
                case WinCtrlConstants.PRODUCT_ID_3NPDCL:
                    AddToCoupledControllers(new WinCtrl3PdcController(MessageSender, WinCtrlConstants.PDC3NL_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_3NPDCR:
                    AddToCoupledControllers(new WinCtrl3PdcController(MessageSender, WinCtrlConstants.PDC3NR_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_3MPDCL:
                    AddToCoupledControllers(new WinCtrl3PdcController(MessageSender, WinCtrlConstants.PDC3ML_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_3MPDCR:
                    AddToCoupledControllers(new WinCtrl3PdcController(MessageSender, WinCtrlConstants.PDC3MR_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_AIRBUS_THROTTLE_L:
                    AddToCoupledControllers(new WinCtrlAirbusThrottleController(MessageSender, WinCtrlConstants.AIRBUS_THROTTLE_L_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_AIRBUS_THROTTLE_R:
                    AddToCoupledControllers(new WinCtrlAirbusThrottleController(MessageSender, WinCtrlConstants.AIRBUS_THROTTLE_R_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_AIRBUS_STICK_L:
                    AddToCoupledControllers(new WinCtrlAirbusSidestickController(MessageSender, WinCtrlConstants.AIRBUS_STICK_L_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_AIRBUS_STICK_R:
                    AddToCoupledControllers(new WinCtrlAirbusSidestickController(MessageSender, WinCtrlConstants.AIRBUS_STICK_R_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_ECAM:
                    AddToCoupledControllers(new WinCtrlEcamController(MessageSender));
                    break;
                case WinCtrlConstants.PRODUCT_ID_AGP:
                    AddToCoupledControllers(new WinCtrlAgpController(MessageSender));
                    break;
                case WinCtrlConstants.PRODUCT_ID_TCAS:
                    AddToCoupledControllers(new WinCtrlTcasController(MessageSender));
                    break;
                case WinCtrlConstants.PRODUCT_ID_PTO2:
                    AddToCoupledControllers(new WinCtrlPto2Controller(MessageSender));
                    break;
                case WinCtrlConstants.PRODUCT_ID_RMP_L:
                    AddToCoupledControllers(new WinCtrlRmpController(MessageSender, WinCtrlConstants.RMP_L_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_RMP_R:
                    AddToCoupledControllers(new WinCtrlRmpController(MessageSender, WinCtrlConstants.RMP_R_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_RMP_C:
                    AddToCoupledControllers(new WinCtrlRmpController(MessageSender, WinCtrlConstants.RMP_C_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_NWS_L:
                    AddToCoupledControllers(new WinCtrlNwsController(MessageSender, WinCtrlConstants.NWS_L_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_NWS_R:
                    AddToCoupledControllers(new WinCtrlNwsController(MessageSender, WinCtrlConstants.NWS_R_NAME));
                    break;
                case WinCtrlConstants.PRODUCT_ID_MCDU_CPT:
                    AddCduController("/winwing/cdu-captain", WinCtrlCduType.MCDU);
                    break;
                case WinCtrlConstants.PRODUCT_ID_MCDU_FO:
                    AddCduController("/winwing/cdu-co-pilot", WinCtrlCduType.MCDU);
                    break;
                case WinCtrlConstants.PRODUCT_ID_MCDU_OBS:
                    AddCduController("/winwing/cdu-observer", WinCtrlCduType.MCDU);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP3N_CPT:
                    AddCduController("/winwing/cdu-captain", WinCtrlCduType.PFP3N);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP3N_FO:
                    AddCduController("/winwing/cdu-co-pilot", WinCtrlCduType.PFP3N);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP3N_OBS:
                    AddCduController("/winwing/cdu-observer", WinCtrlCduType.PFP3N);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP7_CPT:
                    AddCduController("/winwing/cdu-captain", WinCtrlCduType.PFP7);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP7_FO:
                    AddCduController("/winwing/cdu-co-pilot", WinCtrlCduType.PFP7);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP7_OBS:
                    AddCduController("/winwing/cdu-observer", WinCtrlCduType.PFP7);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP4_CPT:
                    AddCduController("/winwing/cdu-captain", WinCtrlCduType.PFP4);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP4_FO:
                    AddCduController("/winwing/cdu-co-pilot", WinCtrlCduType.PFP4);
                    break;
                case WinCtrlConstants.PRODUCT_ID_PFP4_OBS:
                    AddCduController("/winwing/cdu-observer", WinCtrlCduType.PFP4);
                    break;
                default:
                    break;
            }
        }


        public void Connect()
        {
            MessageSender.Connect();
            foreach (var controller in CoupledControllers)
            {
                controller.Connect();
            }
            StartHeartbeat();

            // Start websocket server if necessary and not already running
            if (!Server.IsListening && Server.WebSocketServices.Count > 0)
            {
                Server.Start();
            }
        }

        public void Stop()
        {
            foreach (var controller in CoupledControllers)
            {
                controller.Stop();
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
                    foreach (var controller in CoupledControllers)
                    {
                        controller.Shutdown();
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
                ErrorMessageCreated?.Invoke(this, $"Error on WinCtrl Heartbeat. Exception: {ex.Message}");
            }
        }

        public void SetLed(string led, byte state)
        {
            try
            {
                if (!string.IsNullOrEmpty(led))
                {
                    LedNameToControllerMapping[led].SetLed(led, state);
                }
            }
            catch
            {
                ErrorMessageCreated?.Invoke(this, $"Error setting WinCtrl LED name='{led}' to value='{state}'. Please check input.");
            }
        }

        public void SendRequestFirmware()
        {
            MessageSender.SendRequestFirmwareMessage();
        }

        public List<string> GetLedNames()
        {
            var ledNames = new List<string>();
            foreach (var controller in CoupledControllers)
            {
                ledNames.AddRange(controller.GetLedNames());
            }
            return ledNames;
        }

        public List<string> GetDisplayNames()
        {
            var displayDict = new Dictionary<string, string>();
            foreach (var controller in CoupledControllers)
            {
                foreach (var name in controller.GetDisplayNames())
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
                    foreach (var controller in DisplayNameToControllerMapping[name])
                    {
                        controller.SetDisplay(name, value);
                    }
                }
                catch (Exception)
                {
                    ErrorMessageCreated?.Invoke(this, $"Error setting WinCtrl display name='{name}' to value='{value}'. Probably value not in a valid number format.");
                }
            }
        }

        public string GetControllerName()
        {
            var builder = new StringBuilder();
            foreach (var controller in CoupledControllers)
            {
                builder.Append(controller.Name + " ");
            }
            return builder.ToString();
        }
    }
}
