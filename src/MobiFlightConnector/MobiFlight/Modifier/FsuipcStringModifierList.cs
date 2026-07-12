using System.Xml;

namespace MobiFlight.Modifier
{
    public class FsuipcStringModifierList : ModifierList
    {
        protected override void ReadModifiers(XmlReader reader)
        {
            // this is the special case
            // when the relevant transformation
            // is the substring part
            if (reader.LocalName == "transformation")
            {
                var t = new Substring();
                t.ReadXml(reader);
                modifiers.Add(t);
                return;
            }

            base.ReadModifiers(reader);
        }

        public override object Clone()
        {
            var clone = new FsuipcStringModifierList();
            foreach (var modifier in modifiers)
            {
                clone.Items.Add(modifier.Clone() as ModifierBase);
            }

            return clone;
        }
    }
}