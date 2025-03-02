using System.IO;

namespace MobiFlight.Base
{
    public static class ConfigFileFactory
    {
        public static IConfigFile CreateConfigFile(string filePath)
        {
            if (IsJson(filePath))
            {
                return new ConfigFile { FileName = filePath };
            }
            else if (IsXml(filePath))
            {
                return new DeprecatedConfigFile { FileName = filePath };
            }
            else
            {
                throw new InvalidDataException("Unsupported file format.");
            }
        }

        private static bool IsJson(string filePath)
        {
            var firstChar = File.ReadAllText(filePath).TrimStart()[0];
            return firstChar == '{' || firstChar == '[';
        }

        private static bool IsXml(string filePath)
        {
            var firstFewChars = File.ReadAllText(filePath).TrimStart().Substring(0, 5);
            return firstFewChars.StartsWith("<?xml");
        }
    }
}

