using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class VariableInputAction : InputAction, ICloneable
    {
        new public const String Label = "MobiFlight Variable";
        public const String TYPE = "VariableInputAction";
        public MobiFlightVariable Variable = new MobiFlightVariable();
        public String Value = "0";
        
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

        public override void execute(FSUIPC.FSUIPCCacheInterface fsuipcCache, 
                                     SimConnectMSFS.SimConnectCacheInterface simConnectCache, 
                                     MobiFlightCacheInterface moduleCache, 
                                     InputEventArgs args,
                                     List<ConfigRefValue> configRefs)
        {
            String result = Variable.Expression;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();

            if (result.Contains("$"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("$", args.Value.ToString());
                replacements.Add(replacement);
            }

            
            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            result = Replace(result, replacements);
            Variable.Number = double.Parse(result);
            Variable.Text = result;
            moduleCache.SetMobiFlightVariable(Variable);
        }
    }
}
