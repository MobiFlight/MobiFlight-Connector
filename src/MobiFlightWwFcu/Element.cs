using System;
using System.Collections.Generic;
using System.Text;

namespace MobiFlightWwFcu
{
    internal class Element
    {
        public Bit[] Bits { get;  }
        public bool IsSevenSegment { get; }
        
        // For 7 segment it expects: 
        // top, topright, bottomright, bottom, bottomleft, topleft, middle
        public Element(Bit[] bits, bool isSevenSegment)
        {
            Bits = bits;
            IsSevenSegment = isSevenSegment;                
        }

        public Element(Bit bit)
        {            
            Bits = new Bit[] { bit };
            IsSevenSegment = false;
        }


        // Constructor for PAP3 or PAC encoding
        public Element(int topByte, int bitNumber, char initChar = '*')
        {
            List<Bit> bits = new List<Bit>();
            for (int i = 0; i < 7; i++)
            {
                bits.Add(new Bit(topByte - i*4, bitNumber));
            }
            Bits = bits.ToArray();
            IsSevenSegment = true;
            SetCharacter(initChar);
        }

        public void SetCharacter(char c)
        {
            if (IsSevenSegment)
            {
                if (WinwingConstants.CharacterDict.TryGetValue(c, out bool[] values))
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        Bits[i].Value = values[i];
                    }
                }
            }
        }

        public void SetValue(bool value)
        {
            foreach (var b in Bits)
            {
                b.Value = value;
            }
        }
    }
}
