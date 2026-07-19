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
        private Dictionary<string, string> CurrentlyLoadedFontForControllers = new Dictionary<string, string>();

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

        public void LoadFont(IWinCtrlController controller, string fontNameJson)
        {
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(fontNameJson);
            string fontName = jsonObject["Data"].Value<string>();
            
            var availableFonts = new Dictionary<string, string>();
            if (controller.Name.ToUpper().Contains("MCDU"))
            {
                availableFonts = AvailableMcduFonts;
            }
            else if (controller.Name.ToUpper().Contains("PFP"))
            {
                availableFonts = AvailablePfpFonts;
            }

            if (availableFonts.ContainsKey(fontName))
            {
                string loadedFont = string.Empty;
                bool isSuccess = CurrentlyLoadedFontForControllers.TryGetValue(controller.Name, out loadedFont);
                if (!isSuccess || (loadedFont != fontName))
                {
                    string fontData = File.ReadAllText(availableFonts[fontName]);
                    controller.SetDisplay(WinCtrlConstants.FONT_DATA, fontData);
                    CurrentlyLoadedFontForControllers[controller.Name] = fontName;
                }
            }
        }
    }
}
