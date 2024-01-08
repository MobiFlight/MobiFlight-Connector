using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks.FlightSimBuilder
{
    internal class Gns530Handler
    {
        bool shiftState = false;
        Gns530Report lastReport = new Gns530Report();
        
        protected Dictionary<string, string> OctaviButtons = new Dictionary<string, string>();
        public Dictionary<string, int> OctaviButtonMatrix;
        public List<string> OctaviButtonList = new List<string>();
        public Dictionary<int, int> ButtonAssignmentMatrix = new Dictionary<int, int>();

        private Dictionary<uint, uint> HIDEventAssignments = new Dictionary<uint, uint>();

        public Gns530Handler()
        {
            OctaviButtons = new Dictionary<string, string>()
            {
                { "Button_COM1_OI", "COM1 Outer +" },
            };

            OctaviButtonMatrix = new Dictionary<string, int>()
            {
                { "Button_COM1_OI", 0 },
            };

            HIDEventAssignments = new Dictionary<uint, uint>()
            {
                { (uint)Gns530Report.OctaviButton.HID_BTN_TOG, 4 },
                { (uint)Gns530Report.OctaviButton.HID_ENC_SW, 5 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_DCT, 6 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_MENU, 7 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_CLR, 8 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_ENT, 9 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_AP, 10 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_AP_HDG, 11 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_AP_NAV, 12 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_AP_APR, 13 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_AP_ALT, 14 },
                { (uint)Gns530Report.OctaviButton.HID_BTN_AP_BC, 15 },
            };

            int i = 0;
            foreach(KeyValuePair<string, int> element in OctaviButtonMatrix)
            {
                if (element.Value == i)
                {
                    ButtonAssignmentMatrix.Add(i, OctaviButtonList.Count);
                    OctaviButtonList.Add(element.Key);
                }
                i++;
            }
            Dictionary<string, int> OctaviButtonMatrixCopy = OctaviButtonMatrix.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (KeyValuePair<string, int> element in OctaviButtonMatrix)
            {
                if(element.Value != -1)
                {
                    OctaviButtonMatrixCopy[element.Key] = ButtonAssignmentMatrix[element.Value];
                }
            }
            OctaviButtonMatrix = OctaviButtonMatrixCopy.ToDictionary(entry => entry.Key, entry => entry.Value);

        }
    public List<int> toButton(Gns530Report report)
        {
            List<int> ButtonPresses = new List<int>();
            uint pressed = report.buttonState & ~lastReport.buttonState; // only rising edges

            ButtonPresses.Add(1);


            lastReport = report;
        return ButtonPresses;
        }
    }
}
