using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using MobiFlight.FSUIPC;
using MobiFlight.VJoy;

namespace MobiFlight.InputConfig
{
    public class VJoyInputAction : InputAction
    {
        new public const string Label = "MobiFlight - Virtual Joystick input (vJoy)";
        public const String TYPE = "vJoyInputAction";

        public uint vJoyID;
        public int buttonNr;
        public String axisString;
        public bool buttonComand;
        public String sendValue;

        public override object Clone()
        {
            VJoyInputAction clone = new VJoyInputAction();
            clone.axisString = axisString;
            clone.buttonNr = buttonNr;
            clone.vJoyID = vJoyID;
            clone.buttonComand = buttonComand;
            clone.sendValue = sendValue;

            return clone;
        }

        public override void execute(
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            String value = sendValue;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();

            if (value != null && value.Contains("@"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("@", args.Value.ToString());
                replacements.Add(replacement);
            }

            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            value = Replace(value, replacements);

            if (buttonNr > 0)
            {
                if (VJoyHelper.sendButton(vJoyID,UInt16.Parse(buttonNr.ToString()),buttonComand))
                {
                    Log.Instance.log("sending Btn Nr:" + buttonNr + " ID:" + vJoyID + " State" + buttonComand, LogSeverity.Debug);
                }
                else
                {
                    Log.Instance.log("ERROR sending Btn Nr:" + buttonNr + " ID:" + vJoyID + " State" + buttonComand, LogSeverity.Error);
                }
                return;
            }

            if (axisString != "--")
            {
                UInt16 vJoyIntValue = (UInt16)Math.Round(Double.Parse(value));
                if (VJoyHelper.setAxisVal(vJoyID,axisString,vJoyIntValue))
                {
                    Log.Instance.log("set Axis:" + axisString + " ID:" + vJoyID + " to " + vJoyIntValue, LogSeverity.Debug);
                }
                else
                {
                    Log.Instance.log("ERROR set Axis:" + axisString + " ID:" + vJoyID + " to " + vJoyIntValue, LogSeverity.Error);
                }
                return;
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            String xvJoyId = reader["vJoyId"];
            String xbuttenNr = reader["buttonNr"];
            String xbuttenCmd = reader["buttonCmd"];
            String xaxisString = reader["Axis"];
            String xaxisVal = reader["axisVal"];

            buttonNr = Int16.Parse(xbuttenNr);
            vJoyID = UInt16.Parse(xvJoyId);
            buttonComand = bool.Parse(xbuttenCmd);
            axisString = xaxisString;
            sendValue = xaxisVal;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("vJoyId", vJoyID.ToString());
            writer.WriteAttributeString("buttonNr", buttonNr.ToString());
            writer.WriteAttributeString("buttonCmd", buttonComand.ToString());
            writer.WriteAttributeString("Axis",axisString);
            writer.WriteAttributeString("axisVal", sendValue);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is VJoyInputAction &&
                axisString == (obj as VJoyInputAction).axisString &&
                 buttonNr == (obj as VJoyInputAction).buttonNr &&
                 vJoyID == (obj as VJoyInputAction).vJoyID &&
                 buttonComand == (obj as VJoyInputAction).buttonComand &&
                 sendValue == (obj as VJoyInputAction).sendValue;
        }
    }
}
