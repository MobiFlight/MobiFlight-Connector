using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using MobiFlight.UI;
using System.Configuration;
using System.Globalization;

namespace MobiFlight
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{57699317-1D72-4B54-82BC-CF6B38254550}");
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                // if (Environment.OSVersion.Version.Major >= 6)
                //      SetProcessDPIAware();

                CheckForCorruptSettings();

                // this is needed for correct conversion
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(true);
                Application.Run(new MainForm());
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }

        // The .NET Settings system can result in corrupted settings files if the PC crashes while the app is running.
        // Unfortunately this is a not uncommon occurrence when using Microsoft Flight Simulator 2020. It's hard to
        // catch the exception from the underlying settings system so instead the common solution is to test the
        // validity of the settings file before ever referencing it in code and then deleting if the file is
        // corrupted. This causes the settings system to automatically load the defaults in the case of a corrupted
        // file. This solution comes from https://stackoverflow.com/a/18905791.
        // 
        // Returns true if a corrupt config was found.
        private static void CheckForCorruptSettings()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationErrorsException ex)
            {
                string filename = string.Empty;
                if (!string.IsNullOrEmpty(ex.Filename))
                {
                    filename = ex.Filename;
                }
                else
                {
                    var innerEx = ex.InnerException as ConfigurationErrorsException;
                    if (innerEx != null && !string.IsNullOrEmpty(innerEx.Filename))
                    {
                        filename = innerEx.Filename;
                    }
                }

                if (!string.IsNullOrEmpty(filename))
                {
                    if (System.IO.File.Exists(filename))
                    {
                        var fileInfo = new System.IO.FileInfo(filename);
                        var watcher
                             = new System.IO.FileSystemWatcher(fileInfo.Directory.FullName, fileInfo.Name);
                        System.IO.File.Delete(filename);
                        if (System.IO.File.Exists(filename))
                        {
                            watcher.WaitForChanged(System.IO.WatcherChangeTypes.Deleted);
                        }
                    }
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
