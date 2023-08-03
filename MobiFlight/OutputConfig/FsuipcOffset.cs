using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class FsuipcOffset
    {
        public const int OffsetNull = 0;
        public int Offset { get; set; }
        public byte Size { get; set; }
        public FSUIPCOffsetType
                            OffsetType
        { get; set; }
        public long Mask { get; set; }

        public bool BcdMode { get; set; }

        public FsuipcOffset()
        {
            Offset        = OffsetNull;
            Mask          = 0xFF;
            OffsetType    = FSUIPCOffsetType.Integer;
            Size          = 1;
            BcdMode       = false;
        }

        internal void ReadXml(XmlReader reader)
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
                    OffsetType = FSUIPCOffsetType.Integer;
                }
            }
            else
            {
                // Backward compatibility
                // byte 1,2,4 -> int, this already is default
                // exception
                // byte 8 -> float
                if (Size == 8) OffsetType = FSUIPCOffsetType.Float;
            }
            Mask = Int64.Parse(reader["mask"].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);



            if (reader["bcdMode"] != null && reader["bcdMode"] != "")
            {
                BcdMode = Boolean.Parse(reader["bcdMode"]);
            }
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", "FSUIPC");
            writer.WriteAttributeString("offset", "0x" + Offset.ToString("X4"));
            writer.WriteAttributeString("offsetType", OffsetType.ToString());
            writer.WriteAttributeString("size", Size.ToString());
            writer.WriteAttributeString("mask", "0x" + Mask.ToString("X4"));
            writer.WriteAttributeString("bcdMode", BcdMode.ToString());
        }

        public override bool Equals(object obj)
        {
            return (
                obj != null && obj is FsuipcOffset &&
                this.Offset == (obj as FsuipcOffset).Offset &&
                this.Size == (obj as FsuipcOffset).Size &&
                this.OffsetType == (obj as FsuipcOffset).OffsetType &&
                this.Mask == (obj as FsuipcOffset).Mask &&
                this.BcdMode == (obj as FsuipcOffset).BcdMode
            );
        }

        public object Clone()
        {
            return new FsuipcOffset()
            {
                Offset = this.Offset,
                Mask = this.Mask,
                OffsetType = this.OffsetType,
                Size = this.Size,
                BcdMode = this.BcdMode
            };
        }
    }
}
