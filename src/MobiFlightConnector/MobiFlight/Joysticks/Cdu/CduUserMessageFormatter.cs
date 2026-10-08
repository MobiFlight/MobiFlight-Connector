using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.Joysticks.Cdu
{
    /// <summary>
    /// Renders a <see cref="UserMessageCodes"/> message as the 336-cell
    /// <c>{"Target":"Display",...}</c> JSON page every CDU (WinCtrl and MOZA alike)
    /// consumes. Shared so the word-wrap/centering rules exist in exactly one place.
    /// </summary>
    internal static class CduUserMessageFormatter
    {
        private const int Columns = 24;
        private const int Rows = 14;
        private const int Cells = Columns * Rows;

        private sealed class Cell
        {
            public char Character = ' ';
        }

        public static string Format(int messageCode, params string[] parameters)
        {
            string message = string.Format(UserMessageCodes.CodeToMessageMap[messageCode], parameters).ToUpper();
            string[] tokens = message.Split(' ');

            var lines = new List<string> { tokens[0], string.Empty };
            var currentLine = new StringBuilder();

            for (int i = 1; i < tokens.Length; i++)
            {
                string token = tokens[i];
                token = token.Length > Columns ? token.Substring(0, Columns) : token;

                if (currentLine.Length + token.Length + 1 > Columns)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                }

                token = currentLine.Length == 0 ? token : $" {token}";
                currentLine.Append(token);

                if (i == tokens.Length - 1)
                {
                    lines.Add(currentLine.ToString());
                }
            }

            return BuildPage(lines);
        }

        private static string BuildPage(IList<string> lines)
        {
            int emptyLineCount = (Rows - Math.Min(lines.Count, Rows)) / 2;
            var linesToDisplay = Enumerable.Repeat(string.Empty, emptyLineCount).Concat(lines).ToList();

            var cells = new Cell[Cells];
            int currentIndex = 0;

            for (int i = 0; i < linesToDisplay.Count; i++)
            {
                string currentLine = linesToDisplay[i];
                int padLeft = (Columns - currentLine.Length) / 2 + currentLine.Length;
                string paddedLine = currentLine.PadLeft(padLeft).PadRight(Columns);

                for (int j = 0; j < paddedLine.Length; j++)
                {
                    if (currentIndex < cells.Length)
                    {
                        cells[currentIndex] = new Cell { Character = paddedLine[j] };
                    }
                    currentIndex++;
                }
            }

            var builder = new StringBuilder("{ \"Target\": \"Display\", \"Data\": [");
            foreach (var cell in cells)
            {
                if (cell == null || cell.Character == ' ')
                {
                    builder.Append("[],");
                }
                else
                {
                    builder.Append($"[\"{cell.Character}\", \"w\", 0],");
                }
            }
            builder.Remove(builder.Length - 1, 1); // Remove last comma
            builder.Append("] }");
            return builder.ToString();
        }
    }
}
