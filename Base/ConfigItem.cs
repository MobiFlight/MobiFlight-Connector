using MobiFlight.Base.Serialization.Json;
using MobiFlight.Modifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Base
{
    [JsonConverter(typeof(ConfigValueOnlyItemConverter))]
    public interface IConfigValueOnlyItem
    {
        string GUID { get; set; }
        string RawValue { get; set; }
        string Value { get; set; }
        Dictionary<ConfigItemStatusType, string> Status { get; set; }
    }

    public class ConfigValueOnlyItem : IConfigValueOnlyItem
    {
        public string GUID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RawValue { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
        public Dictionary<ConfigItemStatusType, string> Status { get; set; }
        public ConfigValueOnlyItem() { }
        public ConfigValueOnlyItem(IConfigValueOnlyItem item)
        {
            GUID = item.GUID.Clone() as string;
            RawValue = item.RawValue?.Clone() as string;
            Value = item.Value?.Clone() as string;
            Status = new Dictionary<ConfigItemStatusType, string>(item.Status);
        }
    }

    [JsonConverter(typeof(ConfigItemConverter))]
    public interface IConfigItem : IConfigValueOnlyItem
    {
        bool Active { get; set; }
        string Type { get; }
        string Name { get; set; }
        [Obsolete("Use Controller property instead. ModuleSerial is kept for XML backward compatibility.")]
        string ModuleSerial { get; set; }
        Controller Controller { get; set; }
        PreconditionList Preconditions { get; set; }
        ModifierList Modifiers { get; set; }
        ConfigRefList ConfigRefs { get; set; }
        IDeviceConfig Device { get; }
        object Clone();
        IConfigItem Duplicate();
    }

    public abstract class ConfigItem : IConfigItem
    {
        public string GUID { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Type { get { return GetConfigItemType(); } }
        
        private Controller _controller;
        private string _moduleSerial;

        /// <summary>
        /// Controller property for JSON serialization. This is the preferred way to access controller information.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Controller Controller 
        { 
            get 
            {
                // Lazy migrate from ModuleSerial if Controller is not set
                if (_controller == null && !string.IsNullOrEmpty(_moduleSerial))
                {
                    _controller = Base.Controller.FromModuleSerial(_moduleSerial);
                }
                return _controller;
            }
            set 
            {
                _controller = value;
                // Keep ModuleSerial in sync for backward compatibility
                if (_controller != null)
                {
                    _moduleSerial = _controller.ToModuleSerial();
                }
                else
                {
                    _moduleSerial = "";
                }
            }
        }

        /// <summary>
        /// ModuleSerial property kept for XML backward compatibility.
        /// For JSON, use Controller property instead.
        /// </summary>
        [Obsolete("Use Controller property instead. ModuleSerial is kept for XML backward compatibility.")]
        [XmlIgnore]
        [JsonIgnore]
        public string ModuleSerial 
        { 
            get 
            {
                // Return from Controller if available, otherwise from backing field
                if (_controller != null)
                {
                    return _controller.ToModuleSerial();
                }
                return _moduleSerial ?? "";
            }
            set 
            {
                _moduleSerial = value ?? "";
                // Migrate immediately to avoid repeated lazy migrations
                _controller = string.IsNullOrEmpty(_moduleSerial) 
                    ? null 
                    : Base.Controller.FromModuleSerial(_moduleSerial);
            }
        }

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
            _controller = null;
            _moduleSerial = "";
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
            _controller = item._controller?.Clone();
            _moduleSerial = item._moduleSerial?.Clone() as string;
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
                   (Controller?.Equals(item.Controller) ?? item.Controller == null) &&
                   Preconditions.Equals(item.Preconditions) &&
                   Modifiers.Equals(item.Modifiers) &&
                   ConfigRefs.Equals(item.ConfigRefs) &&
                   RawValue == item.RawValue &&
                   Value == item.Value &&
                   Status.SequenceEqual(item.Status);
        }

        #region JsonConverter ShouldSerialize methods
        /// <summary>
        /// Determines whether the <see cref="Controller"/> property should be serialized by JsonConverter.
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="Controller"/> is not null and has meaningful data;
        /// otherwise, <see langword="false"/>.</returns>
        public bool ShouldSerializeController()
        {
            return Controller != null && 
                   (!string.IsNullOrEmpty(Controller.Name) || !string.IsNullOrEmpty(Controller.Serial));
        }

        /// <summary>
        /// Determines whether the <see cref="Preconditions"/> property should be serialized by JsonConverter.
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="Preconditions"/> is not null and contains one or more elements;
        /// otherwise, <see langword="false"/>.</returns>
        public bool ShouldSerializePreconditions()
        {
            return Preconditions != null &&
                   Preconditions.Any(p => !p.IsEmpty());
        }

        /// <summary>
        /// Determines whether the <see cref="Modifiers"/> property should be serialized by JsonConverter.
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="Modifiers"/> is not null and contains one or more elements; 
        /// otherwise, <see langword="false"/>.</returns>
        public bool ShouldSerializeModifiers()
        {
            return Modifiers != null && Modifiers.Items.Count > 0;
        }

        /// <summary>
        /// Determines whether the <see cref="ConfigRefs"/> property should be serialized by JsonConverter.
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="ConfigRefs"/> is not null and contains one or more elements; 
        /// otherwise, <see langword="false"/>.</returns>
        public bool ShouldSerializeConfigRefs()
        {
            return ConfigRefs != null && ConfigRefs.Count > 0;
        }

        /// <summary>
        /// Determines whether the <see cref="Status"/> property should be serialized by JsonConverter.
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="Status"/> is not null and contains one or more elements; 
        /// otherwise, <see langword="false"/>.</returns>
        public bool ShouldSerializeStatus()
        {
            return Status != null && Status.Keys.Count > 0;
        }

        /// <summary>
        /// Determines whether the <see cref="RawValue"/> property should be serialized by JsonConverter.
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="RawValue"/> is not null or empty; otherwise, <see langword="false"/>.</returns>
        public bool ShouldSerializeRawValue()
        {
            return !string.IsNullOrEmpty(RawValue);
        }

        /// <summary>
        /// Determines whether the <see cref="Value"/> property should be serialized by JsonConverter.
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="Value"/> is not null or empty; otherwise, <see langword="false"/>.</returns>
        public bool ShouldSerializeValue()
        {
            return !string.IsNullOrEmpty(Value);
        }
        #endregion
    }
}
