using System;

namespace MobiFlight.Joysticks
{
    /// <summary>
    /// A raw HID input report with access to report ID and payload.
    /// </summary>
    internal class HidReport
    {
        /// <summary>Raw report bytes including the report ID at index 0.</summary>
        public byte[] Buffer { get; }

        /// <summary>The report ID (first byte of the raw report).</summary>
        public byte ReportId
        {
            get { return Buffer[0]; }
        }

        /// <summary>Report bytes after the report ID, created lazily on first access.</summary>
        public byte[] Payload
        {
            get
            {
                if (payload == null)
                {
                    payload = new byte[Buffer.Length - 1];
                    Array.Copy(Buffer, 1, payload, 0, payload.Length);
                }
                return payload;
            }
        }
        private byte[] payload;

        public HidReport(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
