using System;
using System.Runtime.InteropServices;

namespace MobiFlight.UI.StateBadge
{
    // Wraps the Win7+ ITaskbarList3.SetOverlayIcon shell call (the same API
    // Discord/Slack use to badge their pinned taskbar buttons). Owns the COM
    // object lifetime and the currently-applied overlay icon's GDI handle.
    internal sealed class TaskbarOverlay : IDisposable
    {
        private ITaskbarList3 taskbarList;
        private bool initFailed;
        private BadgedIcon currentOverlay;

        public void Set(IntPtr hwnd, bool isRunning, string description)
        {
            if (initFailed) return;
            if (hwnd == IntPtr.Zero) return; // can't overlay a window that has no HWND yet

            try
            {
                if (taskbarList == null)
                {
                    taskbarList = (ITaskbarList3)new TaskbarInstance();
                    taskbarList.HrInit();
                }

                var newOverlay = StateBadgeRenderer.BuildOverlayIcon(isRunning);
                taskbarList.SetOverlayIcon(hwnd, newOverlay.Handle, description);

                currentOverlay?.Dispose();
                currentOverlay = newOverlay;
            }
            catch (Exception ex)
            {
                // Pre-Win7 or no shell — give up silently.
                initFailed = true;
                Log.Instance.log("Taskbar overlay unavailable: " + ex.Message, LogSeverity.Info);
            }
        }

        public void Dispose()
        {
            currentOverlay?.Dispose();
            currentOverlay = null;
            if (taskbarList != null)
            {
                Marshal.FinalReleaseComObject(taskbarList);
                taskbarList = null;
            }
        }

        // COM interop for ITaskbarList3 — the shell interface that backs the Win7+
        // taskbar API (progress bars, overlay icons, etc.). Methods MUST be declared
        // in vtable order; we stop after SetOverlayIcon since we don't call any of
        // the methods that come after it.
        [ComImport]
        [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList3
        {
            // ITaskbarList
            void HrInit();
            void AddTab(IntPtr hwnd);
            void DeleteTab(IntPtr hwnd);
            void ActivateTab(IntPtr hwnd);
            void SetActiveAlt(IntPtr hwnd);
            // ITaskbarList2
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
            // ITaskbarList3 (only up through SetOverlayIcon — slots after are unused)
            void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
            void SetProgressState(IntPtr hwnd, int tbpFlags);
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            void UnregisterTab(IntPtr hwndTab);
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);
            void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
            void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
        }

        [ComImport]
        [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class TaskbarInstance { }
    }
}
