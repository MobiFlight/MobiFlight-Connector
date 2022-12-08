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
            string FirmwareName;

            if (module.Board.AvrDudeSettings != null)
            {
                FirmwareName = module.Board.AvrDudeSettings.GetFirmwareName(module.Board.Info.LatestFirmwareVersion);
            }
            else if (module.Board.UsbDriveSettings != null)
            {
                FirmwareName = module.Board.UsbDriveSettings.GetFirmwareName(module.Board.Info.LatestFirmwareVersion);
            }
            else
            {
                Log.Instance.log($"Firmware update requested for {module.Board.Info.MobiFlightType} ({module.Port}) however no update settings were specified in the board definition file. Module update skipped.", LogSeverity.Error);
                return false;
            }

            return UpdateFirmware(module, FirmwareName);
        }

        public static bool Reset(MobiFlightModule module)
        {
            if (module.Board.AvrDudeSettings == null)
            {
                Log.Instance.log($"Firmware reset requested for {module.Board.Info.MobiFlightType} ({module.Port}) however no reset settings were specified in the board definition file. Module reset skipped.", LogSeverity.Error);
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

            try
            {
                if (module.Board.AvrDudeSettings != null)
                {
                    while (!SerialPort.GetPortNames().Contains(Port))
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    RunAvrDude(Port, module.Board, FirmwareName);
                }
                else if (module.Board.UsbDriveSettings != null)
                {
                    FlashViaUsbDrive(module.Board);
                }
                else
                {
                    Log.Instance.log($"Firmware update requested for {module.Board.Info.MobiFlightType} ({module.Port}) however no update settings were specified in the board definition file. Module update skipped.", LogSeverity.Warn);
                }

                if (module.Board.Connection.DelayAfterFirmwareUpdate > 0)
                {
                    System.Threading.Thread.Sleep(module.Board.Connection.DelayAfterFirmwareUpdate);
                }

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static void RunAvrDude(String Port, Board board, String FirmwareName) 
        {
            String message = "";

            if (!IsValidFirmwareFilepath(FirmwarePath + "\\" + FirmwareName))
            {
                message = $"Firmware not found: {FirmwarePath}\\{FirmwareName}.";
                Log.Instance.log(message, LogSeverity.Error);
                throw new FileNotFoundException(message);
            }

            String verboseLevel = "";
            
            //verboseLevel = " -v -v -v -v";

            String FullAvrDudePath = $@"{ArduinoIdePath}\{AvrPath}";

            foreach (var baudRate in board.AvrDudeSettings.BaudRates)
            {
                var proc1 = new ProcessStartInfo();
                string anyCommand =
                    $@"-C""{FullAvrDudePath}\etc\avrdude.conf"" {verboseLevel} -x attempts={board.AvrDudeSettings.Attempts} -p{board.AvrDudeSettings.Device} -c{board.AvrDudeSettings.Programmer} -P{Port} -b{baudRate} -D -Uflash:w:""{FirmwarePath}\{FirmwareName}"":i";
                proc1.UseShellExecute = true;
                proc1.WorkingDirectory = $@"""{FullAvrDudePath}""";
                proc1.FileName = $@"""{FullAvrDudePath}\bin\avrdude""";
                proc1.Arguments = anyCommand;
                proc1.WindowStyle = ProcessWindowStyle.Hidden;
                Log.Instance.log($"{proc1.FileName} {anyCommand}", LogSeverity.Debug);
                Process p = Process.Start(proc1);
                if (p.WaitForExit(board.AvrDudeSettings.Timeout))
                {
                    Log.Instance.log($"Firmware upload exit code: {p.ExitCode}.", LogSeverity.Debug);
                    // everything OK
                    if (p.ExitCode == 0) return;

                    // process terminated but with an error.
                    message = $"ExitCode: {p.ExitCode} => Something went wrong when flashing with command \n {proc1.FileName} {anyCommand}.";
                }
                else
                {
                    // we timed out;
                    p.Kill();
                    message = $"avrdude timed out! Something went wrong when flashing with command \n {proc1.FileName} {anyCommand}.";
                }
                Log.Instance.log(message, LogSeverity.Error);
            }

            throw new Exception(message);
        }

        public static void FlashViaUsbDrive(Board board)
        {
            String FirmwareName = board.UsbDriveSettings.GetFirmwareName(board.Info.LatestFirmwareVersion);
            String FullFirmwarePath = $"{FirmwarePath}\\{FirmwareName}";
            String message = "";

            if (!IsValidFirmwareFilepath(FullFirmwarePath))
            {
                message = $"Firmware not found: {FullFirmwarePath}";
                Log.Instance.log(message, LogSeverity.Error);
                throw new FileNotFoundException(message);
            }

            // Find all drives connected to the PC with a volume label that matches the one used to identify the 
            // drive that's the device to flash. This assumes the first matching drive is the one we want,
            // since it is extremely unlikely that more than one flashable USB drive will be connected and in a
            // flashable state at the same time.
            DriveInfo drive;

            try
            {
                drive = DriveInfo.GetDrives().Where(d => d.VolumeLabel == board.UsbDriveSettings.VolumeLabel).First();
            }
            catch
            {
                message = $"No mounted USB drives named {board.UsbDriveSettings.VolumeLabel} found";
                Log.Instance.log(message, LogSeverity.Error);
                throw new FileNotFoundException(message);
            }

            // Look for the presence of a file on the drive to confirm it is really a drive that supports flashing via
            // file copy.
            try
            {
                var verificationFile = drive.RootDirectory.GetFiles(board.UsbDriveSettings.VerificationFileName).First();
            }
            catch
            {
                message = $"A mounted USB drive named {board.UsbDriveSettings.VolumeLabel} was found but verification file {board.UsbDriveSettings.VerificationFileName} was not found.";
                Log.Instance.log(message, LogSeverity.Error);
                throw new FileNotFoundException(message);
            }

            // At this point the drive is valid so all that's left is to copy the firmware over. If the file
            // copy succeeds the connected device will automatically reboot itself so there's no need to
            // attempt any kind of reconnect after either. Nice!
            var destination = $"{drive.RootDirectory.FullName}{FirmwareName}";
            try
            {
                Log.Instance.log($"Copying {FullFirmwarePath} to {destination}", LogSeverity.Debug);
                File.Copy(FullFirmwarePath, destination);
            }
            catch (Exception e)
            {
                message = $"Unable to copy {FullFirmwarePath} to {destination}: {e.Message}";
                Log.Instance.log(message, LogSeverity.Error);
                throw new Exception(message);
            }
        }
    }
}
