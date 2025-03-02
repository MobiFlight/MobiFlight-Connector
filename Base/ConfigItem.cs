using MobiFlight.Base.Serialization.Json;
using MobiFlight.Modifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Base
{
    [JsonConverter(typeof(ConfigItemConverter))]
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

        object Clone();

        IConfigItem Duplicate();
    }

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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RawValue { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual IDeviceConfig Device { get { return GetDeviceConfig(); } set { new NotImplementedException(); } }

        public Dictionary<ConfigItemStatusType, string> Status { get; set; } = new Dictionary<ConfigItemStatusType, string>();

        protected abstract IDeviceConfig GetDeviceConfig();

        protected virtual string GetConfigItemType()
        {
            return this.GetType().Name;
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
            RawValue = item.RawValue?.Clone() as string;
            Value = item.Value?.Clone() as string;
            Status = new Dictionary<ConfigItemStatusType, string>(item.Status);
        }

        public virtual object Clone()
        {
            return Clone() as ConfigItem;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ConfigItem item)) return false;

            return GUID == item.GUID &&
                   Active == item.Active &&
                   Name == item.Name &&
                   ModuleSerial == item.ModuleSerial &&
                   Preconditions.Equals(item.Preconditions) &&
                   Modifiers.Equals(item.Modifiers) &&
                   ConfigRefs.Equals(item.ConfigRefs) &&
                   RawValue == item.RawValue &&
                   Value == item.Value &&
                   Status.SequenceEqual(item.Status);
        }
    }
}
