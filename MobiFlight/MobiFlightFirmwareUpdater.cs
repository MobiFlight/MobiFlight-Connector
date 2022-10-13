using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

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

        public static bool Update(MobiFlightModule module)
        {
            if (module.Board.AvrDudeSettings == null)
            {
                Log.Instance.log($"Firmware update requested for {module.Board.Info.MobiFlightType} ({module.Port}) however no update settings were specified in the board definition file. Module update skipped.", LogSeverity.Warn);
                return false;
            }

            var FirmwareName = module.Board.AvrDudeSettings.GetFirmwareName(module.Board.Info.LatestFirmwareVersion);
            return UpdateFirmware(module, FirmwareName);
        }

        public static bool Reset(MobiFlightModule module)
        {
            if (module.Board.AvrDudeSettings == null)
            {
                Log.Instance.log($"Firmware reset requested for {module.Board.Info.MobiFlightType} ({module.Port}) however no reset settings were specified in the board definition file. Module reset skipped.", LogSeverity.Warn);
                return false;
            }

            var FirmwareName = module.Board.AvrDudeSettings.ResetFirmwareFile;
            return UpdateFirmware(module, FirmwareName);
        }

        public static bool UpdateFirmware(MobiFlightModule module, String FirmwareName)
        {
            bool result = false;
            String Port = module.InitUploadAndReturnUploadPort();
            if (module.Connected) module.Disconnect();

            while (!SerialPort.GetPortNames().Contains(Port))
            {
                System.Threading.Thread.Sleep(100);
            }

            if (module.Board.AvrDudeSettings != null)
            {
                try {
                    RunAvrDude(Port, module.Board, FirmwareName);
                    result = true;
                } catch(Exception e) {
                    result = false;
                }

                if (module.Board.Connection.DelayAfterFirmwareUpdate > 0)
                {
                    System.Threading.Thread.Sleep(module.Board.Connection.DelayAfterFirmwareUpdate);
                }
            } else
            {
                Log.Instance.log($"Firmware update requested for {module.Board.Info.MobiFlightType} ({module.Port}) however no update settings were specified in the board definition file. Module update skipped.", LogSeverity.Warn);
            }
            return result;
        }

        public static void RunAvrDude(String Port, Board board, String FirmwareName) 
        {
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
                $@"-C""{FullAvrDudePath}\etc\avrdude.conf"" {verboseLevel} -p{ArduinoChip} -c{C} -P{Port} -b{Bytes} -D -Uflash:w:""{FirmwarePath}\{FirmwareName}"":i";
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
