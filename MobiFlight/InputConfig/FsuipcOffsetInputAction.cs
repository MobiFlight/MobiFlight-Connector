using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MobiFlight;

namespace MobiFlight.InputConfig
{
    public class FsuipcOffsetInputAction : InputAction, IFsuipcConfigItem, ICloneable
    {
        public const int FSUIPCOffsetNull = 0;
        [XmlAttribute]
        public int FSUIPCOffset { get; set; }
        [XmlAttribute]
        public byte FSUIPCSize { get; set; }
        [XmlAttribute]
        public FSUIPCOffsetType FSUIPCOffsetType { get; set; }
        [XmlAttribute]
        public long FSUIPCMask { get; set; }
        [XmlAttribute]
        public double FSUIPCMultiplier { get; set; }
        [XmlAttribute]
        public bool FSUIPCBcdMode { get; set; }
        [XmlAttribute]
        public String InputValue { get; set; }


        public FsuipcOffsetInputAction()
        {
            FSUIPCOffset = FSUIPCOffsetNull;
            FSUIPCMask = 0xFF;
            FSUIPCMultiplier = 1.0;
            FSUIPCOffsetType = FSUIPCOffsetType.Integer;
            FSUIPCSize = 1;
            FSUIPCBcdMode = false;
            InputValue = "";
        }

        override public object Clone()
        {
            FsuipcOffsetInputAction clone = new FsuipcOffsetInputAction();
            clone.FSUIPCOffset = this.FSUIPCOffset;
            clone.FSUIPCOffsetType = this.FSUIPCOffsetType;
            clone.FSUIPCSize = this.FSUIPCSize;
            clone.FSUIPCMask = this.FSUIPCMask;
            clone.FSUIPCMultiplier = this.FSUIPCMultiplier;
            clone.FSUIPCBcdMode = this.FSUIPCBcdMode;
            clone.InputValue = this.InputValue;
            return clone;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", "FsuipcOffsetInputAction");
            writer.WriteStartElement("source");
                writer.WriteAttributeString("type", "FSUIPC");
                writer.WriteAttributeString("offset", "0x" + FSUIPCOffset.ToString("X4"));
                writer.WriteAttributeString("offsetType", FSUIPCOffsetType.ToString());
                writer.WriteAttributeString("size", FSUIPCSize.ToString());
                writer.WriteAttributeString("mask", "0x" + FSUIPCMask.ToString("X4"));
                writer.WriteAttributeString("multiplier", FSUIPCMultiplier.ToString(serializationCulture));
                writer.WriteAttributeString("bcdMode", FSUIPCBcdMode.ToString());
                writer.WriteAttributeString("inputValue", InputValue);
            writer.WriteEndElement();
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read(); // this should be the "source"

            if (reader.LocalName == "source")
            {
                FSUIPCOffset = Int32.Parse(reader["offset"].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                FSUIPCSize = Byte.Parse(reader["size"]);
                if (reader["offsetType"] != null && reader["offsetType"] != "")
                {
                    try
                    {
                        FSUIPCOffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), reader["offsetType"]);
                    }
                    catch (Exception e)
                    {
                        FSUIPCOffsetType = MobiFlight.FSUIPCOffsetType.Integer;
                    }
                }
                else
                {
                    // Backward compatibility
                    // byte 1,2,4 -> int, this already is default
                    // exception
                    // byte 8 -> float
                    if (FSUIPCSize == 8) FSUIPCOffsetType = MobiFlight.FSUIPCOffsetType.Float;
                }
                FSUIPCMask = Int64.Parse(reader["mask"].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                FSUIPCMultiplier = Double.Parse(reader["multiplier"], serializationCulture);
                if (reader["bcdMode"] != null && reader["bcdMode"] != "")
                {
                    FSUIPCBcdMode = Boolean.Parse(reader["bcdMode"]);
                }

                if (reader["inputValue"] != null && reader["inputValue"] != "")
                {
                    InputValue = reader["inputValue"];
                }

                // read to the end not needed
            }
        }

        public override void execute(Fsuipc2Cache fsuipcCache)
        {
            String value = InputValue;
            // apply ncalc logic
            if (value.Contains("$"))
            {
                ConnectorValue tmpValue = MobiFlight.FSUIPC.FsuipcHelper.executeRead(this, fsuipcCache);

                String expression = InputValue.Replace("$", tmpValue.ToString());
                var ce = new NCalc.Expression(expression);
                try
                {
                    value = (ce.Evaluate()).ToString();
                }
                catch
                {
                    Log.Instance.log("checkPrecondition : Exception on NCalc evaluate", LogSeverity.Warn);
                    throw new Exception(MainForm._tr("uiMessageErrorOnParsingExpression"));
                }
            }

            if (FSUIPCSize == 1) {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (FSUIPCBcdMode) format = System.Globalization.NumberStyles.HexNumber;
                byte bValue = Byte.Parse(value, format);
                if (FSUIPCMask != 0xFF)
                {
                    byte cByte = (byte)fsuipcCache.getValue(FSUIPCOffset, FSUIPCSize);
                    if (bValue == 1)
                    {
                        bValue = (byte)(cByte | FSUIPCMask);
                    }
                    else
                    {
                        bValue = (byte)(cByte & ~FSUIPCMask);
                    }
                }
                fsuipcCache.setOffset(FSUIPCOffset, bValue);
            }
            else if (FSUIPCSize == 2)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (FSUIPCBcdMode) format = System.Globalization.NumberStyles.HexNumber;
                Int16 sValue = Int16.Parse(value, format);
                if (FSUIPCMask != 0xFFFF)
                {
                    Int16 cByte = (Int16) fsuipcCache.getValue(FSUIPCOffset, FSUIPCSize);
                    if (sValue == 1)
                    {
                        sValue = (Int16)(cByte | FSUIPCMask);
                    }
                    else
                    {
                        sValue = (Int16)(cByte & ~FSUIPCMask);
                    }
                }
                
                fsuipcCache.setOffset(FSUIPCOffset, sValue);
            }
            else if (FSUIPCSize == 4)
            {
                Int32 iValue = Int32.Parse(value);
                if (FSUIPCMask != 0xFFFFFFFF)
                {
                    Int32 cByte = (Int32)fsuipcCache.getValue(FSUIPCOffset, FSUIPCSize);
                    if (iValue == 1)
                    {
                        iValue = (Int32)(cByte | FSUIPCMask);
                    }
                    else
                    {
                        iValue = (Int32)(cByte & ~FSUIPCMask);
                    }
                }

                fsuipcCache.setOffset(FSUIPCOffset, iValue);
            }
        }
    }
}
