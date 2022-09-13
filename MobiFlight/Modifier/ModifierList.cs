using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Modifier
{
    public class ModifierList : IXmlSerializable, ICloneable
    {
        List<ModifierBase> modifiers = new List<ModifierBase>();
        public List<ModifierBase> Items { get { return modifiers; } }

        public Transformation Transformation
        {
            get
            {
                if (modifiers.Count == 0) { InitModifiersForBackwardCompatibility(); }
                foreach (ModifierBase modifier in modifiers)
                {
                    if (modifier is Transformation)
                        return modifier as Transformation;
                }

                var t = new Transformation();
                modifiers.Add(t);
                return t;
            }

            set
            {
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (!(modifiers[i] is Transformation)) continue;

                    modifiers[i] = value;
                    return;
                }
            }
        }

        public Comparison Comparison
        {
            get
            {
                if (modifiers.Count == 0) { InitModifiersForBackwardCompatibility(); }
                foreach (ModifierBase modifier in modifiers)
                {
                    if (modifier is Comparison)
                        return modifier as Comparison;
                }

                var c = new Comparison();
                modifiers.Add(c);
                return c;
            }

            set
            {
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (!(modifiers[i] is Comparison)) continue;

                    modifiers[i] = value;
                    return;
                }
            }
        }

        public Interpolation Interpolation
        {
            get
            {
                if (modifiers.Count == 0) { InitModifiersForBackwardCompatibility(); }
                foreach (ModifierBase modifier in modifiers)
                {
                    if (modifier is Interpolation)
                        return modifier as Interpolation;
                }

                var i = new Interpolation();
                modifiers.Add(i);
                return i;
            }

            set
            {
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (!(modifiers[i] is Interpolation)) continue;

                    modifiers[i] = value;
                    return;
                }

                modifiers.Add(value);
            }
        }

        /// <summary>
        /// This method serves only for backward compatibliy
        /// The old config logic was always executing in this order
        ///  1. Transformation
        ///  2. Comparison
        ///  3. Interpolation
        ///  
        /// So if we access any of the "old" direct properties
        /// and no modifiers have been registered yet, we simply
        /// add these default modifiers to the list
        /// 
        /// Once the config got saved with the new format, 
        /// this does not matter anymore
        /// </summary>
        private void InitModifiersForBackwardCompatibility()
        {
            modifiers.Add(new Transformation());
            modifiers.Add(new Comparison());
            modifiers.Add(new Interpolation());
        }

        public override bool Equals(object obj)
        {
            var result = obj != null && obj is ModifierList && (obj as ModifierList).Items.Count == Items.Count;
            if (!result) return result;

            for(var i = 0; i < modifiers.Count; i++)
            {
                if (!modifiers[i].Equals((obj as ModifierList).Items[i]))
                    return false;
            }

            return true;
        }

        public void ReadXml(XmlReader reader)
        {
            do
            {
                // advance to the next node
                reader.Read();
                switch (reader.LocalName)
                {
                    case "transformation":
                        var t = new Transformation();
                        t.ReadXml(reader);
                        modifiers.Add(t);
                        break;

                    case "blink":
                        var b = new Blink();
                        b.ReadXml(reader);
                        modifiers.Add(b);
                        break;

                    case "comparison":
                        var c = new Comparison();
                        c.ReadXml(reader);
                        modifiers.Add(c);
                        break;

                    case "interpolation":
                        var i = new Interpolation();
                        i.ReadXml(reader);
                        modifiers.Add(i);
                        break;
                }
            } while (reader.LocalName != "modifiers");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("modifiers");
            foreach (var modifier in modifiers)
            {
                modifier.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public object Clone()
        {
            var clone = new ModifierList();
            foreach (var modifier in modifiers)
            {
                clone.Items.Add(modifier.Clone() as ModifierBase);
            }

            return clone;
        }
    }
}
