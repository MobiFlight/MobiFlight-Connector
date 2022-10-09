using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class VariableInputAction : InputAction, ICloneable
    {
        new public const String Label = "MobiFlight - Variable";
        public const String TYPE = "VariableInputAction";
        public MobiFlightVariable Variable = new MobiFlightVariable();
        
        override public object Clone()
        {
            VariableInputAction clone = new VariableInputAction();
            clone.Variable = Variable.Clone() as MobiFlightVariable;
            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            Variable.TYPE = reader["varType"];
            Variable.Name = reader["varName"];
            Variable.Expression = reader["varExpression"];
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", getType());
            writer.WriteAttributeString("varType", Variable.TYPE);
            writer.WriteAttributeString("varName", Variable.Name);
            writer.WriteAttributeString("varExpression", Variable.Expression);
        }

        protected virtual String getType()
        {
            return TYPE;
        }

        public override void execute(CacheCollection cacheCollection, 
                                     InputEventArgs args,
                                     List<ConfigRefValue> configRefs)
        {
            String value = Variable.Expression;
            MobiFlightCacheInterface moduleCache = cacheCollection.moduleCache;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();

            if (value.Contains("@"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("@", args.Value.ToString());
                replacements.Add(replacement);
            }

            if (value.Contains("$"))
            {
                MobiFlightVariable variable = moduleCache.GetMobiFlightVariable(Variable.Name);
                Tuple<string, string> replacement = new Tuple<string, string>("$", variable.TYPE == MobiFlightVariable.TYPE_NUMBER ? variable.Number.ToString() : variable.Text);
                replacements.Add(replacement);
            }


            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            value = Replace(value, replacements);

            if (Variable.TYPE == MobiFlightVariable.TYPE_NUMBER)
            {
                try
                {
                    Variable.Number = double.Parse(value);
                }
                catch (Exception)
                {


                }
            }

            Variable.Text = value;
            moduleCache.SetMobiFlightVariable(Variable);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is VariableInputAction &&
                Variable.Equals((obj as VariableInputAction).Variable);
        }
    }
}
