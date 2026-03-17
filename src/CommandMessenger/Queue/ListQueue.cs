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

using System.Collections.Generic;

namespace CommandMessenger.Queue
{
    /// <summary> Queue class.  </summary>
    /// <typeparam name="T"> Type of object to queue. </typeparam>
    public class ListQueue<T> : List<T>
    {
        /// <summary> Adds item to front of queue. </summary>
        /// <param name="item"> The item to queue. </param>
        public void EnqueueFront(T item)
        {
            Insert(Count, item);
        }

        /// <summary> Adds item to back of queue. </summary>
        /// <param name="item"> The item to queue. </param>
        public void Enqueue(T item)
        {
            Add(item);
        }

        /// <summary> fetches item from front of queue. </summary>
        /// <returns> The item to dequeue. </returns>
        public T Dequeue()
        {
            var t = base[0];
            RemoveAt(0);
            return t;
        }

        /// <summary> look at item at front of queue without removing it from the queue. </summary>
        /// <returns> The item to peek at. </returns>
        public T Peek()
        {
            return base[0];
        }

    }
}
