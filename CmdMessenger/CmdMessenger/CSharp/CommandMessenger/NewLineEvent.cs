#region CmdMessenger - MIT - (c) 2014 Thijs Elenbaas.
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

  Copyright 2014 - Thijs Elenbaas
*/
#endregion
using System;

namespace CommandMessenger
{
    public class NewLineEvent
    {
        public delegate void NewLineHandler(object sender, NewLineArgs e);

        public class NewLineArgs : EventArgs
        {
            public Command Command { get; private set; }
            public NewLineArgs(Command command)
            {
                Command = command;
            }
        }
    }
}
