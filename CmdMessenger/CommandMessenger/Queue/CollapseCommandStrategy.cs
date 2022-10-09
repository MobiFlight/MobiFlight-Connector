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
    /// <summary> Collapse command strategy. 
    /// 		  The purpose of the strategy is to avoid duplicates of a certain command on the queue
    /// 		  to avoid lagging </summary>
    public class CollapseCommandStrategy : CommandStrategy
    {
        /// <summary>  Collapse strategy. </summary>
        /// <param name="command"> The command that will be collapsed on the queue. </param>
        public CollapseCommandStrategy(Command command) : base(command)
        {}

        /// <summary> Add command (strategy) to command queue. </summary>
        public override void Enqueue()
        {
            // find if there already is a command with the same CmdId
            var index = CommandQueue.FindIndex(strategy => strategy.Command.CmdId == Command.CmdId);
            if (index < 0)
            {
                // if not, add to the back of the queue
                CommandQueue.Enqueue(this);
            }
            else
            {
                // if on the queue, replace with new command
                CommandQueue[index] = this;
            }      
        }
    }
}
