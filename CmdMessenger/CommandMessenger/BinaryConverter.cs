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
using System.Text;

namespace CommandMessenger
{
    public class BinaryConverter
    {
        private static Encoding _stringEncoder = Encoding.GetEncoding("ISO-8859-1"); // The string encoder

        /// <summary> Sets the string encoder. </summary>
        /// <value> The string encoder. </value>
        public Encoding StringEncoder
        {
            set { _stringEncoder = value; }
        }

        /***** from binary value to string ****/

        /// <summary> Convert a float into a string representation. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> A string representation of this object. </returns>
        public static string ToString(float value)
        {
            try
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return BytesToEscapedString(byteArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Convert a Double into a string representation. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> A string representation of this object. </returns>
        public static string ToString(Double value)
        {
            try
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return BytesToEscapedString(byteArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Convert an int into a string representation. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> A string representation of this object. </returns>
        public static string ToString(int value)
        {
            try
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return BytesToEscapedString(byteArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Convert an unsigned int into a string representation. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> A string representation of this object. </returns>
        public static string ToString(uint value)
        {
            try
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return BytesToEscapedString(byteArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Convert a short into a string representation. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> A string representation of this object. </returns>
        public static string ToString(short value)
        {
            try
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return BytesToEscapedString(byteArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Convert an unsigned an unsigned short into a string representation. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> A string representation of this object. </returns>
        public static string ToString(ushort value)
        {
            try
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return BytesToEscapedString(byteArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Convert a byte into a string representation. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> A string representation of this object. </returns>
        public static string ToString(byte value)
        {
            try
            {
                return BytesToEscapedString(new byte[] {value});
            }
            catch (Exception)
            {
                return null;
            }
        }


        /***** from string to binary value ****/

        /// <summary> Converts a string to a float. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> Input string as a float? </returns>
        public static float? ToFloat(String value)
        {
            try
            {
                byte[] bytes = EscapedStringToBytes(value);
                if (bytes.Length < 4)
                {
                    return null;
                }
                return BitConverter.ToSingle(bytes, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts a string representation to a double. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> Input string as a Double? </returns>
        public static Double? ToDouble(String value)
        {
            try
            {
                byte[] bytes = EscapedStringToBytes(value);
                return BitConverter.ToDouble(bytes, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts a string representation to an int 32. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> This object as an Int32? </returns>
        public static Int32? ToInt32(String value)
        {
            try
            {
                byte[] bytes = EscapedStringToBytes(value);
                return BitConverter.ToInt32(bytes, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts a string representation to a u int 32. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> Input string as a UInt32? </returns>
        public static UInt32? ToUInt32(String value)
        {
            try
            {
                byte[] bytes = EscapedStringToBytes(value);
                return BitConverter.ToUInt32(bytes, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts a string representation to a u int 16. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> Input string as a UInt16? </returns>
        public static UInt16? ToUInt16(String value)
        {
            try
            {
                byte[] bytes = EscapedStringToBytes(value);
                return BitConverter.ToUInt16(bytes, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts a string representation to an int 16. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> This object as an Int16? </returns>
        public static Int16? ToInt16(String value)
        {
            try
            {
                byte[] bytes = EscapedStringToBytes(value);
                return BitConverter.ToInt16(bytes, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts a string representation to a byte. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> Input string as a byte? </returns>
        public static byte? ToByte(String value)
        {
            try
            {
                byte[] bytes = EscapedStringToBytes(value);
                return bytes[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /***** conversion functions ****/

        /// <summary> Converts a byte array to escaped string. </summary>
        /// <param name="byteArray"> Array of bytes. </param>
        /// <returns> input value as an escaped string. </returns>
        private static string BytesToEscapedString(byte[] byteArray)
        {
            try
            {
                string stringValue = _stringEncoder.GetString(byteArray);
                return Escaping.Escape(stringValue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts an escaped string to a bytes array. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> input value as an escaped string. </returns>
        public static byte[] EscapedStringToBytes(String value)
        {
            try
            {
                string unEscapedValue = Escaping.Unescape(value);
                return _stringEncoder.GetBytes(unEscapedValue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Converts a string to a bytes array. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> input value as a byte array. </returns>
        public static byte[] StringToBytes(string value)
        {
            return _stringEncoder.GetBytes(value);
        }

        /// <summary> Converts a char array to a bytes array. </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <returns> input value as a byte array. </returns>
        public static byte[] CharsToBytes(char[] value)
        {
            return _stringEncoder.GetBytes(value);
        }

    }
}