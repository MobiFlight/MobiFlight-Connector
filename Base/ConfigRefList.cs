using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Base
{
    public class ConfigRefList : ICloneable, IEnumerable
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
    }
}
