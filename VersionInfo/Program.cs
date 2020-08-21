using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace VersionInfo
{
    class VersionInfo
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 1) ShowUsage();

            string target = args[0];

            string path = Path.IsPathRooted(target)
                                ? target
                                : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Path.DirectorySeparatorChar + target;

            if (Assembly.LoadFile(path).GetName().Version.Revision>0)
                Console.Write(Assembly.LoadFile(path).GetName().Version.ToString(4));
            else
                Console.Write(Assembly.LoadFile(path).GetName().Version.ToString(3));
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage: versioninfo.exe <target>");
        }
    }
}