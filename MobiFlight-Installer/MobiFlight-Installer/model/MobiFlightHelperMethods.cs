using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlightInstaller
{
    static public class MobiFlightHelperMethods
    {
        public static readonly string ProcessName = "MFConnector";
        public static readonly string OptionBetaEnableSearch = "/configuration/userSettings/MobiFlight.Properties.Settings/setting[@name='BetaUpdates']";
        static Char[] s_Base32Char = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
            'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
            'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
            'y', 'z', '0', '1', '2', '3', '4', '5'};

        static public bool GetMfBetaOptionValue()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + ProcessName + ".exe"))
            {
                Log.Instance.log("MFConnector.exe not found, BETA disable", LogSeverity.Debug);
                return false;
            }

            string PatchConfigFile = MobiFlightHelperMethods.GetExeLocalAppDataUserConfigPath(Directory.GetCurrentDirectory() + "\\" + ProcessName + ".exe");
            Log.Instance.log("Check BETA option in " + PatchConfigFile, LogSeverity.Debug);

            if (!File.Exists(PatchConfigFile))
            {
                Log.Instance.log("Impossible to read the file does not exist -> BETA disable", LogSeverity.Debug);
                return false;
            }

            bool result = ExtractConfigBetaValueFromXML(PatchConfigFile);

            if (result)
                Log.Instance.log("BETA enable", LogSeverity.Debug);
            else
                Log.Instance.log("BETA disable", LogSeverity.Debug);

            return result;
        }

        static public bool ExtractConfigBetaValueFromXML(string PatchConfigFile)
        {
            string xmlFile = File.ReadAllText(@PatchConfigFile);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlFile);
            XmlNodeList nodeList = xmldoc.SelectNodes(OptionBetaEnableSearch);
            string Result = string.Empty;
            foreach (XmlNode node in nodeList)
            {
                Result = node.InnerText;
            }
            if (Result == "True") return true;
            else return false;
        }

        static public string GetExeLocalAppDataUserConfigPath(string fullExePath)
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var versionInfo = FileVersionInfo.GetVersionInfo(fullExePath);
            var companyName = versionInfo.CompanyName;
            var exeName = versionInfo.OriginalFilename;

            var assemblyName = AssemblyName.GetAssemblyName(fullExePath);
            var version = assemblyName.Version.ToString();

            var uri = "file:///" + fullExePath;
            uri = uri.ToUpperInvariant();

            var ms = new MemoryStream();
            var bSer = new BinaryFormatter();
            bSer.Serialize(ms, uri);
            ms.Position = 0;
            var sha1 = new SHA1CryptoServiceProvider();
            var hash = sha1.ComputeHash(ms);
            var hashstring = ToBase32StringSuitableForDirName(hash);

            var userConfigLocalAppDataPath = Path.Combine(localAppDataPath, companyName, exeName + "_Url_" + hashstring, version, "user.config");

            return userConfigLocalAppDataPath;
        }

        private static string ToBase32StringSuitableForDirName(byte[] buff)
        {
            StringBuilder sb = new StringBuilder();
            byte b0, b1, b2, b3, b4;
            int l, i;

            l = buff.Length;
            i = 0;

            // Create l chars using the last 5 bits of each byte.  
            // Consume 3 MSB bits 5 bytes at a time.

            do
            {
                b0 = (i < l) ? buff[i++] : (byte)0;
                b1 = (i < l) ? buff[i++] : (byte)0;
                b2 = (i < l) ? buff[i++] : (byte)0;
                b3 = (i < l) ? buff[i++] : (byte)0;
                b4 = (i < l) ? buff[i++] : (byte)0;

                // Consume the 5 Least significant bits of each byte
                sb.Append(s_Base32Char[b0 & 0x1F]);
                sb.Append(s_Base32Char[b1 & 0x1F]);
                sb.Append(s_Base32Char[b2 & 0x1F]);
                sb.Append(s_Base32Char[b3 & 0x1F]);
                sb.Append(s_Base32Char[b4 & 0x1F]);

                // Consume 3 MSB of b0, b1, MSB bits 6, 7 of b3, b4
                sb.Append(s_Base32Char[(
                    ((b0 & 0xE0) >> 5) |
                    ((b3 & 0x60) >> 2))]);

                sb.Append(s_Base32Char[(
                    ((b1 & 0xE0) >> 5) |
                    ((b4 & 0x60) >> 2))]);

                // Consume 3 MSB bits of b2, 1 MSB bit of b3, b4

                b2 >>= 5;

                if ((b3 & 0x80) != 0)
                    b2 |= 0x08;
                if ((b4 & 0x80) != 0)
                    b2 |= 0x10;

                sb.Append(s_Base32Char[b2]);

            } while (i < l);

            return sb.ToString();
        }
    }
}
