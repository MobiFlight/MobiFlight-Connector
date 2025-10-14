using MobiFlight.Base;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandResortConfigItem
    {
        public ConfigItem[] Items { get; set; }
        public int SourceFileIndex { get; set; }
        public int TargetFileIndex { get; set; }
        public int NewIndex { get; set; }
    }
}
