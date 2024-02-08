using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace MobiFlight.Frontend
{
    public class InputConfigItemAdapter : IConfigItem
    {
        private InputConfigItem item;
        public InputConfigItemAdapter(InputConfigItem item) { 
            this.item = item;
        }

        private string DetermineComponent()
        {
            return item.Name;
        }

        public bool Active { get => item.Active; set => item.Active = value; }
        public string Description { get => item.Description; set => item.Description = value; }
        public string Device { get => item.ModuleSerial; set => throw new System.NotImplementedException(); }
        public string Component { get => DetermineComponent(); set => throw new System.NotImplementedException(); }
        public string Type { get => item.Type; set => throw new System.NotImplementedException(); }
        public string[] Tags { get => new List<string>().ToArray(); set => throw new System.NotImplementedException(); }
        public string[] Status { get => new List<string>().ToArray(); set => throw new System.NotImplementedException(); }
        public string RawValue { get => "0"; set => throw new System.NotImplementedException(); }
        public string ModifiedValue { get => "0"; set => throw new System.NotImplementedException(); }
    }
}
