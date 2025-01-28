using MobiFlight.BrowserMessages.Incoming.Converter;
using MobiFlight.Modifier;
using Newtonsoft.Json;

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
        }
    }
}
