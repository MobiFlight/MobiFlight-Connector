using System;
using System.Collections.Generic;
using System.Text;

namespace MobiFlightMoza.Protocol
{
    internal readonly struct McduInitConfig
    {
        public byte Version { get; }
        public ushort Rows { get; }
        public ushort Columns { get; }
        public ushort CellBytes { get; }
        public byte ServerCapabilities { get; }
        public ushort ServerUdpPort { get; }
        public ushort TcpKeyframeIntervalMs { get; }

        // `serverCapabilities` has no default value on purpose: `new McduInitConfig()` with
        // no arguments always binds to the compiler-synthesized zero-init struct
        // constructor, never to this one, so a defaultable parameter here would silently
        // produce an all-zero config. Use Default instead of `new McduInitConfig()`.
        public McduInitConfig(byte serverCapabilities)
        {
            Version = 2;
            Rows = MozaConstants.McduRows;
            Columns = MozaConstants.McduColumns;
            CellBytes = 2;
            ServerCapabilities = serverCapabilities;
            ServerUdpPort = MozaConstants.ServicePortMcduUdp;
            TcpKeyframeIntervalMs = 750;
        }

        public static McduInitConfig Default => new(0);
    }

    internal readonly struct McduClientCapability
    {
        public byte Version { get; }
        public byte Capabilities { get; }
        public ushort UdpListenPort { get; }
        public byte PageIndex { get; }

        public McduClientCapability(byte version, byte capabilities, ushort udpListenPort, byte pageIndex)
        {
            Version = version;
            Capabilities = capabilities;
            UdpListenPort = udpListenPort;
            PageIndex = pageIndex;
        }
    }

    internal readonly struct McduTextSegment
    {
        public byte StartColumn { get; }
        public string Text { get; }

        public McduTextSegment(byte startColumn, string text)
        {
            StartColumn = startColumn;
            Text = text ?? "";
        }
    }

    internal readonly struct McduTextRow
    {
        public byte Row { get; }
        public IReadOnlyList<McduTextSegment> Segments { get; }

        public McduTextRow(byte row, IReadOnlyList<McduTextSegment> segments)
        {
            Row = row;
            Segments = segments ?? [];
        }
    }

    internal readonly struct McduStyleSegment
    {
        public byte StartColumn { get; }
        public byte[] Styles { get; }

        public McduStyleSegment(byte startColumn, byte[] styles)
        {
            StartColumn = startColumn;
            Styles = styles ?? [];
        }
    }

    internal readonly struct McduStyleRow
    {
        public byte Row { get; }
        public IReadOnlyList<McduStyleSegment> Segments { get; }

        public McduStyleRow(byte row, IReadOnlyList<McduStyleSegment> segments)
        {
            Row = row;
            Segments = segments ?? [];
        }
    }

    /// <summary>
    /// Pack/parse for the MCDU channel's NetworkPackages: InitConfig (0x32), ClientCapability
    /// (0x33, parse only), Keyframe (0x30) and Delta (0x31, both share the same patch body).
    /// </summary>
    internal static class MozaMcduFrame
    {
        public static byte[] PackInitConfig(McduInitConfig config)
        {
            byte[] payload =
            [
                config.Version,
                (byte)config.Rows, (byte)(config.Rows >> 8),
                (byte)config.Columns, (byte)(config.Columns >> 8),
                (byte)config.CellBytes, (byte)(config.CellBytes >> 8),
                config.ServerCapabilities,
                (byte)config.ServerUdpPort, (byte)(config.ServerUdpPort >> 8),
                (byte)config.TcpKeyframeIntervalMs, (byte)(config.TcpKeyframeIntervalMs >> 8),
            ];
            return NetworkPackage.Pack(0x32, payload);
        }

        public static bool TryParseClientCapability(byte[] payload, out McduClientCapability capability)
        {
            capability = default;
            if (payload == null || payload.Length != 5) return false;

            byte version = payload[0];
            byte capabilities = payload[1];
            ushort udpPort = (ushort)(payload[2] | (payload[3] << 8));
            byte pageIndex = payload[4];
            if (version != 2) return false;
            if (pageIndex != 0 && pageIndex != 1) return false;

            capability = new McduClientCapability(version, capabilities, udpPort, pageIndex);
            return true;
        }

        public static byte[] PackKeyframe(uint sequence, IReadOnlyList<McduTextRow> textRows, IReadOnlyList<McduStyleRow> styleRows)
            => NetworkPackage.Pack(0x30, PackPatchPayload(sequence, textRows, styleRows, null));

        public static byte[] PackDelta(uint sequence, IReadOnlyList<McduTextRow> textRows, IReadOnlyList<McduStyleRow> styleRows, uint? baseSequence = null)
            => NetworkPackage.Pack(0x31, PackPatchPayload(sequence, textRows, styleRows, baseSequence));

        private static byte[] PackPatchPayload(uint sequence, IReadOnlyList<McduTextRow> textRows, IReadOnlyList<McduStyleRow> styleRows, uint? baseSequence)
        {
            textRows ??= [];
            styleRows ??= [];
            if (textRows.Count > 255) throw new ArgumentException("More than 255 text rows.", nameof(textRows));
            if (styleRows.Count > 255) throw new ArgumentException("More than 255 style rows.", nameof(styleRows));

            byte flags = 0;
            if (textRows.Count > 0) flags |= 0x01;
            if (styleRows.Count > 0) flags |= 0x02;
            if (baseSequence.HasValue) flags |= 0x04;

            List<byte> payload =
            [
                2, // patch format version
                (byte)sequence, (byte)(sequence >> 8), (byte)(sequence >> 16), (byte)(sequence >> 24),
                flags,
            ];
            if (baseSequence.HasValue)
            {
                uint b = baseSequence.Value;
                payload.AddRange((byte[])[(byte)b, (byte)(b >> 8), (byte)(b >> 16), (byte)(b >> 24)]);
            }

            payload.Add((byte)textRows.Count);
            foreach (var row in textRows)
            {
                AppendTextRow(payload, row);
            }

            payload.Add((byte)styleRows.Count);
            foreach (var row in styleRows)
            {
                AppendStyleRow(payload, row);
            }

            return [.. payload];
        }

        private static void AppendTextRow(List<byte> payload, McduTextRow row)
        {
            if (row.Segments.Count > 255) throw new ArgumentException("More than 255 segments in one text row.");
            payload.Add(row.Row);
            payload.Add((byte)row.Segments.Count);
            foreach (var segment in row.Segments)
            {
                if (segment.Text.Length > 255) throw new ArgumentException("Text segment spans more than 255 columns.");
                byte[] utf8 = Encoding.UTF8.GetBytes(segment.Text);
                if (utf8.Length > ushort.MaxValue) throw new ArgumentException("Text segment's UTF-8 encoding is too long.");

                payload.Add(segment.StartColumn);
                payload.Add((byte)segment.Text.Length); // ColumnLength: UTF-16 code units, not UTF-8 bytes
                payload.Add((byte)utf8.Length);
                payload.Add((byte)(utf8.Length >> 8));
                payload.AddRange(utf8);
            }
        }

        private static void AppendStyleRow(List<byte> payload, McduStyleRow row)
        {
            if (row.Segments.Count > 255) throw new ArgumentException("More than 255 segments in one style row.");
            payload.Add(row.Row);
            payload.Add((byte)row.Segments.Count);
            foreach (var segment in row.Segments)
            {
                if (segment.Styles.Length > 255) throw new ArgumentException("Style segment spans more than 255 columns.");
                payload.Add(segment.StartColumn);
                payload.Add((byte)segment.Styles.Length);
                payload.AddRange(segment.Styles);
            }
        }
    }
}
