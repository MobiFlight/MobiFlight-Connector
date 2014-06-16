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
        /***
         * C:\\Users\\SEBAST~1\\AppData\\Local\\Temp\\build2060068306446321513.tmp\\cmd_test_mega.cpp.hex
         **/
        public static String FirmwarePath { get; set; }

        public static bool IsValidArduinoIdePath(string path)
        {
            return Directory.Exists(path + "\\" + AvrPath);
        }

        public static bool Update(MobiFlightModule module)
        {
            bool result = true;
            String Port = module.InitUploadAndReturnUploadPort();

            while (!SerialPort.GetPortNames().Contains(Port))
            {
                System.Threading.Thread.Sleep(100);
            }
            RunAvrDude(Port, module.ArduinoType);
            //module.Connect();    
            return result;
        }

        public static void RunAvrDude(String Port, String ArduinoType) 
        {


            String FirmwareName = "mobiflight_micro_1_0_0.hex";
            String ArduinoChip = "atmega32u4";
            String Bytes = "57600";
            String C = "avr109";
            if (MobiFlightModuleInfo.TYPE_ARDUINO_MEGA == ArduinoType) {
                FirmwareName = "mobiflight_mega_1_0_0.hex";
                ArduinoChip = "atmega2560";
                Bytes = "115200";
                C = "wiring";
            }

            String FullAvrDudePath = ArduinoIdePath + "\\" + AvrPath;

            var proc1 = new ProcessStartInfo();
            string anyCommand = "-C" + FullAvrDudePath + "\\etc\\avrdude.conf -v -v -v -v -p" + ArduinoChip + " -c"+ C +" -P\\\\.\\" + Port + " -b"+ Bytes +" -D -Uflash:w:" + FirmwarePath + "\\" + FirmwareName + ":i";
            proc1.UseShellExecute = true;
            proc1.WorkingDirectory = @FullAvrDudePath;
            proc1.FileName = @FullAvrDudePath + "\\bin\\avrdude";
            proc1.Verb = "runas";
            proc1.Arguments = anyCommand;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            // Log.Instance.log("RunAvrDude : " + anyCommand, LogSeverity.Info);
            Process p = Process.Start(proc1);
            p.WaitForExit();
        }
    }
}
