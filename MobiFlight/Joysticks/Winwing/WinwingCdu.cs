using MobiFlight.Config;
using MobiFlightWwFcu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingCdu : Joystick
    {
        private WinwingDisplayControl DisplayControl;

        private List<IBaseDevice> LcdDevices = new List<IBaseDevice>();
        private List<ListItem<IBaseDevice>> LedDevices = new List<ListItem<IBaseDevice>>();

        private const int COLUMNS = 24;
        private const int ROWS = 14;
        private const int CELLS = COLUMNS * ROWS;

        public WinwingCdu(SharpDX.DirectInput.Joystick joystick, JoystickDefinition def, int productId, WebSocketServer server) : base(joystick, def)
        {
            Log.Instance.log($"WinwingCdu - New WinwingCdu ProductId={productId.ToString("X")}", LogSeverity.Debug);
            DisplayControl = new WinwingDisplayControl(productId, server);
            var displayNames = DisplayControl.GetDisplayNames();
            var ledNames = DisplayControl.GetLedNames();

            DisplayControl.ErrorMessageCreated += DisplayControl_ErrorMessageCreated;

            // Initialize LCD and LED device lists and current value cache
            foreach (string displayName in displayNames)
            {
                LcdDevices.Add(new LcdDisplay() { Name = displayName }); // Col and Lines values don't matter   
            }
            foreach (string ledName in ledNames)
            {
                LedDevices.Add(new JoystickOutputDevice() { Label = ledName, Name = ledName }.ToListItem()); // Byte and Bit values don't matter           
            }
        }

        private void DisplayControl_ErrorMessageCreated(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                Log.Instance.log(e, LogSeverity.Error);
            }
        }


        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            DisplayControl.Connect();
        }


        public override IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {
            // Output for the led indicators, LcdDisplay to control brightness
            return new List<DeviceType>() { DeviceType.Output, DeviceType.LcdDisplay };
        }

        public override void SetLcdDisplay(string address, string value)
        {
            // Check for value change is done in display control
            DisplayControl.SetDisplay(address, value);
        }

        public override void SetOutputDeviceState(string name, byte state)
        {
            // Check for value change is done in display control
            DisplayControl.SetLed(name, state);
        }

        public override List<IBaseDevice> GetAvailableLcdDevices()
        {
            return LcdDevices;
        }

        public override List<ListItem<IBaseDevice>> GetAvailableOutputDevicesAsListItems()
        {
            return LedDevices;
        }

        public override void UpdateOutputDeviceStates()
        {
            // do nothing, update is event based not polled
        }

        protected override void SendData(byte[] data)
        {
            // do nothing, data is directly send in SetOutputDeviceState
        }

        public override void Stop()
        {
            DisplayControl.Stop();
        }

        public override void Shutdown()
        {
            DisplayControl.Shutdown();
        }
        

        /// <summary>
        /// Displays a formatted user message on the screen based on the specified message code and parameters.
        /// </summary>
        /// <remarks>The method formats the message by substituting the provided parameters into the
        /// template associated with the given <paramref name="messageCode"/>. The resulting message is split into lines
        /// to fit a predefined maximum line length and is displayed on the screen.</remarks>
        /// <param name="messageCode">The code representing the message to display. Must correspond to a valid entry in the <see
        /// cref="UserMessageCodes.CodeToMessageMap"/>.</param>
        /// <param name="parameters">An array of strings containing the parameters to format the message. These are substituted into the message
        /// template associated with the <paramref name="messageCode"/>.</param>
        public override void ShowUserMessage(int messageCode, params string[] parameters)
        {
            try
            {
                // Grab the message
                string message = string.Format(UserMessageCodes.CodeToMessageMap[messageCode], parameters).ToUpper();

                // Create a list of word tokens
                string[] tokens = message.Split(' ');

                List<string> lines = new List<string>();

                // Add first token in separate line
                lines.Add(tokens[0]);
                lines.Add(string.Empty); // empty line
                var currentLine = new StringBuilder();

                for (int i = 1; i < tokens.Length; i++)
                {
                    string token = tokens[i];
                    token = token.Length > COLUMNS ? token.Substring(0, COLUMNS) : token;

                    // Does not fit
                    if (currentLine.Length + token.Length + 1 > COLUMNS)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                    }

                    // Append
                    token = currentLine.Length == 0 ? token : $" {token}";
                    currentLine.Append(token);

                    // Last token
                    if (i == tokens.Length - 1)
                    {
                        lines.Add(currentLine.ToString());
                    }
                }
                SendToDisplay(lines);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"WinwingCdu - Error on show user message: {ex.Message}", LogSeverity.Error);
            }
        }

        
        /// <summary>
        /// Sends a list of text lines to the display, formatting them to fit within the display's dimensions.  
        /// </summary>
        /// <remarks>This method formats the provided text lines to fit within the display's fixed
        /// dimensions, ensuring proper alignment and padding. If the number of lines exceeds the display's capacity,
        /// only the first 14 lines are used. Empty lines are added to center the text vertically, and each line is
        /// padded to align horizontally. The formatted data is then converted to a JSON structure and sent to the
        /// display.</remarks>
        /// <param name="lines">A collection of strings representing the lines of text to be displayed. Each line is centered horizontally
        /// and the entire set of lines is centered vertically within the display.</param>
        private void SendToDisplay(IList<string> lines)
        {
            // Center vertically          
            int emptyLineCount = (ROWS - Math.Min(lines.Count, ROWS)) / 2;
            var linesToDisplay = Enumerable.Repeat(string.Empty, emptyLineCount).Concat(lines).ToList();          

            var cduData = new CduChar[CELLS]; 
            int currentIndex = 0;

            // Fill CduChar array
            for (int i = 0; i < linesToDisplay.Count; i++)
            {
                string currentLine = linesToDisplay[i];
                int padLeft = (COLUMNS - currentLine.Length) / 2 + currentLine.Length;
                string paddedLine = currentLine.PadLeft(padLeft).PadRight(COLUMNS); ;                

                for (int j = 0; j < paddedLine.Length; j++)
                {
                    if (currentIndex < cduData.Length)
                    {
                        cduData[currentIndex] = new CduChar
                        {
                            Character = paddedLine[j]
                        };                        
                    }
                    currentIndex++;
                }
            }
            // Convert CduChar array to json.
            var builder = new StringBuilder("{ \"Target\": \"Display\", \"Data\": [");
            foreach (var cduChar in cduData)
            {
                if (cduChar == null || cduChar.Character == ' ')
                {
                    builder.Append("[],");
                }
                else
                {
                    builder.Append($"[\"{cduChar.Character}\", \"{cduChar.Color}\", {cduChar.IsSmall}],");
                }
            }
            builder.Remove(builder.Length - 1, 1); // Remove last comma
            builder.Append("] }");
            string data = builder.ToString();
            SetLcdDisplay(WinwingConstants.CDU_DATA, data);
        }        
    }
}
