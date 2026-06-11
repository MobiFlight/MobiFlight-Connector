using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    internal abstract class SegmentDisplayDeviceBase : WinwingDeviceBase
    {
        protected const string ANN_LIGHT = "LCD Test On/Off";
        protected const int    HeaderOffset = 17;

        protected Dictionary<string, DisplaySegment> DisplaySetValueSegments;
        protected Dictionary<string, DisplaySegment> DisplayTestCommands;

        protected readonly byte[] DisplayTestCommand;
        protected readonly byte[] RefreshCommand;
        protected readonly byte[] SetValuesCommand;

        protected SegmentDisplayDeviceBase(IWinwingMessageSender sender, byte[] destinationAddress, int setValuesSize)
            : base(sender, destinationAddress)
        {
            DisplayTestCommand = new byte[0x12];
            RefreshCommand     = new byte[0x11];
            SetValuesCommand   = new byte[setValuesSize];
        }

        // Subclasses provide the header keys defined in WinwingConstants.DisplayCmdHeaders.
        protected abstract string SetValuesHeaderKey { get; }
        protected virtual  string DisplayTestHeaderKey => "0401";
        protected virtual  string RefreshHeaderKey     => "0301";

        // The header section may use a different destination address (e.g. PAC for the throttle).
        protected virtual byte[] HeaderDestinationAddress => DestinationAddress;

        protected void PrepareCommands()
        {
            BuildHeader(DisplayTestCommand, DisplayTestHeaderKey);
            BuildHeader(SetValuesCommand,   SetValuesHeaderKey);
            BuildHeader(RefreshCommand,     RefreshHeaderKey);

            foreach (var seg in DisplaySetValueSegments.Values)
            {
                ApplySegment(seg, SetValuesCommand);
            }
        }

        private void BuildHeader(byte[] target, string headerKey)
        {
            var init = new List<byte>(HeaderDestinationAddress);
            init.AddRange(new byte[2]);
            init.AddRange(WinwingConstants.DisplayCmdHeaders[headerKey]);
            init.CopyTo(target, 0);
        }

        protected void ApplySegment(DisplaySegment seg, byte[] msg)
        {
            foreach (Bit b in seg.Bits)
            {
                int idx = b.ByteNumber + HeaderOffset;
                msg[idx] = b.Value
                    ? (byte)(msg[idx] |  (1 << b.BitPosition))
                    : (byte)(msg[idx] & ~(1 << b.BitPosition));
            }
        }

        protected void SendValues()
        {
            MessageSender.SendDisplayCommands(new byte[][] { SetValuesCommand, RefreshCommand });
        }

        protected void SendTest(byte[] testFrame)
        {
            MessageSender.SendDisplayCommands(new byte[][] { testFrame, RefreshCommand });
        }

        protected void LcdTest(string variant)
        {
            ApplySegment(DisplayTestCommands[variant], DisplayTestCommand);
            SendTest(DisplayTestCommand);
        }

        // Default annunciator behaviour: "1" → AllOn test pattern, otherwise re-emit the current SetValues frame.
        protected virtual void SetAnnunciatorLightOnOff(string s)
        {
            if (AsBool(s))
            {
                LcdTest("AllOn");
            }
            else
            {
                SendValues();
            }
        }

        // Setter helpers (used by Step 4 of the refactoring).
        protected void SetDigits(char[] chars, params string[] segmentNames)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                var seg = DisplaySetValueSegments[segmentNames[i]];
                seg.SetCharacter(chars[i]);
                ApplySegment(seg, SetValuesCommand);
            }
        }

        protected void SetSegmentBool(string segmentName, bool value)
        {
            var seg = DisplaySetValueSegments[segmentName];
            seg.SetValue(value);
            ApplySegment(seg, SetValuesCommand);
        }

        protected void ClearLcdCache(params string[] names)
        {
            foreach (var n in names)
            {
                LcdCurrentValuesCache[n] = string.Empty;
            }
        }
    }
}
