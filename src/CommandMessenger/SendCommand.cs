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

namespace CommandMessenger
{
    /// <summary> A command to be send by CmdMessenger </summary>
    public class SendCommand : Command
    {
        private readonly List<Action> _lazyArguments = new List<Action>();

        /// <summary> Indicates if we want to wait for an acknowledge command. </summary>
        /// <value> true if request acknowledge, false if not. </value>
        public bool ReqAc { get; set; }

        /// <summary> Gets or sets the acknowledge command ID. </summary>
        /// <value> the acknowledge command ID. </value>
        public int AckCmdId { get; set; }

        /// <summary> Gets or sets the time we want to wait for the acknowledge command. </summary>
        /// <value> The timeout on waiting for an acknowledge</value>
        public int Timeout { get; set; }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId"> the command ID. </param>
        public SendCommand(int cmdId)
        {
            Init(cmdId, false, 0, 0);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, string argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">     Command ID </param>
        /// <param name="arguments"> The arguments. </param>
        public SendCommand(int cmdId, string[] arguments)
        {
            Init(cmdId, false, 0, 0);
            AddArguments(arguments);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, float argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, double argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, UInt16 argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, Int16 argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, UInt32 argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, Int32 argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        public SendCommand(int cmdId, bool argument)
        {
            Init(cmdId, false, 0, 0);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, string argument, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">     Command ID </param>
        /// <param name="arguments"> The arguments. </param>
        /// <param name="ackCmdId">  Acknowledge command ID. </param>
        /// <param name="timeout">   The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, string[] arguments, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArguments(arguments);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, float argument, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, double argument, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, Int16 argument, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, UInt16 argument, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, Int32 argument, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArgument(argument);
        }

        /// <summary> Constructor. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="argument"> The argument. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        public SendCommand(int cmdId, UInt32 argument, int ackCmdId, int timeout)
        {
            Init(cmdId, true, ackCmdId, timeout);
            AddArgument(argument);
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="cmdId">    Command ID </param>
        /// <param name="reqAc">    true to request ac. </param>
        /// <param name="ackCmdId"> Acknowledge command ID. </param>
        /// <param name="timeout">  The timeout on waiting for an acknowledge</param>
        private void Init(int cmdId, bool reqAc, int ackCmdId, int timeout)
        {
            ReqAc = reqAc;
            CmdId = cmdId;
            AckCmdId = ackCmdId;
            Timeout = timeout;
        }

        // ***** String based **** /

        /// <summary> Adds a command argument.  </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(string argument)
        {
            if (argument != null)
                _lazyArguments.Add(() => CmdArgs.Add(argument));
        }

        /// <summary> Adds command arguments.  </summary>
        /// <param name="arguments"> The arguments. </param>
        public void AddArguments(string[] arguments)
        {
            if (arguments != null)
                _lazyArguments.Add(() => CmdArgs.AddRange(arguments));
        }

        /// <summary> Adds a command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(float argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(argument.ToString("R", CultureInfo.InvariantCulture)));
        }

        /// <summary> Adds a command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(Double argument)
        {
            _lazyArguments.Add(() =>
            {
                if (CommunicationManager.BoardType == BoardType.Bit16)
                {
                    // Not completely sure if this is needed for plain text sending.
                    var floatArg = (float) argument;
                    CmdArgs.Add(floatArg.ToString("R", CultureInfo.InvariantCulture));
                }
                else
                {
                    CmdArgs.Add(argument.ToString("R", CultureInfo.InvariantCulture));
                }
            });
        }

        /// <summary> Adds a command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(Int16 argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(argument.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary> Adds a command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(UInt16 argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(argument.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary> Adds a command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(Int32 argument)
        {
            // Make sure the other side can read this: on a 16 processor, read as Long
            _lazyArguments.Add(() => CmdArgs.Add(argument.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary> Adds a command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(UInt32 argument)
        {
            // Make sure the other side can read this: on a 16 processor, read as Long
            _lazyArguments.Add(() => CmdArgs.Add(argument.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary> Adds a command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddArgument(bool argument)
        {
            AddArgument((Int16) (argument ? 1 : 0));
        }

        // ***** Binary **** /

        /// <summary> Adds a binary command argument.  </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(string argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(Escaping.Escape(argument)));
        }

        /// <summary> Adds a binary command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(float argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(BinaryConverter.ToString(argument)));
        }

        /// <summary> Adds a binary command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(Double argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(CommunicationManager.BoardType == BoardType.Bit16
                ? BinaryConverter.ToString((float)argument)
                : BinaryConverter.ToString(argument)));
        }

        /// <summary> Adds a binary command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(Int16 argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(BinaryConverter.ToString(argument)));
        }

        /// <summary> Adds a binary command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(UInt16 argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(BinaryConverter.ToString(argument)));
        }

        /// <summary> Adds a binary command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(Int32 argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(BinaryConverter.ToString(argument)));
        }

        /// <summary> Adds a binary command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(UInt32 argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(BinaryConverter.ToString(argument)));
        }

        /// <summary> Adds a binary command argument. </summary>
        /// <param name="argument"> The argument. </param>
        public void AddBinArgument(bool argument)
        {
            _lazyArguments.Add(() => CmdArgs.Add(BinaryConverter.ToString(argument ? (byte) 1 : (byte) 0)));
        }

        internal void InitArguments()
        {
            CmdArgs.Clear();
            foreach (var action in _lazyArguments)
            {
                action.Invoke();
            }
        }
    }
}
