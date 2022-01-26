using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using LibUsbDotNet.LibUsb;

namespace MobiFlight
{
    static class MobiFlightFirmwareUpdater
    {
        /***
         * "D:\portableapps\arduino-1.0.5\hardware\tools\avr\bin\avrdude"
         */
        public static String ArduinoIdePath { get; set; }
        public static String AvrPath { get { return "hardware\\tools\\avr"; } }

        /***
         * C:\\Users\\SEBAST~1\\AppData\\Local\\Temp\\build2060068306446321513.tmp\\cmd_test_mega.cpp.hex
         **/
        public static String FirmwarePath { get; set; }

        public static bool IsValidArduinoIdePath(string path)
        {
            return Directory.Exists(path + "\\" + AvrPath);
        }

        public static bool IsValidFirmwareFilepath(string filepath)
        {
            return File.Exists(filepath);
        }

        // This method would, in theory, return the bus number and address for the supplied module.
        // Unforutnately I can't find any way to do that.

        public static Tuple<int, int> GetBusNumberAndAddress(MobiFlightModule module)
        {
            // The below code illustrates that it is possible to loop through all connected
            // usb devices, but I can't figure out how to map the devices found in this search returned in this search
            // to the one supplied via the module parameter to the function. That makes it impossible to know
            // what bus number and address corresponds to the connected device that needs flashing.
            using (var context = new UsbContext())
            {
                using (var allDevices = context.List())
                {
                    foreach (UsbDevice deviceInfo in allDevices)
                    {
                        // At this point for a given device the following properties are available:
                        // * VendorId (a.k.a. VID)
                        // * ProductId (a.k.a. PID)
                        // * BusNumber (the bus number required to connect to a Pico)
                        // * Address (the address required to connect to a Pico)
                        // * PortNumber (not the same as Address, I think it identifies which port on the USB hub the device is connected to)
                        //
                        // The following properties are NOT available as far as I can tell:
                        // * Friendly name
                        // * COM port
                        // * Identifier of the hub it's plugged into
                        // * LocationInformation
                        Console.WriteLine($"VendorId: {String.Format("0x{0:X4}", deviceInfo.VendorId)} ProductId: {String.Format("0x{0:X4}", deviceInfo.ProductId)} Bus number: {deviceInfo.BusNumber} Address: {deviceInfo.Address}");
                    }
                }
            }

            // Hack in a return value. Bus number 2 and address 12 is what my Pico happens to be at the moment.
            return Tuple.Create(2, 12);
        }

        public static bool Update(MobiFlightModule module)
        {
            bool result = false;
            String Port = module.InitUploadAndReturnUploadPort();
            if (module.Connected) module.Disconnect();

            if (module.Board.AvrDudeSettings != null)
            {
                while (!SerialPort.GetPortNames().Contains(Port))
                {
                    System.Threading.Thread.Sleep(100);
                }

                try
                {
                    RunAvrDude(Port, module.Board);
                    result = true;
                } catch(Exception e) {
                    result = false;
                }

                if (module.Board.Connection.DelayAfterFirmwareUpdate > 0)
                {
                    System.Threading.Thread.Sleep(module.Board.Connection.DelayAfterFirmwareUpdate);
                }
            }
            else if (module.Board.PicoToolSettings != null)
            {
                System.Threading.Thread.Sleep(module.Board.Connection.DelayBeforeFirmwareUpdate); // wait for Bootloader to be ready for RaspiPico

                try
                {
                    var busAndAddress = GetBusNumberAndAddress(module);
                    RunPicoTool(module.Board, busAndAddress.Item1, busAndAddress.Item2);
                    RebootPico(module.Board, busAndAddress.Item1, busAndAddress.Item2);
                    result = true;
                }
                catch (Exception e)
                {
                    result = false;
                }
            }
            else
            {
                Log.Instance.log($"Firmware update requested for {module.Board.Info.MobiFlightType} ({module.Port}) however no update settings were specified in the board definition file. Module update skipped.", LogSeverity.Warn);
            }
            return result;
        }

        public static void RunPicoTool(Board board, int busNumber, int address)
        {
            var FirmwareName = board.PicoToolSettings.GetFirmwareName(board.Info.LatestFirmwareVersion);
            string message;

            if (!IsValidFirmwareFilepath($"{FirmwarePath}\\{FirmwareName}"))
            {
                message = "Firmware not found: " + FirmwarePath + "\\" + FirmwareName;
                Log.Instance.log(message, LogSeverity.Error);
                throw new FileNotFoundException(message);
            }

            var FullRaspiPicoUpdatePath = $"{Directory.GetCurrentDirectory()}\\RaspberryPi\\tools";
            var proc1 = new ProcessStartInfo();

            var updateCommand = $"load {FirmwarePath}\\{FirmwareName} --bus {busNumber} --address {address}";

            proc1.WorkingDirectory = FullRaspiPicoUpdatePath;
            proc1.FileName = $"\"{FullRaspiPicoUpdatePath}\\picotool.exe\"";

            proc1.Arguments = updateCommand;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;

            Log.Instance.log($"RunRaspberryPicoUpdater : {proc1.FileName} {updateCommand}", LogSeverity.Debug);

            Process p = Process.Start(proc1);
            if (p.WaitForExit(board.PicoToolSettings.Timeout))
            {
                Log.Instance.log($"Firmware Upload Exit Code: {p.ExitCode}", LogSeverity.Info);
                // everything OK
                if (p.ExitCode == 0) return;

                // process terminated but with an error.
                message = $"ExitCode: {p.ExitCode} => Something went wrong when flashing with command \n {proc1.FileName} {updateCommand}";
            }
            else
            {
                // we timed out;
                p.Kill();
                message = $"picotool timed out! Something went wrong when flashing with command \n {proc1.FileName} {updateCommand}";
            }

            Log.Instance.log(message, LogSeverity.Error);
            throw new Exception(message);
        }

        public static void RebootPico(Board board, int busNumber, int address)
        {
            string message;
            var FullRaspiPicoUpdatePath = $"{Directory.GetCurrentDirectory()}\\RaspberryPi\\tools";
            var proc1 = new ProcessStartInfo();

            proc1.WorkingDirectory = FullRaspiPicoUpdatePath;
            proc1.FileName = $"\"{FullRaspiPicoUpdatePath}\\picotool.exe\"";
            proc1.Arguments = "reboot";
            proc1.WindowStyle = ProcessWindowStyle.Hidden;

            Log.Instance.log($"RunRaspberryPicoUpdater : {proc1.FileName} reboot --bus {busNumber} --address {address}", LogSeverity.Debug);

            Process p = Process.Start(proc1);
            if (p.WaitForExit(board.PicoToolSettings.Timeout))
            {
                Log.Instance.log($"PicoTool reboot exit code: {p.ExitCode}", LogSeverity.Info);
                // everything OK
                if (p.ExitCode == 0) return;

                // process terminated but with an error.
                message = $"ExitCode: {p.ExitCode} => Something went wrong when rebooting the Pico with command \n {proc1.FileName} reboot";
            }
            else
            {
                // we timed out;
                p.Kill();
                message = $"picotool timed out! Something went wrong when rebooting the Pico with command \n {proc1.FileName} reboot";
            }

            Log.Instance.log(message, LogSeverity.Error);
            throw new Exception(message);
        }

        public static void RunAvrDude(String Port, Board board) 
        {
            String FirmwareName = board.AvrDudeSettings.GetFirmwareName(board.Info.LatestFirmwareVersion); 
            String ArduinoChip = board.AvrDudeSettings.Device;
            String Bytes = board.AvrDudeSettings.BaudRate;
            String C = board.AvrDudeSettings.Programmer;
            String message = "";

            if (!IsValidFirmwareFilepath(FirmwarePath + "\\" + FirmwareName))
            {
                message = "Firmware not found: " + FirmwarePath + "\\" + FirmwareName;
                Log.Instance.log(message, LogSeverity.Error);
                throw new FileNotFoundException(message);
            }

            String verboseLevel = "";
            
            //verboseLevel = " -v -v -v -v";

            String FullAvrDudePath = $@"{ArduinoIdePath}\{AvrPath}";

            var proc1 = new ProcessStartInfo();
            string anyCommand = 
                $@"-C""{FullAvrDudePath}\etc\avrdude.conf"" {verboseLevel} -p{ArduinoChip} -c{C} -P\\.\{Port} -b{Bytes} -D -Uflash:w:""{FirmwarePath}\{FirmwareName}"":i";
            proc1.UseShellExecute = true;
            proc1.WorkingDirectory = $@"""{FullAvrDudePath}""";
            proc1.FileName = $@"""{FullAvrDudePath}\bin\avrdude""";
            proc1.Arguments = anyCommand;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Log.Instance.log("RunAvrDude : " + proc1.FileName, LogSeverity.Debug);
            Log.Instance.log("RunAvrDude : " + anyCommand, LogSeverity.Debug);
            Process p = Process.Start(proc1);
            if (p.WaitForExit(board.AvrDudeSettings.Timeout))
            {
                Log.Instance.log("Firmware Upload Exit Code: " + p.ExitCode, LogSeverity.Info);
                // everything OK
                if (p.ExitCode == 0) return;
                
                // process terminated but with an error.
                message = $"ExitCode: {p.ExitCode} => Something went wrong when flashing with command \n {proc1.FileName} {anyCommand}";
            } else
            {
                // we timed out;
                p.Kill();
                message = $"avrdude timed out! Something went wrong when flashing with command \n {proc1.FileName} {anyCommand}";
            }

            Log.Instance.log(message, LogSeverity.Error);
            throw new Exception(message);
        }
    }
}
