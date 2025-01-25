using MobiFlight.Modifier;

namespace MobiFlight.Base
{
    public interface IConfigItem
    {
        string GUID { get; set; }
        bool Active { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        PreconditionList Preconditions { get; set; }
        ModifierList Modifiers { get; set; }
        ConfigRefList ConfigRefs { get; set; }
        string RawValue { get; set; }
        string Value { get; set; }
    }

    public abstract class ConfigItem : IConfigItem
    {
        public string GUID { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public PreconditionList Preconditions { get; set; }
        public ModifierList Modifiers { get; set; }
        public ConfigRefList ConfigRefs { get; set; }
        public string RawValue { get; set; }
        public string Value { get; set; }
    }
}
