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

namespace CommandMessenger
{
    // This class will trigger the main thread when a specific command is received on the ReceiveCommandQueue thread
    // this is used when synchronously waiting for an acknowledge command in BlockedTillReply
    public class ReceivedCommandSignal
    {
        private int _cmdIdToMatch;
        private SendQueue _sendQueueState;
        private ReceivedCommand _receivedCommand;

        private readonly object _lock = new object();
        private readonly EventWaiter _waiter = new EventWaiter();

        /// <summary>
        /// Prepares for the WaitForCmd to be used.
        /// </summary>
        /// <param name="cmdId"></param>
        /// <param name="sendQueueState"></param>
        public void PrepareForWait(int cmdId, SendQueue sendQueueState)
        {
            lock (_lock)
            {
                _receivedCommand = null;
                _cmdIdToMatch = cmdId;
                _sendQueueState = sendQueueState;
            }
        }

        /// <summary>
        /// Wait function. PrepareForWait must be called before to prepare!
        /// </summary>
        /// <param name="timeOut">time-out in ms</param>
        /// <returns></returns>
        public ReceivedCommand WaitForCmd(int timeOut)
        {
            return _waiter.WaitOne(timeOut) == EventWaiter.WaitState.TimeOut ? null : _receivedCommand;
        }

        /// <summary>
        /// Process command. See if it needs to be send to the main thread (false) or be used in queue (true)
        /// </summary>
        public bool ProcessCommand(ReceivedCommand receivedCommand)
        {
            lock (_lock)
            {
                if (receivedCommand.CmdId == _cmdIdToMatch)
                {
                    _receivedCommand = receivedCommand;
                    _waiter.Set();
                    return false;
                }

                return (_sendQueueState != SendQueue.ClearQueue);
            }
        }
    }
}
