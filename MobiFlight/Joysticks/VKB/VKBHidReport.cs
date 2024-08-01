namespace MobiFlight.Joysticks.VKB
{
    internal class VKBHidReport
    {
        // Windows swallows a vital input report in the reconstructed descriptor. This fake descriptor helps us read all fields that are relevant for us, and only those.
        public static readonly byte[] Descriptor = new byte[]{
            0x05, 0x01,        // Usage Page (Generic Desktop Ctrls)
            0x09, 0x04,        // Usage (Joystick)
            0xA1, 0x01,        // Collection (Application)
            0x85, 0x08,        //   Report ID (8)
            0x05, 0x01,        //   Usage Page (Generic Desktop Ctrls)
            0x09, 0x00,        //   Usage (Undefined)
            0x75, 0x08,        //   Report Size (8)
            0x95, 0x3F,        //   Report Count (63)
            0x26, 0xFF, 0x00,  //   Logical Maximum (255)
            0x15, 0x00,        //   Logical Minimum (0)
            0x81, 0x01,        //   Input (Const,Array,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x85, 0x59,        //   Report ID (89)
            0x75, 0x08,        //   Report Size (8)
            0x95, 0x80,        //   Report Count (-128)
            0x09, 0x00,        //   Usage (Undefined)
            0xB1, 0x02,        //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0xC0              // End Collection
        };
    }
}
