using MobiFlight.Modifier;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Frontend
{
    public class InputConfigItemAdapter : IConfigItem
    {
        private InputConfigItem item;
        public InputConfigItemAdapter(InputConfigItem item)
        {
            this.item = item;
        }

        public static InputConfigItem FromConfigItem(ConfigItem item)
        {
            var result = new InputConfigItem();
            result.GUID = item.GUID;
            result.Active = item.Active;
            result.Description = item.Description;
            result.ModuleSerial = item.Device;
            result.Type = item.Type;

            item.Context.Preconditions.ForEach(p=> result.Preconditions.Add(p));
            item.Context.ConfigRefs.ForEach(c => result.ConfigRefs.Add(c));
            item.Modifiers.ToList().ForEach(m => result.Modifiers.Items.Add(m));
            return result;
        }

        private string DetermineComponent()
        {
            return item.Name;
        }

        public string GUID { get => item.GUID; set => item.GUID = value; }
        public bool Active { get => item.Active; set => item.Active = value; }
        public string Description { get => item.Description; set => item.Description = value; }
        public string Device { get => item.ModuleSerial; set => throw new System.NotImplementedException(); }
        public string Component { get => DetermineComponent(); set => throw new System.NotImplementedException(); }
        public string Type { get => item.Type; set => throw new System.NotImplementedException(); }
        public string[] Tags { get => new List<string>().ToArray(); set => throw new System.NotImplementedException(); }
        public string[] Status { get => new List<string>().ToArray(); set => throw new System.NotImplementedException(); }
        public string RawValue { get => "0"; set => throw new System.NotImplementedException(); }
        public string ModifiedValue { get => "0"; set => throw new System.NotImplementedException(); }

        public ConfigContext Context
        {
            get
            {
                return new ConfigContext()
                {
                    Preconditions = item.Preconditions.Items,
                    ConfigRefs = item.ConfigRefs.Items
                };
            }
            set => throw new System.NotImplementedException();
        }

        public ModifierBase[] Modifiers { get => item.Modifiers.Items.ToArray(); set => throw new System.NotImplementedException(); }
        public ConfigEvent Event
        {
            get
            {
                return ConfigEvent.Create(this.item);
            }
            set => throw new System.NotImplementedException();
        }
        public ConfigAction Action { get; set; }
    }
}
