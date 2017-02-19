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

namespace CommandMessenger
{
    /// <summary>
    /// The dispose stack takes manages disposal of objects that are pushed onto the stack.
    /// When the stack is disposed all objects are disposed (in reversed order). 
    /// </summary>
    public sealed class DisposeStack : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        /// <summary>
        /// Pushes a disposable object under the DisposeStack.
        /// </summary>
        /// <typeparam name="T">Type of object pushed</typeparam>
        /// <param name="newObject">The object pushed under the stack</param>
        /// <returns>Returns the pushed object</returns>
        public T PushFront<T>(T newObject) where T : IDisposable
        {
            _disposables.Insert(0, newObject);
            return newObject;
        }

        /// <summary>
        /// Pushes a disposable object under the DisposeStack.
        /// </summary>
        /// <typeparam name="T">Type of object pushed</typeparam>
        /// <param name="newObject">The object pushed on the stack</param>
        /// <returns>Returns the pushed object</returns>
        public T Push<T>(T newObject) where T : IDisposable
        {
            _disposables.Add(newObject);
            return newObject;
        }

        /// <summary>
        /// Push an arbitrary number of disposable objects onto the stack in one call
        /// </summary>
        /// <param name="objects">The disposable objects</param>
        public void Push(params IDisposable[] objects)
        {
            foreach (IDisposable d in objects)
            {
                _disposables.Add(d);
            }
        }
        
        /// <summary>
        /// Dispose all items within the dispose stack.
        /// </summary>
        public void Dispose()
        {
            for (int i = _disposables.Count - 1; i >= 0; --i)
            {
                _disposables[i].Dispose();
            }

            _disposables.Clear();
        }
    }
}
