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
        public bool FSUIPCBcdMode { get; set; }
        [XmlAttribute]
        public String Value { get; set; }
        [XmlAttribute]
        public Transformation Transform { get; set; }

        public const String TYPE = "FsuipcOffsetInputAction";

        public FsuipcOffsetInputAction()
        {
            FSUIPCOffset = FSUIPCOffsetNull;
            FSUIPCMask = 0xFF;
            FSUIPCOffsetType = FSUIPCOffsetType.Integer;
            FSUIPCSize = 1;
            FSUIPCBcdMode = false;
            Value = "";
            Transform = new Transformation();
        }

        override public object Clone()
        {
            FsuipcOffsetInputAction clone = new FsuipcOffsetInputAction();
            clone.FSUIPCOffset = this.FSUIPCOffset;
            clone.FSUIPCOffsetType = this.FSUIPCOffsetType;
            clone.FSUIPCSize = this.FSUIPCSize;
            clone.FSUIPCMask = this.FSUIPCMask;
            clone.FSUIPCBcdMode = this.FSUIPCBcdMode;
            clone.Value = this.Value;
            clone.Transform = (Transformation) this.Transform.Clone();
            return clone;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteStartElement("source");
                writer.WriteAttributeString("type", "FSUIPC");
                writer.WriteAttributeString("offset", "0x" + FSUIPCOffset.ToString("X4"));
                writer.WriteAttributeString("offsetType", FSUIPCOffsetType.ToString());
                writer.WriteAttributeString("size", FSUIPCSize.ToString());
                writer.WriteAttributeString("mask", "0x" + FSUIPCMask.ToString("X4"));
                writer.WriteAttributeString("bcdMode", FSUIPCBcdMode.ToString());
                writer.WriteAttributeString("inputValue", Value);
            writer.WriteEndElement();
            // TODO: write and read the transform
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

                // backward compatibility
                if (reader["multiplier"] != null)
                {
                    double multiplier = Double.Parse(reader["multiplier"], serializationCulture);
                    if (multiplier != 1.0)
                        try
                        {
                            Transform.Expression = "$*" + multiplier;
                        }
                        catch (Exception e)
                        {
                            Log.Instance.log("Error on converting multiplier.", LogSeverity.Error);
                        }
                }

                if (reader["bcdMode"] != null && reader["bcdMode"] != "")
                {
                    FSUIPCBcdMode = Boolean.Parse(reader["bcdMode"]);
                }

                if (reader["inputValue"] != null && reader["inputValue"] != "")
                {
                    Value = reader["inputValue"];
                }

                // read to the end not needed
                if (reader.LocalName == "transformation")
                {
                    Transform.ReadXml(reader);
                }
            }
        }   

        public override void execute(FSUIPC.FSUIPCCacheInterface cache, MobiFlightCacheInterface moduleCache)
        {
            String value = Value;
            // apply ncalc logic
            if (value.Contains("$"))
            {
                ConnectorValue tmpValue = FSUIPC.FsuipcHelper.executeRead(this, cache);

                String expression = Value.Replace("$", tmpValue.ToString());
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

            if (FSUIPCSize == 1)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (FSUIPCBcdMode) format = System.Globalization.NumberStyles.HexNumber;
                byte bValue = Byte.Parse(value, format);
                if (FSUIPCMask != 0xFF)
                {
                    byte cByte = (byte)cache.getValue(FSUIPCOffset, FSUIPCSize);
                    if (bValue == 1)
                    {
                        bValue = (byte)(cByte | FSUIPCMask);
                    }
                    else
                    {
                        bValue = (byte)(cByte & ~FSUIPCMask);
                    }
                }
                cache.setOffset(FSUIPCOffset, bValue);
            }
            else if (FSUIPCSize == 2)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (FSUIPCBcdMode) format = System.Globalization.NumberStyles.HexNumber;
                Int16 sValue = Int16.Parse(value, format);
                if (FSUIPCMask != 0xFFFF)
                {
                    Int16 cByte = (Int16)cache.getValue(FSUIPCOffset, FSUIPCSize);
                    if (sValue == 1)
                    {
                        sValue = (Int16)(cByte | FSUIPCMask);
                    }
                    else
                    {
                        sValue = (Int16)(cByte & ~FSUIPCMask);
                    }
                }

                cache.setOffset(FSUIPCOffset, sValue);
            }
            else if (FSUIPCSize == 4)
            {
                if (FSUIPCOffsetType == FSUIPCOffsetType.Integer)
                {
                    Int32 iValue = Int32.Parse(value);
                    if (FSUIPCMask != 0xFFFFFFFF)
                    {
                        Int32 cByte = (Int32)cache.getValue(FSUIPCOffset, FSUIPCSize);
                        if (iValue == 1)
                        {
                            iValue = (Int32)(cByte | FSUIPCMask);
                        }
                        else
                        {
                            iValue = (Int32)(cByte & ~FSUIPCMask);
                        }
                    }

                    cache.setOffset(FSUIPCOffset, iValue);
                }
                else if (FSUIPCOffsetType == FSUIPCOffsetType.Float)
                {
                    float fValue = float.Parse(value);
                    if (FSUIPCMask != 0xFFFFFFFF)
                    {
                        new Exception("Float Inputs don't accept masked values.");
                    }

                    cache.setOffset(FSUIPCOffset, fValue);
                }
            }
            else if (FSUIPCSize == 8)
            {
                if (FSUIPCOffsetType == FSUIPCOffsetType.Float)
                {
                    double fValue = double.Parse(value);
                    //if (FSUIPCMask != 0xFFFFFFFF)
                    //{
                    //    new Exception("Float Inputs don't accept masked values.");
                    //}

                    cache.setOffset(FSUIPCOffset, fValue);
                }
            }
            else if (FSUIPCSize == 255)
            {
                cache.setOffset(FSUIPCOffset, Value);
            }

            cache.Write();
        }
    }
}
