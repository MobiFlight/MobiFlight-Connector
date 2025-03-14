using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight
{
    public interface IConfigFile
    {
        List<IConfigItem> ConfigItems { get; set; }
        string FileName { get; set; }
        bool ReferenceOnly { get; }
        bool EmbedContent { get; }

        void OpenFile();
        void SaveFile();
    }
}