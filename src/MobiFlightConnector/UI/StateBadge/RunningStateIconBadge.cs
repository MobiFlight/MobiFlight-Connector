using System;
using System.Drawing;
using System.Windows.Forms;

namespace MobiFlight.UI.StateBadge
{
    // Orchestrates the three places the running/stopped state is shown:
    //   - the system tray icon
    //   - the form's titlebar icon
    //   - the Windows taskbar overlay (pinned/visible while the app is running)
    //
    // The designer-loaded base icons are stashed on first use so we can keep
    // re-composing badges over the originals across many state toggles.
    internal sealed class RunningStateIconBadge : IDisposable
    {
        private readonly NotifyIcon notifyIcon;
        private readonly Form form;
        private readonly TaskbarOverlay taskbarOverlay = new TaskbarOverlay();

        private Icon notifyIconBase;
        private Icon formIconBase;

        private BadgedIcon currentNotifyIcon;
        private BadgedIcon currentFormIcon;

        public RunningStateIconBadge(NotifyIcon notifyIcon, Form form)
        {
            this.notifyIcon = notifyIcon;
            this.form = form;
        }

        public void Update(bool isRunning)
        {
            if (notifyIconBase == null) notifyIconBase = notifyIcon.Icon;
            if (formIconBase == null) formIconBase = form.Icon;

            if (notifyIconBase != null)
            {
                var next = StateBadgeRenderer.BuildBadgedIcon(notifyIconBase, isRunning);
                notifyIcon.Icon = next.Icon;
                currentNotifyIcon?.Dispose();
                currentNotifyIcon = next;
            }

            if (formIconBase != null)
            {
                var next = StateBadgeRenderer.BuildBadgedIcon(formIconBase, isRunning);
                form.Icon = next.Icon;
                currentFormIcon?.Dispose();
                currentFormIcon = next;
            }

            string stateLabel = i18n._tr(isRunning ? "Running" : "Stopped");

            if (form.IsHandleCreated)
            {
                taskbarOverlay.Set(form.Handle, isRunning, stateLabel);
            }

            notifyIcon.Text = string.Format(i18n._tr("uiLabelTrayTooltipFormat"), stateLabel);
        }

        public void Dispose()
        {
            currentNotifyIcon?.Dispose();
            currentFormIcon?.Dispose();
            taskbarOverlay.Dispose();
        }
    }
}
