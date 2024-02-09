namespace MobiFlight.Frontend
{
    public interface IConfigItem
    {
        string GUID { get; set; }
        bool Active { get; set; }
        string Description { get; set; }
        string Device { get; set; }
        string Component { get; set; }
        string Type { get; set; }
        string[] Tags { get; set; }
        string[] Status { get; set; }
        string RawValue { get; set; }
        string ModifiedValue { get; set; }
    }
}
