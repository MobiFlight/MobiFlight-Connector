using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace MobiFlight
{
    static class MobiFlightFirmwareUpdater
    {
        /***
         * "D:\portableapps\arduino-1.0.5\hardware\tools\avr\bin\avrdude"
         */
        public static String ArduinoIdePath { get; set; }
        public static String AvrPath { get { return "hardware\\tools\\avr"; } }
        public static String LatestFirmwareMega = "1.4.1";
        public static String LatestFirmwareMicro = "1.0.0";
        
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
            String Port = module.InitUploadAndReturnUploadPort();
            if (module.Connected) module.Disconnect();

            while (!SerialPort.GetPortNames().Contains(Port))
            {
                System.Threading.Thread.Sleep(100);
            }

            RunAvrDude(Port, module.ArduinoType);
            
            return true;
        }

        /*
        public static String GetLatestFirmwareFile(String ArduinoType)
        {
            String prefix = "mobiflight_micro_";
            if (MobiFlightModuleInfo.TYPE_ARDUINO_MEGA == ArduinoType)
            {
                prefix = "mobiflight_mega_";
            }
            string[] filePaths = Directory.GetFiles(@FirmwarePath, prefix + "*.hex");

            String result = null;
            foreach (string file in filePaths)
            {
            }

            if (result == null) throw new FileNotFoundException("Could not find any firmware in " + FirmwarePath);
            return result;
        }
        */

        public static void RunAvrDude(String Port, String ArduinoType) 
        {
            String FirmwareName = "mobiflight_micro_" + LatestFirmwareMicro.Replace('.','_') + ".hex";
            String ArduinoChip = "atmega32u4";
            String Bytes = "57600";
            String C = "avr109";
            if (MobiFlightModuleInfo.TYPE_ARDUINO_MEGA == ArduinoType) {
                FirmwareName = "mobiflight_mega_" + LatestFirmwareMega.Replace('.', '_') + ".hex";
                ArduinoChip = "atmega2560";
                Bytes = "115200";
                C = "wiring";
            }

            if (!IsValidFirmwareFilepath(FirmwarePath + "\\" + FirmwareName))
            {
                String message = "Firmware not found: " + FirmwarePath + "\\" + FirmwareName;
                Log.Instance.log(message, LogSeverity.Error);
                throw new FileNotFoundException(message);
            }

            String verboseLevel = "";
            //if (false) verboseLevel = " -v -v -v -v";

            String FullAvrDudePath = ArduinoIdePath + "\\" + AvrPath;

            var proc1 = new ProcessStartInfo();
            string anyCommand = "-C\"" + FullAvrDudePath + "\\etc\\avrdude.conf\"" + verboseLevel + " -p" + ArduinoChip + " -c"+ C +" -P\\\\.\\" + Port + " -b"+ Bytes +" -D -Uflash:w:\"" + FirmwarePath + "\\" + FirmwareName + "\":i";
            proc1.UseShellExecute = true;
            proc1.WorkingDirectory = "\"" + FullAvrDudePath + "\"";
            proc1.FileName = "\"" + FullAvrDudePath + "\\bin\\avrdude" + "\"";
            //proc1.Verb = "runas";
            proc1.Arguments = anyCommand;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Log.Instance.log("RunAvrDude : " + proc1.FileName, LogSeverity.Info);
            Log.Instance.log("RunAvrDude : " + anyCommand, LogSeverity.Info);
            Process p = Process.Start(proc1);
            p.WaitForExit();
            Log.Instance.log("Exit Code: " + p.ExitCode, LogSeverity.Info);
            if (p.ExitCode != 0)
            {
                String message = "Something went wrong when flashing with command \n" + proc1.FileName + " " + anyCommand;
                Log.Instance.log(message, LogSeverity.Error);
                throw new Exception(message);
            }
        }
    }
}
