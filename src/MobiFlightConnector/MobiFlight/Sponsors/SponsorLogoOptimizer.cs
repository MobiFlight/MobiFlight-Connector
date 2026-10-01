using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace MobiFlight.Sponsors
{
    internal sealed class OptimizedSponsorLogo
    {
        public byte[] Data { get; set; }

        public string MimeType { get; set; }
    }

    internal static class SponsorLogoOptimizer
    {
        public const int MaxWidth = 448;
        public const int MaxHeight = 128;

        public static OptimizedSponsorLogo Optimize(
            byte[] sourceData,
            string sourcePath)
        {
            if (sourceData == null || sourceData.Length == 0)
            {
                throw new ArgumentException(
                    "Source data is null or empty.",
                    nameof(sourceData));
            }

            var mimeType = GetMimeType(sourcePath);

            // Keep SVG files as-is.
            if (mimeType == "image/svg+xml")
            {
                return new OptimizedSponsorLogo
                {
                    Data = sourceData,
                    MimeType = mimeType
                };
            }

            try
            {
                using var inputStream = new MemoryStream(sourceData);
                using var sourceImage = Image.FromStream(inputStream);

                var scale = Math.Min(
                    1.0,
                    Math.Min(
                        (double)MaxWidth / sourceImage.Width,
                        (double)MaxHeight / sourceImage.Height));

                // Do not re-encode images that are already small enough.
                if (scale >= 1.0)
                {
                    return new OptimizedSponsorLogo
                    {
                        Data = sourceData,
                        MimeType = mimeType
                    };
                }

                var targetWidth = Math.Max(
                    1,
                    (int)Math.Round(sourceImage.Width * scale));

                var targetHeight = Math.Max(
                    1,
                    (int)Math.Round(sourceImage.Height * scale));

                // 32-bit ARGB is the appropriate format for PNG transparency.
                using var targetImage = new Bitmap(
                    targetWidth,
                    targetHeight,
                    PixelFormat.Format32bppArgb);

                if (sourceImage.HorizontalResolution > 0 &&
                    sourceImage.VerticalResolution > 0)
                {
                    targetImage.SetResolution(
                        sourceImage.HorizontalResolution,
                        sourceImage.VerticalResolution);
                }

                using (var graphics = Graphics.FromImage(targetImage))
                {
                    graphics.Clear(Color.Transparent);

                    // Preserve source alpha instead of blending it
                    // against the transparent target.
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality =
                        CompositingQuality.HighQuality;

                    graphics.InterpolationMode =
                        InterpolationMode.HighQualityBicubic;

                    graphics.SmoothingMode =
                        SmoothingMode.HighQuality;

                    graphics.PixelOffsetMode =
                        PixelOffsetMode.HighQuality;

                    var destination = new Rectangle(
                        0,
                        0,
                        targetWidth,
                        targetHeight);

                    graphics.DrawImage(
                        sourceImage,
                        destination,
                        0,
                        0,
                        sourceImage.Width,
                        sourceImage.Height,
                        GraphicsUnit.Pixel);
                }

                using var outputStream = new MemoryStream();

                targetImage.Save(
                    outputStream,
                    ImageFormat.Png);

                return new OptimizedSponsorLogo
                {
                    Data = outputStream.ToArray(),
                    MimeType = "image/png"
                };
            }
            catch (ArgumentException)
            {
                // If System.Drawing cannot process the format,
                // keep the original image.
                return new OptimizedSponsorLogo
                {
                    Data = sourceData,
                    MimeType = mimeType
                };
            }
        }

        private static string GetMimeType(string path)
        {
            var extension =
                Path.GetExtension(path)?.ToLowerInvariant();

            return extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "image/png"
            };
        }
    }
}