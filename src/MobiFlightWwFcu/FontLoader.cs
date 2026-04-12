using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class FontLoader
    {
        private Dictionary<string, string> AvailableMcduFonts = new Dictionary<string, string>();
        private Dictionary<string, string> AvailablePfpFonts = new Dictionary<string, string>();
        private Dictionary<string, string> CurrentlyLoadedFontForDevices = new Dictionary<string, string>();

        private const string DefaultMcduFolder = @"Scripts\Winwing\Fonts\Default\MCDU\";
        private const string DefaultPfpFolder = @"Scripts\Winwing\Fonts\Default\PFP\";

        public FontLoader()
        {        
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(baseDirectory))
            {
                var filesMcduFullPath = Directory.GetFiles(Path.Combine(baseDirectory, DefaultMcduFolder), "*.dat").ToList();
                var filesPfpFullPath = Directory.GetFiles(Path.Combine(baseDirectory, DefaultPfpFolder), "*.dat").ToList();
                
                filesMcduFullPath.ForEach(f => AvailableMcduFonts.Add(Path.GetFileNameWithoutExtension(f), f));
                filesPfpFullPath.ForEach(f => AvailablePfpFonts.Add(Path.GetFileNameWithoutExtension(f), f));
            }
        }

        // { "Target": "Font",
        //   "Data": "Airbus" }

        public void LoadFont(IWinwingDevice device, string fontNameJson)
        {
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(fontNameJson);
            string fontName = jsonObject["Data"].Value<string>();
            
            var availableFonts = new Dictionary<string, string>();
            if (device.Name.ToUpper().Contains("MCDU"))
            {
                availableFonts = AvailableMcduFonts;
            }
            else if (device.Name.ToUpper().Contains("PFP"))
            {
                availableFonts = AvailablePfpFonts;
            }

            if (availableFonts.ContainsKey(fontName))
            {
                string loadedFont = string.Empty;
                bool isSuccess = CurrentlyLoadedFontForDevices.TryGetValue(device.Name, out loadedFont);
                if (!isSuccess || (loadedFont != fontName))
                {
                    string fontData = File.ReadAllText(availableFonts[fontName]);
                    device.SetDisplay(WinwingConstants.FONT_DATA, fontData);
                    CurrentlyLoadedFontForDevices[device.Name] = fontName;
                }
            }
        }
    }
}
