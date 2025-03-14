using System;
using System.Collections;
using System.Collections.Generic;

namespace MobiFlight.Base
{
    public class ConfigRefList : ICloneable, IEnumerable, ICollection<ConfigRef>
    {
        List<ConfigRef> list = new List<ConfigRef>();

        public int Count { get { return list.Count; } }

        public void Add(ConfigRef item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public object Clone()
        {
            ConfigRefList c = new ConfigRefList();
            foreach(var item in list)
            {
                c.Add(item.Clone() as ConfigRef);
            }
            return c;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            bool areEqual =
                obj != null &&
                obj is ConfigRefList &&
                (Count == (obj as ConfigRefList).Count)
                ;
            if (areEqual)
            {
                for (int i = 0; i != Count; i++)
                {
                    areEqual = areEqual && (list[i].Equals((obj as ConfigRefList).list[i]));
                }
            }

            return areEqual;
        }

        public bool Contains(ConfigRef item)
        {
            return list.Contains(item);
        }

        public void CopyTo(ConfigRef[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ConfigRef item)
        {
            return list.Remove(item);
        }

        public bool IsReadOnly => false;

        IEnumerator<ConfigRef> IEnumerable<ConfigRef>.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
