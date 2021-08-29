using System;
using System.Collections.Generic;

namespace CommandMessengerTests
{
    public class Enumerator
    {
        readonly Dictionary<string, int> _enumTable;
        private int _enumCounter;
        public Enumerator()
        {
            _enumTable = new Dictionary<string, int>();
            _enumCounter = 0;
        }

        public void Add(string enumDescriptor)
        {
            _enumTable.Add(enumDescriptor, _enumCounter++);
        }

        public void Add(string[] enumDescriptors)
        {
            foreach (var enumDescriptor in enumDescriptors)
            {
                Add(enumDescriptor);
            }
        }

        public int this[string enumDescriptor]
        {
            get
            {
                if (_enumTable.ContainsKey(enumDescriptor))
                {
                    return _enumTable[enumDescriptor];
                }
                else
                {
                    throw new ArgumentException("This enum does not exist");
                }
            }
            set { _enumTable[enumDescriptor] = value; }
        }

    }
}
