#region CmdMessenger - MIT - (c) 2013 Thijs Elenbaas.
/*
  CmdMessenger - library that provides command based messaging

  Permission is hereby granted, free of charge, to any person obtaining
  a copy of this software and associated documentation files (the
  "Software"), to deal in the Software without restriction, including
  without limitation the rights to use, copy, modify, merge, publish,
  distribute, sublicense, and/or sell copies of the Software, and to
  permit persons to whom the Software is furnished to do so, subject to
  the following conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  Copyright 2013 - Thijs Elenbaas
*/
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CommandMessenger
{
    /// <summary> Class for bookkeeping which characters in the stream are escaped. </summary>
    public class IsEscaped
    {
        private char _lastChar = '\0';  // The last character

        // Returns if the character is escaped
        // Note create new instance for every independent string

        /// <summary>Returns if the character is escaped.
        /// 		 Note create new instance for every independent string </summary>
        /// <param name="currChar"> The currebt character. </param>
        /// <returns> true if the character is escaped, false if not. </returns>
        public bool EscapedChar(char currChar)
        {
            bool escaped = (_lastChar == Escaping.EscapeCharacter);
            _lastChar = currChar;

            // special case: the escape char has been escaped: 
            if (_lastChar == Escaping.EscapeCharacter && escaped)
            {
                _lastChar = '\0';
            }
            return escaped;
        }
    }

    /// <summary> Utility class providing escaping functions </summary>
    public class Escaping
    {
        // Remove all occurrences of removeChar unless it is escaped by escapeChar

        private static char _fieldSeparator   = ',';	// The field separator
        private static char _commandSeparator = ';';	// The command separator
        private static char _escapeCharacter  = '/';	// The escape character

        /// <summary> Gets the escape character. </summary>
        /// <value> The escape character. </value>
        public static char EscapeCharacter
        {
            get { return _escapeCharacter; }
        }

        /// <summary> Sets custom escape characters. </summary>
        /// <param name="fieldSeparator">   The field separator. </param>
        /// <param name="commandSeparator"> The command separator. </param>
        /// <param name="escapeCharacter">  The escape character. </param>
        public static void EscapeChars(char fieldSeparator, char commandSeparator, char escapeCharacter)
        {
            _fieldSeparator = fieldSeparator;
            _commandSeparator = commandSeparator;
            _escapeCharacter = escapeCharacter;
        }

        /// <summary> Removes all occurences of a specific character unless escaped. </summary>
        /// <param name="input">      The input. </param>
        /// <param name="removeChar"> The  character to remove. </param>
        /// <param name="escapeChar"> The escape character. </param>
        /// <returns> The string with all removeChars removed. </returns>
        public static string Remove(string input, char removeChar, char escapeChar)
        {
            var output = "";
            var escaped = new IsEscaped();
            for (var i = 0; i < input.Length; i++)
            {
                char inputChar = input[i];
                bool isEscaped = escaped.EscapedChar(inputChar);
                if (inputChar != removeChar || isEscaped)
                {
                    output += inputChar;
                }
            }
            return output;
        }

        // Split String on separator character unless it is escaped by escapeChar

        /// <summary> Splits. </summary>
        /// <param name="input">              The input. </param>
        /// <param name="separator">          The separator. </param>
        /// <param name="escapeCharacter">    The escape character. </param>
        /// <param name="stringSplitOptions"> Options for controlling the string split. </param>
        /// <returns> The split string. </returns>
        public static String[] Split(string input, char separator, char escapeCharacter,
                                     StringSplitOptions stringSplitOptions)
        {
            var word = "";
            var result = new List<string>();
            for (var i = 0; i < input.Length; i++)
            {
                var t = input[i];
                if (t == separator)
                {
                    result.Add(word);
                    word = "";
                }
                else
                {
                    if (t == escapeCharacter)
                    {
                        word += t;
                        if (i < input.Length - 1) t = input[++i];
                    }
                    word += t;
                }
            }
            result.Add(word);
            if (stringSplitOptions == StringSplitOptions.RemoveEmptyEntries) result.RemoveAll(item => item == "");
            return result.ToArray();
        }

        /// <summary> Escapes the input string. </summary>
        /// <param name="input"> The unescaped input string. </param>
        /// <returns> Escaped output string. </returns>
        public static string Escape(string input)
        {
            var escapeChars = new[]
                {
                    _escapeCharacter.ToString(CultureInfo.InvariantCulture),
                    _fieldSeparator.ToString(CultureInfo.InvariantCulture),
                    _commandSeparator.ToString(CultureInfo.InvariantCulture),
                    "\0"
                };
            input = escapeChars.Aggregate(input,
                                          (current, escapeChar) =>
                                          current.Replace(escapeChar, _escapeCharacter + escapeChar));
            return input;
        }

        /// <summary> Unescapes the input string. </summary>
        /// <param name="input"> The escaped input string. </param>
        /// <returns> The unescaped output string. </returns>
        public static string Unescape(string input)
        {
            string output = "";
            // Move unescaped characters right
            for (var fromChar = 0; fromChar < input.Length; fromChar++)
            {
                if (input[fromChar] == _escapeCharacter)
                {
                    fromChar++;
                }
                output += input[fromChar];
            }
            return output;
        }
    }
}