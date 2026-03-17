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
using System.Text;

namespace CommandMessenger
{
    /// <summary> A command to be send by CmdMessenger </summary>
    public class Command
    {
        internal CommunicationManager CommunicationManager;

        protected readonly List<string> CmdArgs = new List<string>(); // The argument list of the command, first one is the command ID

        /// <summary> Gets or sets the command ID. </summary>
        /// <value> The command ID. </value>
        public int CmdId { get; set; }

        /// <summary> Gets the command arguments. </summary>
        /// <value> The arguments, first one is the command ID </value>
        public String[] Arguments
        {
            get { return CmdArgs.ToArray(); }
        }

        /// <summary> Gets or sets the time stamp. </summary>
        /// <value> The time stamp. </value>
        public long TimeStamp { get; set; }

        /// <summary> Constructor. </summary>
        public Command()
        {
            CmdId = -1;
            CmdArgs = new List<string>();
            TimeStamp = TimeUtils.Millis;
        }

        /// <summary> Returns whether this is a valid & filled command. </summary>
        /// <value> true if ok, false if not. </value>
        public bool Ok
        {
            get { return (CmdId >= 0); }
        }

        public string CommandString() 
        {
            if (CommunicationManager == null)
                throw new InvalidOperationException("CommunicationManager was not set for command.");

            var commandString = new StringBuilder(CmdId.ToString(CultureInfo.InvariantCulture));

            foreach (var argument in Arguments)
            {
                commandString.Append(CommunicationManager.FieldSeparator).Append(argument);
            }
            commandString.Append(CommunicationManager.CommandSeparator);

            return commandString.ToString();
        }
    }
}
