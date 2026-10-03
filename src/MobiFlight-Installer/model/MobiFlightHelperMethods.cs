using System.IO;
using System.Linq;

namespace MobiFlightInstaller
{
    static public class MobiFlightHelperMethods
    {
        public const string ProcessName = "MFConnector";

        private static string[] FilePathCandidates = new string[2] {
            Path.Combine(Directory.GetCurrentDirectory(), $"{ProcessName}.dll"),
            Path.Combine(Directory.GetCurrentDirectory(), $"{ProcessName}.exe"),
        };

        public static string GetMobiFlightApplicationPath() => FilePathCandidates.FirstOrDefault(File.Exists);
    }
}
