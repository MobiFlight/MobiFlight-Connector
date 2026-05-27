using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MobiFlight.UI.StateBadge
{
    // Pure drawing helpers for the running/stopped badge. No window/icon state lives
    // here — callers own the resulting BadgedIcon and decide when to swap it in.
    internal static class StateBadgeRenderer
    {
        private static readonly Color RunningFill = Color.FromArgb(40, 200, 80);
        private static readonly Color StoppedFill = Color.FromArgb(220, 50, 50);
        private static readonly Color OutlineColor = Color.FromArgb(220, 0, 0, 0);

        // Composite a small state badge onto the top-right of baseIcon at its
        // native resolution, matching where Windows draws the taskbar overlay.
        // Running = green circle, stopped = red square.
        public static BadgedIcon BuildBadgedIcon(Icon baseIcon, bool isRunning)
        {
            int size = baseIcon.Width;
            using (var bmp = new Bitmap(size, size))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawIcon(baseIcon, new Rectangle(0, 0, size, size));

                int badgeSize = Math.Max(8, (int)(size * 0.4f));
                int x = size - badgeSize - 1;
                int y = 1;
                var badgeRect = new Rectangle(x, y, badgeSize, badgeSize);

                DrawBadgeShape(g, badgeRect, isRunning, Math.Max(1f, size / 32f));

                IntPtr hIcon = bmp.GetHicon();
                return new BadgedIcon(Icon.FromHandle(hIcon), hIcon);
            }
        }

        // Standalone overlay icon (no underlying app glyph) — Windows draws this
        // small in the corner of the taskbar button via ITaskbarList3.SetOverlayIcon.
        public static BadgedIcon BuildOverlayIcon(bool isRunning)
        {
            const int size = 16;
            using (var bmp = new Bitmap(size, size))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(1, 1, size - 3, size - 3);
                DrawBadgeShape(g, rect, isRunning, 1f);

                IntPtr hIcon = bmp.GetHicon();
                return new BadgedIcon(Icon.FromHandle(hIcon), hIcon);
            }
        }

        private static void DrawBadgeShape(Graphics g, Rectangle rect, bool isRunning, float penWidth)
        {
            Color fill = isRunning ? RunningFill : StoppedFill;
            using (var brush = new SolidBrush(fill))
            using (var pen = new Pen(OutlineColor, penWidth))
            {
                if (isRunning)
                {
                    g.FillEllipse(brush, rect);
                    g.DrawEllipse(pen, rect);
                }
                else
                {
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(pen, rect);
                }
            }
        }
    }
}
