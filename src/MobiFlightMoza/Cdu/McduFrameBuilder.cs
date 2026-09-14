using System.Collections.Generic;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Cdu
{
    /// <summary>
    /// Turns a <see cref="CduPage"/> into the next MCDU frame to send: a full Keyframe for
    /// the first page (or after <see cref="Reset"/>), otherwise a Delta covering only the
    /// rows that changed - or nothing at all if the page is unchanged.
    /// </summary>
    internal sealed class McduFrameBuilder
    {
        private uint NextSequence = 1;
        private CduPage LastSentPage;

        public void Reset()
        {
            NextSequence = 1;
            LastSentPage = null;
        }

        // Returns null when the page hasn't changed since the last call.
        public byte[] BuildNext(CduPage page)
        {
            if (LastSentPage == null)
            {
                byte[] keyframe = BuildKeyframe(page);
                LastSentPage = page;
                return keyframe;
            }

            List<McduTextRow> textRows = [];
            List<McduStyleRow> styleRows = [];
            for (int row = 0; row < CduPage.Rows; row++)
            {
                if (LastSentPage.RowEquals(page, row)) continue;
                textRows.Add(BuildTextRow(page, row));
                styleRows.Add(BuildStyleRow(page, row));
            }

            LastSentPage = page;
            if (textRows.Count == 0) return null;

            return MozaMcduFrame.PackDelta(NextSequence++, textRows, styleRows);
        }

        private byte[] BuildKeyframe(CduPage page)
        {
            List<McduTextRow> textRows = [];
            List<McduStyleRow> styleRows = [];
            for (int row = 0; row < CduPage.Rows; row++)
            {
                textRows.Add(BuildTextRow(page, row));
                styleRows.Add(BuildStyleRow(page, row));
            }
            return MozaMcduFrame.PackKeyframe(NextSequence++, textRows, styleRows);
        }

        private static McduTextRow BuildTextRow(CduPage page, int row)
            => new((byte)row, [new McduTextSegment(0, page.GetRowText(row))]);

        private static McduStyleRow BuildStyleRow(CduPage page, int row)
        {
            byte[] styles = new byte[CduPage.Columns];
            for (int c = 0; c < CduPage.Columns; c++)
            {
                styles[c] = MozaStyleMapper.ToStyleByte(page[row, c]);
            }
            return new McduStyleRow((byte)row, [new McduStyleSegment(0, styles)]);
        }
    }
}
