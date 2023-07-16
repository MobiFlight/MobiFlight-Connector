using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MobiFlight.Joysticks.Octavi
{
    internal class OctaviHandler
    {
        bool shiftState = false;
        OctaviReport lastReport = new OctaviReport();
        byte contextState = (byte)OctaviReport.OctaviState.STATE_COM1;

        protected Dictionary<string, string> OctaviButtons = new Dictionary<string, string>();
        public Dictionary<string, int> OctaviButtonMatrix;
        public List<string> OctaviButtonList = new List<string>();
        public Dictionary<int, int> ButtonAssignmentMatrix = new Dictionary<int, int>();

        private Dictionary<uint, uint> HIDEventAssignments = new Dictionary<uint, uint>();

        public OctaviHandler()
        {
            OctaviButtons = new Dictionary<string, string>()
            {
                { "Button_COM1_OI", "COM1 Outer +" },
                { "Button_COM1_OD", "COM1 Outer -" },
                { "Button_COM1_II", "COM1 Inner +" },
                { "Button_COM1_ID", "COM1 Inner -" },
                { "Button_COM1_TOG", "COM1 Toggle" },
                { "Button_COM2_OI", "COM2 Outer +" },
                { "Button_COM2_OD", "COM2 Outer -" },
                { "Button_COM2_II", "COM2 Inner +" },
                { "Button_COM2_ID", "COM2 Inner -" },
                { "Button_COM2_TOG", "COM2 Toggle" },
                { "Button_NAV1_OI", "NAV1 Outer +" },
                { "Button_NAV1_OD", "NAV1 Outer -" },
                { "Button_NAV1_II", "NAV1 Inner +" },
                { "Button_NAV1_ID", "NAV1 Inner -" },
                { "Button_NAV1_TOG", "NAV1 Toggle" },
                { "Button_NAV2_OI", "NAV2 Outer +" },
                { "Button_NAV2_OD", "NAV2 Outer -" },
                { "Button_NAV2_II", "NAV2 Inner +" },
                { "Button_NAV2_ID", "NAV2 Inner -" },
                { "Button_NAV2_TOG", "NAV2 Toggle" },
                { "Button_FMS1_OI", "FMS1 Outer +" },
                { "Button_FMS1_OD", "FMS1 Outer -" },
                { "Button_FMS1_II", "FMS1 Inner +" },
                { "Button_FMS1_ID", "FMS1 Inner -" },
                { "Button_FMS1_TOG", "FMS1 Toggle" },
                { "Button_FMS2_OI", "FMS2 Outer +" },
                { "Button_FMS2_OD", "FMS2 Outer -" },
                { "Button_FMS2_II", "FMS2 Inner +" },
                { "Button_FMS2_ID", "FMS2 Inner -" },
                { "Button_FMS2_TOG", "FMS2 Toggle" },
                { "Button_AP_OI", "AP Outer +" },
                { "Button_AP_OD", "AP Outer -" },
                { "Button_AP_II", "AP Inner +" },
                { "Button_AP_ID", "AP Inner -" },
                { "Button_AP_TOG", "AP Toggle" },
                { "Button_XPDR_OI", "XPDR Outer +" },
                { "Button_XPDR_OD", "XPDR Outer -" },
                { "Button_XPDR_II", "XPDR Inner +" },
                { "Button_XPDR_ID", "XPDR Inner -" },
                { "Button_XPDR_TOG", "XPDR Toggle" },
                { "Button_FMS1_CRSR", "FMS1 CRSR" },
                { "Button_FMS1_DCT", "FMS1 DCT" },
                { "Button_FMS1_MENU", "FMS1 MENU" },
                { "Button_FMS1_CLR", "FMS1 CLR" },
                { "Button_FMS1_ENT", "FMS1 ENT" },
                { "Button_FMS1_CDI", "FMS1 CDI" },
                { "Button_FMS1_OBS", "FMS1 OBS" },
                { "Button_FMS1_MSG", "FMS1 MSG" },
                { "Button_FMS1_FPL", "FMS1 FPL" },
                { "Button_FMS1_VNAV", "FMS1 VNAV" },
                { "Button_FMS1_PROC", "FMS1 PROC" },
                { "Button_FMS2_CRSR", "FMS2 CRSR" },
                { "Button_FMS2_DCT", "FMS2 DCT" },
                { "Button_FMS2_MENU", "FMS2 MENU" },
                { "Button_FMS2_CLR", "FMS2 CLR" },
                { "Button_FMS2_ENT", "FMS2 ENT" },
                { "Button_FMS2_CDI", "FMS2 CDI" },
                { "Button_FMS2_OBS", "FMS2 OBS" },
                { "Button_FMS2_MSG", "FMS2 MSG" },
                { "Button_FMS2_FPL", "FMS2 FPL" },
                { "Button_FMS2_VNAV", "FMS2 VNAV" },
                { "Button_FMS2_PROC", "FMS2 PROC" },
                { "Button_AP_AP", "Autopilot Toggle" },
                { "Button_AP_HDG", "Autopilot HDG" },
                { "Button_AP_NAV", "Autopilot NAV" },
                { "Button_AP_APR", "Autopilot APR" },
                { "Button_AP_VS", "Autopilot VS" },
                { "Button_AP_BC", "Autopilot BC" },
            };

            OctaviButtonMatrix = new Dictionary<string, int>()
            {
                //new List<string>()
                //{
                //"BTN_COM1_OI",
                //"BTN_COM1_OD",
                //"BTN_COM1_II",
                //"BTN_COM1_ID",
                //"BTN_COM1_TOG",
                //"BTN_COM1_CRSR",
                //"BTN_COM1_DCT",
                //"BTN_COM1_MENU",
                //"BTN_COM1_CLR",
                //"BTN_COM1_ENT",
                //"BTN_COM1_CDI",
                //"BTN_COM1_OBS",
                //"BTN_COM1_MSG",
                //"BTN_COM1_FPL",
                //"BTN_COM1_VNAV",
                //"BTN_COM1_PROC",
                //},
                //new List<string>()
                //{
                //"BTN_COM2_OI",
                //"BTN_COM2_OD",
                //"BTN_COM2_II",
                //"BTN_COM2_ID",
                //"BTN_COM2_TOG",
                //"BTN_COM2_CRSR",
                //"BTN_COM2_DCT",
                //"BTN_COM2_MENU",
                //"BTN_COM2_CLR",
                //"BTN_COM2_ENT",
                //"BTN_COM2_CDI",
                //"BTN_COM2_OBS",
                //"BTN_COM2_MSG",
                //"BTN_COM2_FPL",
                //"BTN_COM2_VNAV",
                //"BTN_COM2_PROC",
                //},
                //new List<string>()
                //{
                //"BTN_NAV1_OI",
                //"BTN_NAV1_OD",
                //"BTN_NAV1_II",
                //"BTN_NAV1_ID",
                //"BTN_NAV1_TOG",
                //"BTN_NAV1_CRSR",
                //"BTN_NAV1_DCT",
                //"BTN_NAV1_MENU",
                //"BTN_NAV1_CLR",
                //"BTN_NAV1_ENT",
                //"BTN_NAV1_CDI",
                //"BTN_NAV1_OBS",
                //"BTN_NAV1_MSG",
                //"BTN_NAV1_FPL",
                //"BTN_NAV1_VNAV",
                //"BTN_NAV1_PROC",
                //},
                //new List<string>()
                //{
                //"BTN_NAV2_OI",
                //"BTN_NAV2_OD",
                //"BTN_NAV2_II",
                //"BTN_NAV2_ID",
                //"BTN_NAV2_TOG",
                //"BTN_NAV2_CRSR",
                //"BTN_NAV2_DCT",
                //"BTN_NAV2_MENU",
                //"BTN_NAV2_CLR",
                //"BTN_NAV2_ENT",
                //"BTN_NAV2_CDI",
                //"BTN_NAV2_OBS",
                //"BTN_NAV2_MSG",
                //"BTN_NAV2_FPL",
                //"BTN_NAV2_VNAV",
                //"BTN_NAV2_PROC",
                //},
                //new List<string>()
                //{
                //"BTN_FMS1_OI",
                //"BTN_FMS1_OD",
                //"BTN_FMS1_II",
                //"BTN_FMS1_ID",
                //"BTN_FMS1_TOG",
                //"BTN_FMS1_CRSR",
                //"BTN_FMS1_DCT",
                //"BTN_FMS1_MENU",
                //"BTN_FMS1_CLR",
                //"BTN_FMS1_ENT",
                //"BTN_FMS1_CDI",
                //"BTN_FMS1_OBS",
                //"BTN_FMS1_MSG",
                //"BTN_FMS1_FPL",
                //"BTN_FMS1_VNAV",
                //"BTN_FMS1_PROC",
                //},
                //new List<string>()
                //{
                //"BTN_FMS2_OI",
                //"BTN_FMS2_OD",
                //"BTN_FMS2_II",
                //"BTN_FMS2_ID",
                //"BTN_FMS2_TOG",
                //"BTN_FMS2_CRSR",
                //"BTN_FMS2_DCT",
                //"BTN_FMS2_MENU",
                //"BTN_FMS2_CLR",
                //"BTN_FMS2_ENT",
                //"BTN_FMS2_CDI",
                //"BTN_FMS2_OBS",
                //"BTN_FMS2_MSG",
                //"BTN_FMS2_FPL",
                //"BTN_FMS2_VNAV",
                //"BTN_FMS2_PROC",
                //},
                //new List<string>()
                //{
                //"BTN_AP_OI",
                //"BTN_AP_OD",
                //"BTN_AP_II",
                //"BTN_AP_ID",
                //"BTN_AP_TOG",
                //"BTN_AP_CRSR",
                //"BTN_AP_DCT",
                //"BTN_AP_MENU",
                //"BTN_AP_CLR",
                //"BTN_AP_ENT",
                //"BTN_AP_CDI",
                //"BTN_AP_OBS",
                //"BTN_AP_MSG",
                //"BTN_AP_FPL",
                //"BTN_AP_VNAV",
                //"BTN_AP_PROC",
                //},
                //new List<string>()
                //{
                //"BTN_XPDR_OI",
                //"BTN_XPDR_OD",
                //"BTN_XPDR_II",
                //"BTN_XPDR_ID",
                //"BTN_XPDR_TOG",
                //"BTN_XPDR_CRSR",
                //"BTN_XPDR_DCT",
                //"BTN_XPDR_MENU",
                //"BTN_XPDR_CLR",
                //"BTN_XPDR_ENT",
                //"BTN_XPDR_CDI",
                //"BTN_XPDR_OBS",
                //"BTN_XPDR_MSG",
                //"BTN_XPDR_FPL",
                //"BTN_XPDR_VNAV",
                //"BTN_XPDR_PROC",
                //},
                { "Button_COM1_OI", 0 },
                { "Button_COM1_OD", 1 },
                { "Button_COM1_II", 2 },
                { "Button_COM1_ID", 3 },
                { "Button_COM1_TOG", 4 },
                { "Button_COM1_CRSR", -1 },
                { "Button_COM1_DCT", -1 },
                { "Button_COM1_MENU", -1 },
                { "Button_COM1_CLR", -1 },
                { "Button_COM1_ENT", -1 },
                { "Button_COM1_CDI", 106 },
                { "Button_COM1_OBS", 107 },
                { "Button_COM1_MSG", 108 },
                { "Button_COM1_FPL", 109 },
                { "Button_COM1_VNAV", 110 },
                { "Button_COM1_PROC", 111 },
                { "Button_COM2_OI", 16 },
                { "Button_COM2_OD", 17 },
                { "Button_COM2_II", 18 },
                { "Button_COM2_ID", 19 },
                { "Button_COM2_TOG", 20 },
                { "Button_COM2_CRSR", 21 },
                { "Button_COM2_DCT", -1 },
                { "Button_COM2_MENU", -1 },
                { "Button_COM2_CLR", -1 },
                { "Button_COM2_ENT", -1 },
                { "Button_COM2_CDI", 106 },
                { "Button_COM2_OBS", 107 },
                { "Button_COM2_MSG", 108 },
                { "Button_COM2_FPL", 109 },
                { "Button_COM2_VNAV", 110 },
                { "Button_COM2_PROC", 111 },
                { "Button_NAV1_OI", 32 },
                { "Button_NAV1_OD", 33 },
                { "Button_NAV1_II", 34 },
                { "Button_NAV1_ID", 35 },
                { "Button_NAV1_TOG", 36 },
                { "Button_NAV1_CRSR", 37 },
                { "Button_NAV1_DCT", -1 },
                { "Button_NAV1_MENU", -1 },
                { "Button_NAV1_CLR", -1 },
                { "Button_NAV1_ENT", -1 },
                { "Button_NAV1_CDI", 106 },
                { "Button_NAV1_OBS", 107 },
                { "Button_NAV1_MSG", 108 },
                { "Button_NAV1_FPL", 109 },
                { "Button_NAV1_VNAV", 110 },
                { "Button_NAV1_PROC", 111 },
                { "Button_NAV2_OI", 48 },
                { "Button_NAV2_OD", 49 },
                { "Button_NAV2_II", 50 },
                { "Button_NAV2_ID", 51 },
                { "Button_NAV2_TOG", 52 },
                { "Button_NAV2_CRSR", 53 },
                { "Button_NAV2_DCT", -1 },
                { "Button_NAV2_MENU", -1 },
                { "Button_NAV2_CLR", -1 },
                { "Button_NAV2_ENT", -1 },
                { "Button_NAV2_CDI", 106 },
                { "Button_NAV2_OBS", 107 },
                { "Button_NAV2_MSG", 108 },
                { "Button_NAV2_FPL", 109 },
                { "Button_NAV2_VNAV", 110 },
                { "Button_NAV2_PROC", 111 },
                { "Button_FMS1_OI", 64 },
                { "Button_FMS1_OD", 65 },
                { "Button_FMS1_II", 66 },
                { "Button_FMS1_ID", 67 },
                { "Button_FMS1_TOG", 68 },
                { "Button_FMS1_CRSR", 69 },
                { "Button_FMS1_DCT", 70 },
                { "Button_FMS1_MENU", 71 },
                { "Button_FMS1_CLR", 72 },
                { "Button_FMS1_ENT", 73 },
                { "Button_FMS1_CDI", 74 },
                { "Button_FMS1_OBS", 75 },
                { "Button_FMS1_MSG", 76 },
                { "Button_FMS1_FPL", 77 },
                { "Button_FMS1_VNAV", 78 },
                { "Button_FMS1_PROC", 79 },
                { "Button_FMS2_OI", 80 },
                { "Button_FMS2_OD", 81 },
                { "Button_FMS2_II", 82 },
                { "Button_FMS2_ID", 83 },
                { "Button_FMS2_TOG", 84 },
                { "Button_FMS2_CRSR", 85 },
                { "Button_FMS2_DCT", 86 },
                { "Button_FMS2_MENU", 87 },
                { "Button_FMS2_CLR", 88 },
                { "Button_FMS2_ENT", 89 },
                { "Button_FMS2_CDI", 90 },
                { "Button_FMS2_OBS", 91 },
                { "Button_FMS2_MSG", 92 },
                { "Button_FMS2_FPL", 93 },
                { "Button_FMS2_VNAV", 94 },
                { "Button_FMS2_PROC", 95 },
                { "Button_AP_OI", 96 },
                { "Button_AP_OD", 97 },
                { "Button_AP_II", 98 },
                { "Button_AP_ID", 99 },
                { "Button_AP_TOG", 100 },
                { "Button_AP_CRSR", 101 },
                { "Button_AP_DCT", -1 },
                { "Button_AP_MENU", -1 },
                { "Button_AP_CLR", -1 },
                { "Button_AP_ENT", -1 },
                { "Button_AP_CDI", 106 },
                { "Button_AP_OBS", 107 },
                { "Button_AP_MSG", 108 },
                { "Button_AP_FPL", 109 },
                { "Button_AP_VNAV", 110 },
                { "Button_AP_PROC", 111 },
                { "Button_XPDR_OI", 112 },
                { "Button_XPDR_OD", 113 },
                { "Button_XPDR_II", 114 },
                { "Button_XPDR_ID", 115 },
                { "Button_XPDR_TOG", 116 },
                { "Button_XPDR_CRSR", 117 },
                { "Button_XPDR_DCT", -1 },
                { "Button_XPDR_MENU", -1 },
                { "Button_XPDR_CLR", -1 },
                { "Button_XPDR_ENT", -1 },
                { "Button_XPDR_CDI", 106 },
                { "Button_XPDR_OBS", 107 },
                { "Button_XPDR_MSG", 108 },
                { "Button_XPDR_FPL", 109 },
                { "Button_XPDR_VNAV", 110 },
                { "Button_XPDR_PROC", 111 },
                { "Button_COM1^_OI", 128 },
                { "Button_COM1^_OD", 129 },
                { "Button_COM1^_II", 130 },
                { "Button_COM1^_ID", 131 },
                { "Button_COM1^_TOG", 132 },
                { "Button_COM1^_CRSR", -1 },
                { "Button_COM1^_DCT", -1 },
                { "Button_COM1^_MENU", -1 },
                { "Button_COM1^_CLR", -1 },
                { "Button_COM1^_ENT", -1 },
                { "Button_COM1^_CDI", 106 },
                { "Button_COM1^_OBS", 107 },
                { "Button_COM1^_MSG", 108 },
                { "Button_COM1^_FPL", 109 },
                { "Button_COM1^_VNAV", 110 },
                { "Button_COM1^_PROC", 111 },
                { "Button_COM2^_OI", 144 },
                { "Button_COM2^_OD", 145 },
                { "Button_COM2^_II", 146 },
                { "Button_COM2^_ID", 147 },
                { "Button_COM2^_TOG", 148 },
                { "Button_COM2^_CRSR", -1 },
                { "Button_COM2^_DCT", -1 },
                { "Button_COM2^_MENU", -1 },
                { "Button_COM2^_CLR", -1 },
                { "Button_COM2^_ENT", -1 },
                { "Button_COM2^_CDI", 106 },
                { "Button_COM2^_OBS", 107 },
                { "Button_COM2^_MSG", 108 },
                { "Button_COM2^_FPL", 109 },
                { "Button_COM2^_VNAV", 110 },
                { "Button_COM2^_PROC", 111 },
                { "Button_NAV1^_OI", 160 },
                { "Button_NAV1^_OD", 161 },
                { "Button_NAV1^_II", 162 },
                { "Button_NAV1^_ID", 163 },
                { "Button_NAV1^_TOG", 164 },
                { "Button_NAV1^_CRSR", -1 },
                { "Button_NAV1^_DCT", -1 },
                { "Button_NAV1^_MENU", -1 },
                { "Button_NAV1^_CLR", -1 },
                { "Button_NAV1^_ENT", -1 },
                { "Button_NAV1^_CDI", 106 },
                { "Button_NAV1^_OBS", 107 },
                { "Button_NAV1^_MSG", 108 },
                { "Button_NAV1^_FPL", 109 },
                { "Button_NAV1^_VNAV", 110 },
                { "Button_NAV1^_PROC", 111 },
                { "Button_NAV2^_OI", 176 },
                { "Button_NAV2^_OD", 177 },
                { "Button_NAV2^_II", 178 },
                { "Button_NAV2^_ID", 179 },
                { "Button_NAV2^_TOG", 180 },
                { "Button_NAV2^_CRSR", -1 },
                { "Button_NAV2^_DCT", -1 },
                { "Button_NAV2^_MENU", -1 },
                { "Button_NAV2^_CLR", -1 },
                { "Button_NAV2^_ENT", -1 },
                { "Button_NAV2^_CDI", 106 },
                { "Button_NAV2^_OBS", 107 },
                { "Button_NAV2^_MSG", 108 },
                { "Button_NAV2^_FPL", 109 },
                { "Button_NAV2^_VNAV", 110 },
                { "Button_NAV2^_PROC", 111 },
                { "Button_FMS1^_OI", -1 },
                { "Button_FMS1^_OD", -1 },
                { "Button_FMS1^_II", -1 },
                { "Button_FMS1^_ID", -1 },
                { "Button_FMS1^_TOG", -1 },
                { "Button_FMS1^_CRSR", -1 },
                { "Button_FMS1^_DCT", -1 },
                { "Button_FMS1^_MENU", -1 },
                { "Button_FMS1^_CLR", -1 },
                { "Button_FMS1^_ENT", -1 },
                { "Button_FMS1^_CDI", -1 },
                { "Button_FMS1^_OBS", -1 },
                { "Button_FMS1^_MSG", -1 },
                { "Button_FMS1^_FPL", -1 },
                { "Button_FMS1^_VNAV", -1 },
                { "Button_FMS1^_PROC", -1 },
                { "Button_FMS2^_OI", -1 },
                { "Button_FMS2^_OD", -1 },
                { "Button_FMS2^_II", -1 },
                { "Button_FMS2^_ID", -1 },
                { "Button_FMS2^_TOG", -1 },
                { "Button_FMS2^_CRSR", -1 },
                { "Button_FMS2^_DCT", -1 },
                { "Button_FMS2^_MENU", -1 },
                { "Button_FMS2^_CLR", -1 },
                { "Button_FMS2^_ENT", -1 },
                { "Button_FMS2^_CDI", -1 },
                { "Button_FMS2^_OBS", -1 },
                { "Button_FMS2^_MSG", -1 },
                { "Button_FMS2^_FPL", -1 },
                { "Button_FMS2^_VNAV", -1 },
                { "Button_FMS2^_PROC", -1 },
                { "Button_AP^_OI", 224 },
                { "Button_AP^_OD", 225 },
                { "Button_AP^_II", 226 },
                { "Button_AP^_ID", 227 },
                { "Button_AP^_TOG", 228 },
                { "Button_AP^_CRSR", -1 },
                { "Button_AP^_DCT", -1 },
                { "Button_AP^_MENU", -1 },
                { "Button_AP^_CLR", -1 },
                { "Button_AP^_ENT", -1 },
                { "Button_AP^_CDI", 106 },
                { "Button_AP^_OBS", 107 },
                { "Button_AP^_MSG", 108 },
                { "Button_AP^_FPL", 109 },
                { "Button_AP^_VNAV", 110 },
                { "Button_AP^_PROC", 111 },
                { "Button_XPDR^_OI", 240 },
                { "Button_XPDR^_OD", 241 },
                { "Button_XPDR^_II", 242 },
                { "Button_XPDR^_ID", 243 },
                { "Button_XPDR^_TOG", 244 },
                { "Button_XPDR^_CRSR", -1 },
                { "Button_XPDR^_DCT", -1 },
                { "Button_XPDR^_MENU", -1 },
                { "Button_XPDR^_CLR", -1 },
                { "Button_XPDR^_ENT", -1 },
                { "Button_XPDR^_CDI", 106 },
                { "Button_XPDR^_OBS", 107 },
                { "Button_XPDR^_MSG", 108 },
                { "Button_XPDR^_FPL", 109 },
                { "Button_XPDR^_VNAV", 110 },
                { "Button_XPDR^_PROC", 111 },
            };

            HIDEventAssignments = new Dictionary<uint, uint>()
            {
                { (uint)OctaviReport.OctaviButton.HID_BTN_TOG, 4 },
                { (uint)OctaviReport.OctaviButton.HID_ENC_SW, 5 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_DCT, 6 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_MENU, 7 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_CLR, 8 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_ENT, 9 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_AP, 10 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_AP_HDG, 11 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_AP_NAV, 12 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_AP_APR, 13 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_AP_ALT, 14 },
                { (uint)OctaviReport.OctaviButton.HID_BTN_AP_BC, 15 },
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
    public List<int> toButton(OctaviReport report)
        {
            List<int> ButtonPresses = new List<int>();
            uint pressed = report.buttonState & ~lastReport.buttonState; // only rising edges
            byte extContentState = report.contextState;

            if(report.contextState != lastReport.contextState)
            {
                shiftState = false; // reset shift mode on context change
            }

            if (shiftState) extContentState += 8; // shift to second half of button events if shift is active

            // To Do: Replace contextState with extContentState in next block?
            // To Do: Translate to 
            if (report.incrCoarse != 0 || report.incrFine != 0)
                if (report.incrCoarse > 0) for (int i = 0; i < report.incrCoarse; i++) ButtonPresses.Add(OctaviButtonMatrix.ElementAt(extContentState * 16 + 0).Value);
                else if (report.incrCoarse < 0) for (int i = 0; i > report.incrCoarse; i--) ButtonPresses.Add(OctaviButtonMatrix.ElementAt(extContentState * 16 + 1).Value);
                if (report.incrFine > 0) for (int i = 0; i < report.incrFine; i++) ButtonPresses.Add(OctaviButtonMatrix.ElementAt(extContentState * 16 + 2).Value);
                else if (report.incrFine < 0) for (int i = 0; i > report.incrFine; i--) ButtonPresses.Add(OctaviButtonMatrix.ElementAt(extContentState * 16 + 3).Value);

            if((pressed & (uint)OctaviReport.OctaviButton.HID_ENC_SW)!=0)
            {
                //if (report.contextState != (byte)OctaviReport.OctaviState.STATE_FMS1 && report.contextState != (byte)OctaviReport.OctaviState.STATE_FMS2 && report.contextState != (byte)OctaviReport.OctaviState.STATE_NAV1 && report.contextState != (byte)OctaviReport.OctaviState.STATE_NAV2)
                if (report.contextState != (byte)OctaviReport.OctaviState.STATE_FMS1 && report.contextState != (byte)OctaviReport.OctaviState.STATE_FMS2)
                {
                        shiftState = !shiftState; // FMS1&2 do not have shift modes for now, sorry
                }
            }
            foreach(uint HIDEvent in HIDEventAssignments.Keys)
            {
                if ((pressed & HIDEvent) != 0)
                {
                    int ButtonPress = extContentState * 16 + (int)HIDEventAssignments[HIDEvent]; // find "full matrix" event index
                    ButtonPress = OctaviButtonMatrix.ElementAt(ButtonPress).Value; // translate to existing Octavi "devices" in MF
                    if (ButtonPress >= 0) ButtonPresses.Add(ButtonPress); // if not "unassigned" (-1), then press the button!
                }
            }
            
            lastReport = report;
        return ButtonPresses;
        }
    }
}
