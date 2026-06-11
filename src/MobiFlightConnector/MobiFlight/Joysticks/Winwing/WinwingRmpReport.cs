using System;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingRmpReport
    {
        public uint ReportId { get; set; }
        public uint ButtonState { get; set; }
        public ushort InnerKnobValue { get; set; }
        public ushort OuterKnobValue { get; set; }

        private const uint BUTTONS_REPORT = 1;

        public void CopyTo(WinwingRmpReport targetReport)
        {
            targetReport.ReportId = this.ReportId;
            targetReport.ButtonState = this.ButtonState;

            targetReport.InnerKnobValue = this.InnerKnobValue;
            targetReport.OuterKnobValue = this.OuterKnobValue;
        }

        public void ParseReport(HidBuffer hidBuffer)
        {
            byte[] data = hidBuffer.HidReport.TransferResult.Data;
            ReportId = hidBuffer.HidReport.ReportId;
            if (ReportId == BUTTONS_REPORT)
            {
                // get 32 bit Button report field - First 4 bytes: uint:  [3][2][1][0]
                ButtonState = ((uint)data[0] + ((uint)data[1] << 8) + ((uint)data[2] << 16) + ((uint)data[3] << 24));

                OuterKnobValue = (ushort)(data[9] | (data[10] << 8));
                InnerKnobValue = (ushort)(data[11] | (data[12] << 8)); 
            }
        }   
    }
}
