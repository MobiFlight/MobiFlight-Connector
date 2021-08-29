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

using System.Text;

namespace CommandMessenger
{
    /// <summary> String utilities. </summary>
    public class StringUtils
    {
        /// <summary> Convert string from one codepage to another. </summary>
        /// <param name="input">        The string. </param>
        /// <param name="fromEncoding"> input encoding codepage. </param>
        /// <param name="toEncoding">   output encoding codepage. </param>
        /// <returns> the encoded string. </returns>
        static public string ConvertEncoding(string input, Encoding  fromEncoding, Encoding toEncoding)
        {
            var byteArray = fromEncoding.GetBytes(input);
            var asciiArray = Encoding.Convert(fromEncoding, toEncoding, byteArray);
            var finalString = toEncoding.GetString(asciiArray);
            return finalString;
        }
    }
}
