using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MobiFlight;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;

namespace MobiFlight.InputConfig
{
    public class FsuipcOffsetInputAction : InputAction, IFsuipcConfigItem, ICloneable
    {
        new public const String Label = "FSUIPC - Offset";
        public FsuipcOffset FSUIPC { get; set; }
        public String Value { get; set; }
        public Transformation Transform { get; set; }

        public const String TYPE = "FsuipcOffsetInputAction";

        public FsuipcOffsetInputAction()
        {
            FSUIPC = new FsuipcOffset();
            Value = "";
            Transform = new Transformation();
        }

        override public object Clone()
        {
            FsuipcOffsetInputAction clone = new FsuipcOffsetInputAction();
            clone.FSUIPC = this.FSUIPC.Clone() as FsuipcOffset;
            clone.Value = this.Value;
            clone.Transform = (Transformation) this.Transform.Clone();
            return clone;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteStartElement("source");
                FSUIPC.WriteXml(writer);
                writer.WriteAttributeString("inputValue", Value);
            writer.WriteEndElement();
            // TODO: write and read the transform
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read(); // this should be the "source"

            if (reader.LocalName == "source")
            {
                FSUIPC.ReadXml(reader);

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

        public override void execute(
            CacheCollection cacheCollection, 
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            String value = Value;
            FSUIPC.FSUIPCCacheInterface cache = cacheCollection.fsuipcCache;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();
            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            if (value.Contains("@"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("@", args.Value.ToString());
                replacements.Add(replacement);
            }

            if (value.Contains("$"))
            {
                ConnectorValue tmpValue = MobiFlight.FSUIPC.FsuipcHelper.executeRead(this, cache as FSUIPC.FSUIPCCacheInterface);
                Tuple<string, string> replacement = new Tuple<string, string>("$", tmpValue.ToString());
                replacements.Add(replacement);
            }

            value = Replace(value, replacements);

            if (FSUIPC.Size == 1)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (FSUIPC.BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                byte bValue = Byte.Parse(value, format);
                if (FSUIPC.Mask != 0xFF)
                {
                    byte cByte = (byte)cache.getValue(FSUIPC.Offset, FSUIPC.Size);
                    if (bValue == 1)
                    {
                        bValue = (byte)(cByte | FSUIPC.Mask);
                    }
                    else
                    {
                        bValue = (byte)(cByte & ~FSUIPC.Mask);
                    }
                }
                cache.setOffset(FSUIPC.Offset, bValue);
            }
            else if (FSUIPC.Size == 2)
            {
                System.Globalization.NumberStyles format = System.Globalization.NumberStyles.Integer;
                if (FSUIPC.BcdMode) format = System.Globalization.NumberStyles.HexNumber;
                Int16 sValue = Int16.Parse(Math.Floor(double.Parse(value)).ToString(), format);
                if (FSUIPC.Mask != 0xFFFF)
                {
                    Int16 cByte = (Int16)cache.getValue(FSUIPC.Offset, FSUIPC.Size);
                    if (sValue == 1)
                    {
                        sValue = (Int16)(cByte | FSUIPC.Mask);
                    }
                    else
                    {
                        sValue = (Int16)(cByte & ~FSUIPC.Mask);
                    }
                }

                cache.setOffset(FSUIPC.Offset, sValue);
            }
            else if (FSUIPC.Size == 4)
            {
                if (FSUIPC.OffsetType == FSUIPCOffsetType.Integer)
                {
                    Int32 iValue = Int32.Parse(Math.Floor(double.Parse(value)).ToString());
                    if (FSUIPC.Mask != 0xFFFFFFFF)
                    {
                        Int32 cByte = (Int32)cache.getValue(FSUIPC.Offset, FSUIPC.Size);
                        if (iValue == 1)
                        {
                            iValue = (Int32)(cByte | FSUIPC.Mask);
                        }
                        else
                        {
                            iValue = (Int32)(cByte & ~FSUIPC.Mask);
                        }
                    }

                    cache.setOffset(FSUIPC.Offset, iValue);
                }
                else if (FSUIPC.OffsetType == FSUIPCOffsetType.Float)
                {
                    float fValue = float.Parse(value);
                    if (FSUIPC.Mask != 0xFFFFFFFF)
                    {
                        new Exception("Float Inputs don't accept masked values.");
                    }

                    cache.setOffset(FSUIPC.Offset, fValue);
                }
            }
            else if (FSUIPC.Size == 8)
            {
                if (FSUIPC.OffsetType == FSUIPCOffsetType.Float)
                {
                    double fValue = double.Parse(value);
                    //if (FSUIPCMask != 0xFFFFFFFF)
                    //{
                    //    new Exception("Float Inputs don't accept masked values.");
                    //}

                    cache.setOffset(FSUIPC.Offset, fValue);
                }
            }
            else if (FSUIPC.Size == 255)
            {
                cache.setOffset(FSUIPC.Offset, Value);
            }

            cache.Write();
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is FsuipcOffsetInputAction &&
                FSUIPC.Equals((obj as FsuipcOffsetInputAction).FSUIPC) &&
                Value == (obj as FsuipcOffsetInputAction).Value &&
                Transform.Equals((obj as FsuipcOffsetInputAction).Transform);
        }
    }
}
