using MobiFlight.Modifier;

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
            GUID = item.GUID;
            Active = item.Active;
            Name = item.Name;
            ModuleSerial = item.ModuleSerial;
            Preconditions = item.Preconditions;
            Modifiers = item.Modifiers;
            ConfigRefs = item.ConfigRefs;
            RawValue = item.RawValue;
            Value = item.Value;
        }
    }
}
