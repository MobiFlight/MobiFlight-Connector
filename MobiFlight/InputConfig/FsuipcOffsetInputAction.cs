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
        public int Offset { get; set; }
        [XmlAttribute]
        public byte Size { get; set; }
        [XmlAttribute]
        public FSUIPCOffsetType OffsetType { get; set; }
        [XmlAttribute]
        public long Mask { get; set; }
        [XmlAttribute]
        public bool BcdMode { get; set; }
        [XmlAttribute]
        public String Value { get; set; }
        [XmlAttribute]
        public Transformation Transform { get; set; }

        public const String TYPE = "FsuipcOffsetInputAction";

        public FsuipcOffsetInputAction()
        {
            Offset = FSUIPCOffsetNull;
            Mask = 0xFF;
            OffsetType = FSUIPCOffsetType.Integer;
            Size = 1;
            BcdMode = false;
            Value = "";
            Transform = new Transformation();
        }

        override public object Clone()
        {
            FsuipcOffsetInputAction clone = new FsuipcOffsetInputAction();
            clone.Offset = this.Offset;
            clone.OffsetType = this.OffsetType;
            clone.Size = this.Size;
            clone.Mask = this.Mask;
            clone.BcdMode = this.BcdMode;
            clone.Value = this.Value;
            clone.Transform = (Transformation) this.Transform.Clone();
            return clone;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteStartElement("source");
                writer.WriteAttributeString("type", "FSUIPC");
                writer.WriteAttributeString("offset", "0x" + Offset.ToString("X4"));
                writer.WriteAttributeString("offsetType", OffsetType.ToString());
                writer.WriteAttributeString("size", Size.ToString());
                writer.WriteAttributeString("mask", "0x" + Mask.ToString("X4"));
                writer.WriteAttributeString("bcdMode", BcdMode.ToString());
                writer.WriteAttributeString("inputValue", Value);
            writer.WriteEndElement();
            // TODO: write and read the transform
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read(); // this should be the "source"

            if (reader.LocalName == "source")
            {
                Offset = Int32.Parse(reader["offset"].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                Size = Byte.Parse(reader["size"]);
                if (reader["offsetType"] != null && reader["offsetType"] != "")
                {
                    try
                    {
                        OffsetType = (FSUIPCOffsetType)Enum.Parse(typeof(FSUIPCOffsetType), reader["offsetType"]);
                    }
                    catch (Exception e)
                    {
                        OffsetType = MobiFlight.FSUIPCOffsetType.Integer;
                    }
                }
                else
                {
                    // Backward compatibility
                    // byte 1,2,4 -> int, this already is default
                    // exception
                    // byte 8 -> float
                    if (Size == 8) OffsetType = MobiFlight.FSUIPCOffsetType.Float;
                }
                Mask = Int64.Parse(reader["mask"].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

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
                    BcdMode = Boolean.Parse(reader["bcdMode"]);
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
            reader.Read();
        }   

        public override void execute(FSUIPC.FSUIPCCacheInterface cache, SimConnectMSFS.SimConnectCacheInterface simConnectCache, MobiFlightCacheInterface moduleCache, List<ConfigRefValue> configRefs)
        {
            String value = Value;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();
            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            if (value.Contains("$"))
            {
                ConnectorValue tmpValue = FSUIPC.FsuipcHelper.executeRead(this, cache as FSUIPC.FSUIPCCacheInterface);
                Tuple<string, string> replacement = new Tuple<string, string>("$", tmpValue.ToString());
                replacements.Add(replacement);
            }

            value = Replace(value, replacements);

            if (Size == 1)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                byte bValue = Byte.Parse(value, format);
                if (Mask != 0xFF)
                {
                    byte cByte = (byte)cache.getValue(Offset, Size);
                    if (bValue == 1)
                    {
                        bValue = (byte)(cByte | Mask);
                    }
                    else
                    {
                        bValue = (byte)(cByte & ~Mask);
                    }
                }
                cache.setOffset(Offset, bValue);
            }
            else if (Size == 2)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                Int16 sValue = Int16.Parse(value, format);
                if (Mask != 0xFFFF)
                {
                    Int16 cByte = (Int16)cache.getValue(Offset, Size);
                    if (sValue == 1)
                    {
                        sValue = (Int16)(cByte | Mask);
                    }
                    else
                    {
                        sValue = (Int16)(cByte & ~Mask);
                    }
                }

                cache.setOffset(Offset, sValue);
            }
            else if (Size == 4)
            {
                if (OffsetType == FSUIPCOffsetType.Integer)
                {
                    Int32 iValue = Int32.Parse(value);
                    if (Mask != 0xFFFFFFFF)
                    {
                        Int32 cByte = (Int32)cache.getValue(Offset, Size);
                        if (iValue == 1)
                        {
                            iValue = (Int32)(cByte | Mask);
                        }
                        else
                        {
                            iValue = (Int32)(cByte & ~Mask);
                        }
                    }

                    cache.setOffset(Offset, iValue);
                }
                else if (OffsetType == FSUIPCOffsetType.Float)
                {
                    float fValue = float.Parse(value);
                    if (Mask != 0xFFFFFFFF)
                    {
                        new Exception("Float Inputs don't accept masked values.");
                    }

                    cache.setOffset(Offset, fValue);
                }
            }
            else if (Size == 8)
            {
                if (OffsetType == FSUIPCOffsetType.Float)
                {
                    double fValue = double.Parse(value);
                    //if (FSUIPCMask != 0xFFFFFFFF)
                    //{
                    //    new Exception("Float Inputs don't accept masked values.");
                    //}

                    cache.setOffset(Offset, fValue);
                }
            }
            else if (Size == 255)
            {
                cache.setOffset(Offset, Value);
            }

            cache.Write();
        }
    }
}
