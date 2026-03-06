using MobiFlight.Joysticks.Bodnar;

namespace MobiFlight.Joysticks.AuthentiKit
{
    public class AuthentiKitReport : BU0836AReport
    {
        public new AuthentiKitReport Parse(byte[] inputBuffer)
        {
            var result = new AuthentiKitReport();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }
    }
}
