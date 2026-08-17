using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MobiFlight
{
    internal static class ProcessHelpers
    {
        public static Process OpenUrl(string url)
        {
            return Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
    }
}
