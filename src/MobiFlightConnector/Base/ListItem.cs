using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class ListItem : ListItem<string>
    { }

    public class ListItem<T>
    {
        public T Value { get; set; }
        public string Label { get; set; }
        public override string ToString() { return Value?.ToString() ?? string.Empty; }
    }
}
