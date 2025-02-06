using MobiFlight.BrowserMessages.Incoming.Converter;
using MobiFlight.Modifier;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Base
{
    public interface IConfigItem
    {
        string GUID { get; set; }
        bool Active { get; set; }
        string Type { get; }
        string Name { get; set; }
        string ModuleSerial { get; set; }
        PreconditionList Preconditions { get; set; }
        ModifierList Modifiers { get; set; }
        ConfigRefList ConfigRefs { get; set; }
        string RawValue { get; set; }
        string Value { get; set; }
        IDeviceConfig Device { get; }
        Dictionary<ConfigItemStatusType, string> Status { get; set; }
        bool Equals(object item);
    }

    [JsonConverter(typeof(ConfigItemConverter))]
    public abstract class ConfigItem : IConfigItem
    {
        public string GUID { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Type { get { return GetConfigItemType(); } }
        public string ModuleSerial { get; set; }
        public PreconditionList Preconditions { get; set; } = new PreconditionList();
        public ModifierList Modifiers { get; set; } = new ModifierList();
        public ConfigRefList ConfigRefs { get; set; } = new ConfigRefList();
        public string RawValue { get; set; }
        public string Value { get; set; }
        
        public IDeviceConfig Device { get { return GetDeviceConfig(); } }

        public Dictionary<ConfigItemStatusType, string> Status { get; set; } = new Dictionary<ConfigItemStatusType, string>();

        protected abstract IDeviceConfig GetDeviceConfig();

        protected virtual string GetConfigItemType()
        {
            return this.GetType().ToString();
        }

        public abstract IConfigItem Duplicate();

        protected ConfigItem()
        {
            GUID = System.Guid.NewGuid().ToString();
            Active = true;
            Name = "";
            ModuleSerial = "";
            Preconditions = new PreconditionList();
            Modifiers = new ModifierList();
            ConfigRefs = new ConfigRefList();
            RawValue = "";
            Value = "";
            Status = new Dictionary<ConfigItemStatusType, string>();
        }

        protected ConfigItem(ConfigItem item)
        {
            GUID = item.GUID.Clone() as string;
            Active = item.Active;
            Name = item.Name.Clone() as string;
            ModuleSerial = item.ModuleSerial.Clone() as string;
            Preconditions = item.Preconditions.Clone() as PreconditionList;
            Modifiers = item.Modifiers.Clone() as ModifierList;
            ConfigRefs = item.ConfigRefs.Clone() as ConfigRefList;
            RawValue = item.RawValue.Clone() as string;
            Value = item.Value.Clone() as string;
            Status = new Dictionary<ConfigItemStatusType, string>(item.Status);
        }

        public override bool Equals(object item)
        {
            return item != null &&
                GUID == (item as ConfigItem).GUID &&
                Active == (item as ConfigItem).Active &&
                Name == (item as ConfigItem).Name &&
                ModuleSerial == (item as ConfigItem).ModuleSerial &&
                Preconditions.Equals((item as ConfigItem).Preconditions) &&
                Modifiers.Equals((item as ConfigItem).Modifiers) &&
                ConfigRefs.Equals((item as ConfigItem).ConfigRefs) &&
                RawValue == (item as ConfigItem).RawValue &&
                Value == (item as ConfigItem).Value &&
                Status.SequenceEqual((item as ConfigItem).Status);
        }
    }
}
