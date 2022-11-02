using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class RetriggerInputAction : InputAction
    {
        public new const String Label = "MobiFlight - Retrigger switches";
        public const String TYPE = "RetriggerInputAction";
        DateTime lastExecution = DateTime.Now;

        public override object Clone()
        {
            RetriggerInputAction clone = new RetriggerInputAction();
         
            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            // Nothing to do
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
        }

        public override void execute(
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            // only execute if not happened last 1 seconds
            if (DateTime.Now.Ticks  - lastExecution.Ticks < 50000000) return;
            // Log.Instance.log("RetriggerInputAction.execute: Seconds since lastExecution " + (DateTime.Now.Ticks - lastExecution.Ticks), LogSeverity.Debug);

            foreach (MobiFlightModule module in cacheCollection.moduleCache.GetModules()) {
                module.Retrigger();
            }

            lastExecution = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is RetriggerInputAction;
        }
    }
}
