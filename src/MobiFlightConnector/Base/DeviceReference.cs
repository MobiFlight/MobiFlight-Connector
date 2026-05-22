namespace MobiFlight.Base
{
    public class DeviceReference
    {
        public virtual string Name { get; set; }

        // temporay property, will be removed before merging with the main branch
        public string Label { get; set; }
        public string SubId { get; set; }
        public DeviceType Type { get; set; }

        public DeviceReference() { }
        public DeviceReference(DeviceType type, string name, string subId = null)
        {
            Type = type;
            Name = name;
            SubId = subId;
        }

        public virtual object Clone()
        {
            return new DeviceReference(Type, Name, SubId);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DeviceReference reference))
            {
                return false;
            }

            return
                   Type == reference.Type &&
                   Name == reference.Name &&
                   SubId == reference.SubId;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                hash = hash * 23 + (SubId?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}