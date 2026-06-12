namespace MobiFlight.Base
{
    public class DeviceReference
    {
        public virtual string Name { get; set; }

        // temporay property, will be removed before merging with the main branch
        public string Label { get; set; }
        public DeviceType Type { get; set; }

        public DeviceReference() { }
        public DeviceReference(DeviceType type, string name)
        {
            Type = type;
            Name = name;
        }

        public virtual object Clone()
        {
            return new DeviceReference(Type, Name);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DeviceReference reference))
            {
                return false;
            }

            return
                   Type == reference.Type &&
                   Name == reference.Name;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}