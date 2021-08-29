using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CommandMessenger.Transport.Serial
{
    /// <summary>
    /// Utility methods for serial communication handling.
    /// </summary>
    public static class SerialUtils
    {
        private static readonly bool IsMonoRuntime = (Type.GetType("Mono.Runtime") != null);

        /// <summary>
        /// Commonly used baud rates.
        /// </summary>
        public static int[] CommonBaudRates
        {
            get
            {
                return new []
                {
                    115200, // Arduino Uno, Mega, with AT8u2 USB
                    57600,  // Arduino Duemilanove, FTDI Serial
                    9600    // Often used as default, but slow!
                };
            }
        }

        /// <summary> Queries if a given port exists. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public static bool PortExists(string serialPortName)
        {
            if (IsMonoRuntime)
            {
                return File.Exists(serialPortName);
            }
            else
            {
                return SerialPort.GetPortNames().Contains(serialPortName);
            }
        }

        /// <summary>
        /// Retrieve available serial ports.
        /// </summary>
        /// <returns>Array of serial port names.</returns>
        public static string[] GetPortNames()
        {
            /**
             * Under Mono SerialPort.GetPortNames() returns /dev/ttyS* devices,
             * but Arduino is detected as ttyACM* or ttyUSB*
             * */
            if (IsMonoRuntime)
            {
                var searchPattern = new Regex("ttyACM.+|ttyUSB.+");
                return Directory.GetFiles("/dev").Where(f => searchPattern.IsMatch(f)).ToArray();
            }
            else
            {
                return SerialPort.GetPortNames();
            }
        }

        /// <summary> 
        /// Retrieves the possible baud rates for the provided serial port. Windows ONLY.
        /// </summary>
        /// <returns>List of supported baud rates.</returns>
        public static int[] GetSupportedBaudRates(string serialPortName)
        {
            try
            {
                var serialPort = new SerialPort(serialPortName);
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    var fieldInfo = serialPort.BaseStream.GetType()
                        .GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fieldInfo != null)
                    {
                        object p = fieldInfo.GetValue(serialPort.BaseStream);
                        var fieldInfoValue = p.GetType()
                            .GetField("dwSettableBaud",
                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (fieldInfoValue != null)
                        {
                            var dwSettableBaud = (Int32)fieldInfoValue.GetValue(p);
                            serialPort.Close();

                            return BaudRateMaskToActualRates(dwSettableBaud).ToArray();
                        }
                    }
                }
            }
            catch
            {
                // Ignore.
            }

            // Can't determine possible baud rates, will use all possible values
            return BaudRateMaskToActualRates(int.MaxValue).ToArray();
        }

        /// <summary>
        /// Get the range of possible baud rates for serial port.
        /// </summary>
        /// <param name="possibleBaudRates">dwSettableBaud parameter from the COMMPROP Structure</param>
        /// <returns>List of bad rates</returns>
        private static List<int> BaudRateMaskToActualRates(int possibleBaudRates)
        {
            #pragma warning disable 219
            // ReSharper disable InconsistentNaming
            //const int BAUD_075  = 0x00000001;
            //const int BAUD_110  = 0x00000002;
            //const int BAUD_150  = 0x00000008;
            const int BAUD_300    = 0x00000010;
            const int BAUD_600    = 0x00000020;
            const int BAUD_1200   = 0x00000040;
            const int BAUD_1800   = 0x00000080;
            const int BAUD_2400   = 0x00000100;
            const int BAUD_4800   = 0x00000200;
            const int BAUD_7200   = 0x00000400;
            const int BAUD_9600   = 0x00000800;
            const int BAUD_14400  = 0x00001000;
            const int BAUD_19200  = 0x00002000;
            const int BAUD_38400  = 0x00004000;
            const int BAUD_56K    = 0x00008000;
            const int BAUD_57600  = 0x00040000;
            const int BAUD_115200 = 0x00020000;
            const int BAUD_128K   = 0x00010000;
            #pragma warning restore 219

            var baudRateCollection = new List<int>();

            // We start with the most common baudrates:
            if ((possibleBaudRates & BAUD_115200) > 0)
                baudRateCollection.Add(115200);        // Maxspeed Arduino Uno, Mega, with AT8u2 USB
            if ((possibleBaudRates & BAUD_9600) > 0)
                baudRateCollection.Add(9600);          // Often default speed 
            if ((possibleBaudRates & BAUD_57600) > 0)
                baudRateCollection.Add(57600);         // Maxspeed Arduino Duemilanove, FTDI Serial

            // After that going from fastest to slowest baudrates:
            if ((possibleBaudRates & BAUD_128K) > 0)
                baudRateCollection.Add(128000);
            if ((possibleBaudRates & BAUD_56K) > 0)
                baudRateCollection.Add(56000);
            if ((possibleBaudRates & BAUD_38400) > 0)
                baudRateCollection.Add(38400);
            if ((possibleBaudRates & BAUD_19200) > 0)
                baudRateCollection.Add(19200);
            if ((possibleBaudRates & BAUD_14400) > 0)
                baudRateCollection.Add(14400);
            if ((possibleBaudRates & BAUD_7200) > 0)
                baudRateCollection.Add(7200);
            if ((possibleBaudRates & BAUD_4800) > 0)
                baudRateCollection.Add(4800);
            if ((possibleBaudRates & BAUD_2400) > 0)
                baudRateCollection.Add(2400);
            if ((possibleBaudRates & BAUD_1800) > 0)
                baudRateCollection.Add(1800);
            if ((possibleBaudRates & BAUD_1200) > 0)
                baudRateCollection.Add(1200);
            if ((possibleBaudRates & BAUD_600) > 0)
                baudRateCollection.Add(600);
            if ((possibleBaudRates & BAUD_300) > 0)
                baudRateCollection.Add(300);

            // Skip old and slow rates.
            /*if ((possibleBaudRates & BAUD_150) > 0)
                baudRateCollection.Add(150);
            if ((possibleBaudRates & BAUD_110) > 0)
                baudRateCollection.Add(110);
            if ((possibleBaudRates & BAUD_075) > 0)
                baudRateCollection.Add(75);*/

            return baudRateCollection;
        }
    }
}
