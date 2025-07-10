namespace MobiFlight.xplane
{
    public interface XplaneCacheInterface : Base.CacheInterface
    {
        int UpdateFrequencyPerSecond { get; set; }

        void Start();

        void Stop();

        float readDataRef(string dataRefPath);

        void writeDataRef(string dataRefPath, float value);

        void sendCommand(string command);
    }
}
