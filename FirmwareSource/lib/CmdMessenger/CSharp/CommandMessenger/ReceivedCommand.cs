﻿#region CmdMessenger - MIT - (c) 2013 Thijs Elenbaas.
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
using System.Linq;

namespace CommandMessenger
{
    /// <summary> A command received from CmdMessenger </summary>
    public class ReceivedCommand : Command
    {
        private int _parameter=-1;    // The parameter
        private bool _dumped = true;  // true if parameter has been dumped

        /// <summary> Gets or sets the command input. </summary>
        /// <value> The raw string. </value>
        public string rawString { get; set; }

        /// <summary> Default constructor. </summary>
        public ReceivedCommand()
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="rawArguments"> All command arguments, first one is command ID </param>
        public ReceivedCommand(string[] rawArguments)
        {
            int cmdId;
            CmdId = (rawArguments != null && int.TryParse(rawArguments[0], out cmdId)) ? cmdId : -1;
            if (CmdId<0) return;
            if (rawArguments.Length > 1)
            {
                var array = new string[rawArguments.Length - 1];
                Array.Copy(rawArguments, 1, array, 0, array.Length);
                _arguments.AddRange(array);
            }
        }

        /// <summary> Fetches the next argument. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Next()
        {
            // If this parameter has already been read, see if there is another one
            if (_dumped)
            {
                if (_parameter < _arguments.Count-1)
                {
                    _parameter++;
                    _dumped = false;
                    return true;
                }
                return false;
            }
            return true;
        }

        /// <summary> returns if a next command is available </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool Available()
        {
            return Next();
        }

        // ***** String based **** /

        /// <summary> Reads the current argument as short value. </summary>
        /// <returns> The short value. </returns>
        public Int16 ReadInt16Arg()
        {
            if (Next())
            {
                Int16 current;
                if (Int16.TryParse(_arguments[_parameter], out current))
                {
                    _dumped = true;
                    return current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current argument as unsigned short value. </summary>
        /// <returns> The unsigned short value. </returns>
        public UInt16 ReadUInt16Arg()
        {
            if (Next())
            {
                UInt16 current;
                if (UInt16.TryParse(_arguments[_parameter], out current))
                {
                    _dumped = true;
                    return current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current argument as boolean value. </summary>
        /// <returns> The boolean value. </returns>
        public bool ReadBoolArg()
        {
            return (ReadInt32Arg() != 0);
        }

        /// <summary> Reads the current argument as int value. </summary>
        /// <returns> The int value. </returns>
        public Int32 ReadInt32Arg()
        {
            if (Next())
            {
                Int32 current;
                if (Int32.TryParse(_arguments[_parameter], out current))
                {
                    _dumped = true;
                    return current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current argument as unsigned int value. </summary>
        /// <returns> The unsigned int value. </returns>
        public UInt32 ReadUInt32Arg()
        {
            if (Next())
            {
                UInt32 current;
                if (UInt32.TryParse(_arguments[_parameter], out current))
                {
                    _dumped = true;
                    return current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current argument as a float value. </summary>
        /// <returns> The float value. </returns>
        public Single ReadFloatArg()
        {
            if (Next())
            {
                Single current;
                if (Single.TryParse(_arguments[_parameter], out current))
                {
                    _dumped = true;
                    return current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current argument as a double value. </summary>
        /// <returns> The unsigned double value. </returns>
        public Single ReadDoubleArg()
        {
            if (Next())
            {
                Single current;
                if (Single.TryParse(_arguments[_parameter], out current))
                {
                    _dumped = true;
                    return current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current argument as a string value. </summary>
        /// <returns> The string value. </returns>
        public String ReadStringArg()
        {
            if (Next())
            {
                if (_arguments[_parameter] != null)
                {
                    _dumped = true;
                    return _arguments[_parameter];
                }
            }
            return "";
        }

        // ***** Binary **** /

        /// <summary> Reads the current binary argument as a float value. </summary>
        /// <returns> The float value. </returns>
        public Single ReadBinFloatArg()
        {
            if (Next())
            {
                var current = BinaryConverter.ToFloat(_arguments[_parameter]);
                if (current != null)
                {
                    _dumped = true;
                    return (float) current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current binary argument as a double value. </summary>
        /// <returns> The double value. </returns>
        public Double ReadBinDoubleArg()
        {
            if (Next())
            {
                var current = BinaryConverter.ToDouble(_arguments[_parameter]);
                if (current != null)
                {
                    _dumped = true;
                    return (double) current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current binary argument as a short value. </summary>
        /// <returns> The short value. </returns>
        public Int16 ReadBinInt16Arg()
        {
            if (Next())
            {
                var current = BinaryConverter.ToInt16(_arguments[_parameter]);
                if (current != null)
                {
                    _dumped = true;
                    return (Int16) current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current binary argument as a unsigned short value. </summary>
        /// <returns> The unsigned short value. </returns>
        public UInt16 ReadBinUInt16Arg()
        {
            if (Next())
            {
                var current = BinaryConverter.ToUInt16(_arguments[_parameter]);
                if (current != null)
                {
                    _dumped = true;
                    return (UInt16) current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current binary argument as a int value. </summary>
        /// <returns> The int value. </returns>
        public Int32 ReadBinInt32Arg()
        {
            if (Next())
            {
                var current = BinaryConverter.ToInt32(_arguments[_parameter]);
                if (current != null)
                {
                    _dumped = true;
                    return (Int32) current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current binary argument as a unsigned int value. </summary>
        /// <returns> The unsigned int value. </returns>
        public UInt32 ReadBinUInt32Arg()
        {
            if (Next())
            {
                var current = BinaryConverter.ToUInt32(_arguments[_parameter]);
                if (current != null)
                {
                    _dumped = true;
                    return (UInt32) current;
                }
            }
            return 0;
        }

        /// <summary> Reads the current binary argument as a string value. </summary>
        /// <returns> The string value. </returns>
        public String ReadBinStringArg()
        {
            if (Next())
            {
                if (_arguments[_parameter] != null)
                {
                    _dumped = true;
                    return Escaping.Unescape(_arguments[_parameter]);
                }
            }
            return "";
        }

        /// <summary> Reads the current binary argument as a boolean value. </summary>
        /// <returns> The boolean value. </returns>
        public bool ReadBinBoolArg()
        {
            if (Next())
            {
                var current = BinaryConverter.ToByte(_arguments[_parameter]);
                if (current != null)
                {
                    _dumped = true;
                    return (current != 0);
                }
            }
            return false;
        }
    }
}