using Newtonsoft.Json;
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

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }

        public void Merge(IConfigFile other)
        {
            ConfigFileUtils.MergeConfigItems(this, other);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ConfigFile)) return false;
            var other = obj as ConfigFile;

            return
                FileName.AreEqual(other.FileName) &&
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
    }
}