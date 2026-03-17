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

namespace CommandMessenger.Queue
{
    /// <summary> Base command strategy.  </summary>
    public class CommandStrategy
    {
        /// <summary> Base command strategy. </summary>
        /// <param name="command"> The command to be wrapped in a strategy. </param>
        public CommandStrategy(Command command)
        {
            Command = command;   
        }

        /// <summary> Gets or sets the command queue. </summary>
        /// <value> A Queue of commands. </value>
        public ListQueue<CommandStrategy> CommandQueue { get; set; }

        /// <summary> Gets or sets the command. </summary>
        /// <value> The command wrapped in the strategy. </value>
        public Command Command { get; private set; }

        /// <summary> Add command (strategy) to command queue. </summary>
        public virtual void Enqueue()
        {
            CommandQueue.Enqueue(this);
        }

        /// <summary> Remove this command (strategy) from command queue. </summary>
        public virtual void DeQueue()
        {
            CommandQueue.Remove(this);
        }
    }
}
