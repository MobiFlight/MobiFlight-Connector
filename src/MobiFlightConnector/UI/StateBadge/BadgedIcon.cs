using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MobiFlight.UI.StateBadge
{
    // Owns a managed Icon together with the unmanaged HICON that backs it.
    // Bitmap.GetHicon() returns a handle the caller must DestroyIcon — Icon.FromHandle
    // does not take ownership — so this pair has to be disposed as a unit or
    // every state toggle leaks one GDI icon handle.
    internal sealed class BadgedIcon : IDisposable
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public Icon Icon { get; }
        public IntPtr Handle { get; }

        public BadgedIcon(Icon icon, IntPtr handle)
        {
            Icon = icon;
            Handle = handle;
        }

        public void Dispose()
        {
            Icon?.Dispose();
            if (Handle != IntPtr.Zero) DestroyIcon(Handle);
        }
    }
}
