using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MobiFlightMoza.Cdu
{
    internal readonly struct CduCell
    {
        public static readonly CduCell Blank = new(' ', 'w', false, false);

        public char Character { get; }
        public char ColorLetter { get; }
        public bool IsSmall { get; }
        public bool IsInverted { get; }

        public CduCell(char character, char colorLetter, bool isSmall, bool isInverted)
        {
            Character = character;
            ColorLetter = colorLetter;
            IsSmall = isSmall;
            IsInverted = isInverted;
        }
    }

    /// <summary>
    /// The 14x24 MCDU screen grid, and the parser for the <c>{"Target":"Display","Data":[...]}</c>
    /// JSON the CDU websocket hub broadcasts. Vendor-neutral - has no idea MOZA exists.
    /// </summary>
    internal sealed class CduPage
    {
        public const int Rows = 14;
        public const int Columns = 24;

        private readonly CduCell[] Cells = new CduCell[Rows * Columns];

        public CduCell this[int row, int column]
        {
            get => Cells[row * Columns + column];
            set => Cells[row * Columns + column] = value;
        }

        public static CduPage CreateBlank()
        {
            var page = new CduPage();
            for (int i = 0; i < page.Cells.Length; i++)
            {
                page.Cells[i] = CduCell.Blank;
            }
            return page;
        }

        public string GetRowText(int row)
        {
            char[] chars = new char[Columns];
            for (int c = 0; c < Columns; c++)
            {
                chars[c] = this[row, c].Character;
            }
            return new string(chars);
        }

        public bool RowEquals(CduPage other, int row)
        {
            for (int c = 0; c < Columns; c++)
            {
                var a = this[row, c];
                var b = other[row, c];
                if (a.Character != b.Character || a.ColorLetter != b.ColorLetter
                    || a.IsSmall != b.IsSmall || a.IsInverted != b.IsInverted)
                {
                    return false;
                }
            }
            return true;
        }

        // Tolerant of short/long Data arrays and of 3- or 4-element cells; a malformed
        // JSON document is the only thing that fails outright.
        public static bool TryParse(string json, out CduPage page, out string error)
        {
            page = null;
            error = null;
            try
            {
                var root = JObject.Parse(json);
                if (root["Data"] is not JArray data)
                {
                    error = "Missing \"Data\" array.";
                    return false;
                }

                var result = CreateBlank();
                int count = Math.Min(data.Count, Rows * Columns);
                for (int i = 0; i < count; i++)
                {
                    result[i / Columns, i % Columns] = ParseCell(data[i]);
                }
                page = result;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static CduCell ParseCell(JToken token)
        {
            if (!token.HasValues) return CduCell.Blank;

            char character = token[0].Value<char>();
            char colorLetter = token.Count() > 1 ? token[1].Value<char>() : 'w';
            bool isSmall = token.Count() > 2 && token[2].Value<int>() != 0; // 0=large, 1=small
            bool isInverted = token.Count() > 3 && token[3].Value<bool>();
            return new CduCell(character, colorLetter, isSmall, isInverted);
        }
    }
}
