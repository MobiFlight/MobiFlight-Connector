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

namespace CommandMessenger
{
    /// <summary> Stale strategy. Any command older than the time-out is removed from the queue</summary>
    public class StaleGeneralStrategy : GeneralStrategy
    {
        private readonly long _commandTimeOut;

        /// <summary> Stale strategy. Any command older than the time-out is removed from the queue</summary>
        /// <param name="commandTimeOut"> The time-out for any commands on the queue. </param>
        public StaleGeneralStrategy(long commandTimeOut)
        {
            _commandTimeOut = commandTimeOut;
        }

        /// <summary> Remove this command (strategy) from command queue. </summary>
        public override void OnDequeue()
        {
            // Remove commands that have gone stale
            //Console.WriteLine("Before StaleStrategy {0}", CommandQueue.Count);
            var currentTime = TimeUtils.Millis;
            // Work from oldest to newest
            for (var item = 0; item < CommandQueue.Count; item++)
            {
                var age = currentTime - CommandQueue[item].Command.TimeStamp;
                if (age > _commandTimeOut && CommandQueue.Count > 1 )
                {
                    CommandQueue.RemoveAt(item);
                }
                else
                {
                    // From here on commands are newer, so we can stop
                    break;
                }
            }
            //Console.WriteLine("After StaleStrategy {0}", CommandQueue.Count);
        }
    }
}
