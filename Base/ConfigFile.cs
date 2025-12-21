using MobiFlight.InputConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MobiFlight.Base
{
    public class ConfigFile : IConfigFile
    {
        public string Label { get; set; }
        public string FileName { get; set; }
        public bool ReferenceOnly { get; set; } = false;
        public bool EmbedContent { get; set; } = false;
        public List<IConfigItem> ConfigItems { get; set; } = new List<IConfigItem>();

        public ConfigFile() { }

        public ConfigFile(string FileName)
        {
            this.FileName = FileName;
        }

        public void OpenFile()
        {
            if (EmbedContent)
            {
                // Content is embedded, no need to load from file
                return;
            }

            var json = File.ReadAllText(FileName);
            var configFile = JsonConvert.DeserializeObject<ConfigFile>(json);
            FileName = configFile.FileName ?? Path.GetFileName(FileName);
            ReferenceOnly = configFile.ReferenceOnly;
            EmbedContent = configFile.EmbedContent;
            ConfigItems = configFile.ConfigItems;
        }

        public void SaveFile()
        {
            if (EmbedContent || ReferenceOnly)
            {
                // Content is embedded or read-only, no need to save to file
                return;
            }
            var json = ToJson();
            File.WriteAllText(FileName, json);
        }

        public void Merge(IConfigFile other)
        {
            ConfigFileUtils.MergeConfigItems(this, other);
        }

        public string ToJson()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ConfigFile)) return false;
            var other = obj as ConfigFile;

            return
                FileName.AreEqual(other.FileName) &&
                Label.AreEqual(other.Label) &&
                ReferenceOnly == other.ReferenceOnly &&
                EmbedContent == other.EmbedContent &&
                ConfigItems.SequenceEqual(other.ConfigItems)
                ;
        }

        public bool HasDuplicateGuids()
        {
            var guids = new HashSet<string>();
            foreach (var item in ConfigItems)
            {
                if (!guids.Add(item.GUID))
                {
                    return true; // Duplicate GUID found
                }
            }
            return false; // No duplicates
        }

        public void RemoveDuplicateGuids()
        {
            var guids = new HashSet<string>();
            foreach (var item in ConfigItems)
            {
                while (!guids.Add(item.GUID))
                {
                    item.GUID = System.Guid.NewGuid().ToString();
                }
            }
        }

        internal string DetermineSim()
        {
            if (ContainsConfigOfSourceType(new SimConnectSource()))
            {
                return "msfs";
            }
            else if (ContainsConfigOfSourceType(new XplaneSource()))
            {
                return (new XplaneSource().SourceType).ToLower();
            }
            else if (ContainsConfigOfSourceType(new ProSimSource()))
            {
                return (new ProSimSource()).SourceType.ToLower();
            }
            return null;
        }

        internal List<string> DetermineAircaft()
        {
            return new List<string>();
        }

        internal bool DetermineUsingFsuipc()
        {
            return ContainsConfigOfSourceType(new FsuipcSource());
        }

        public bool ContainsConfigOfSourceType(Source type)
        {
            return ContainsConfigOfSourceType(ConfigItems, type);
        }

        public static bool ContainsConfigOfSourceType(List<IConfigItem> configItems, Source type)
        {
            var result = false;
            if (type is SimConnectSource)
            {
                result = configItems
                        .Any(x => x is OutputConfigItem && (x as OutputConfigItem)?.Source is SimConnectSource) ||
                        configItems
                        .Any(x => x is InputConfigItem && (x as InputConfigItem)?.GetInputActionsByType(typeof(MSFS2020CustomInputAction)).Count > 0);
            }
            else if (type is FsuipcSource)
            {
                result = configItems
                        .Any(x => x is OutputConfigItem && (x as OutputConfigItem)?.Source is FsuipcSource) ||
                         configItems
                        .Any(x => x is InputConfigItem &&
                                  (
                                  (x as InputConfigItem)?.GetInputActionsByType(typeof(FsuipcOffsetInputAction)).Count > 0 ||
                                  (x as InputConfigItem)?.GetInputActionsByType(typeof(EventIdInputAction)).Count > 0 ||
                                  (x as InputConfigItem)?.GetInputActionsByType(typeof(PmdgEventIdInputAction)).Count > 0 ||
                                  (x as InputConfigItem)?.GetInputActionsByType(typeof(JeehellInputAction)).Count > 0 ||
                                  (x as InputConfigItem)?.GetInputActionsByType(typeof(LuaMacroInputAction)).Count > 0
                                  )
                                  );
            }
            else if (type is XplaneSource)
            {
                result = configItems
                        .Any(x => x is OutputConfigItem && (x as OutputConfigItem)?.Source is XplaneSource) ||
                         configItems
                        .Any(x => x is InputConfigItem && (x as InputConfigItem)?.GetInputActionsByType(typeof(XplaneInputAction)).Count > 0);
            }
            else if (type is VariableSource)
            {
                result = configItems
                        .Any(x => x is OutputConfigItem && (x as OutputConfigItem)?.Source is VariableSource) ||
                         configItems
                        .Any(x => x is InputConfigItem && (x as InputConfigItem)?.GetInputActionsByType(typeof(VariableInputAction)).Count > 0);
            }
            else if (type is ProSimSource)
            {
                result = configItems
                        .Any(x => x is OutputConfigItem && (x as OutputConfigItem)?.Source is ProSimSource) ||
                         configItems
                        .Any(x => x is InputConfigItem && (x as InputConfigItem)?.GetInputActionsByType(typeof(ProSimInputAction)).Count > 0);
            }
            return result;
        }

        public List<String> GetIUniqueControllerSerials()
        {
            return ConfigItems.Select((i) => i.ModuleSerial).Distinct().ToList();
        }
    }
}