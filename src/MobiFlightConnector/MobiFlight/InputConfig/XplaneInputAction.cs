using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MobiFlight.InputConfig
{
    public class XplaneInputAction : InputAction, ICloneable
    {
        public new const string Label = "X-Plane (all versions)";
        public const string INPUT_TYPE_DATAREF = "DataRef";
        public const string INPUT_TYPE_COMMAND = "Command";
        public const string TYPE = "XplaneInputAction";
        public string InputType { get; set; } = "DataRef";
        public string Path { get; set; } = "";
        public string Expression { get; set; } = "$";

        public override object Clone()
        {
            XplaneInputAction clone = new XplaneInputAction();
            clone.InputType = InputType;
            clone.Path = Path.Clone() as string;
            clone.Expression = Expression.Clone() as string;
            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            Path = reader["path"];
            InputType = reader["inputType"];
            Expression = reader["expression"];
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("inputType", InputType);
            writer.WriteAttributeString("path", Path);
            writer.WriteAttributeString("expression", Expression);
        }

        public override void execute(
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            var xplaneCache = cacheCollection.xplaneCache;

            switch (InputType)
            {
                case INPUT_TYPE_DATAREF:
                    var value = PrepareValue(args, configRefs, cacheCollection.xplaneCache);
                    HandleDataref(value, xplaneCache);
                    break;
                case INPUT_TYPE_COMMAND:
                    HandleCommand(Path, args, xplaneCache);
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is XplaneInputAction action &&
                   Path == action.Path &&
                   Expression == action.Expression;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The following commands would both be treated as fire-and-forget
        /// onPress: laminar/B738/EFIS_control/capt/vor1_off_up
        /// onRelease: laminar/B738/EFIS_control/capt/vor1_off_dn
        /// <br/>
        /// <br/>
        /// While these would invoke the begin/end phases:
        /// onPress: laminar/B738/EFIS_control/capt/do_something
        /// onRelease: laminar/B738/EFIS_control/capt/do_something
        /// </remarks>
        public override bool TargetsSameCommandAs(InputAction other)
        {
            return InputType == INPUT_TYPE_COMMAND &&
                   other is XplaneInputAction otherAction &&
                   otherAction.InputType == INPUT_TYPE_COMMAND &&
                   Path == otherAction.Path;
        }

        private void HandleDataref(
            string value,
            XplaneCacheInterface xplaneCache)
        {
            try
            {
                var finalValue = float.Parse(value, CultureInfo.InvariantCulture);
                xplaneCache.writeDataRef(Path, finalValue);
            }
            catch (Exception)
            {
                // Ignore
            }
        }
        
        private void HandleCommand(
            string value,
            InputEventArgs args,
            XplaneCacheInterface xplaneCache)
        {
            if (args is { InputType: DeviceType.Button, SyntheticDelayMs: null, SyntheticHoldDelayMs: null })
            {
                switch (args)
                {
                    case { Value: (int)MobiFlightButton.InputEvent.PRESS }:
                        if (args.HasPairedReleaseAction)
                            xplaneCache.beginCommand(value);
                        else
                            xplaneCache.sendCommand(value);
                        return;
                    case { Value: (int)MobiFlightButton.InputEvent.RELEASE }:
                    case { Value: (int)MobiFlightButton.InputEvent.LONG_RELEASE }:
                        if (args.HasPairedReleaseAction)
                            xplaneCache.endCommand(value);
                        else
                            xplaneCache.sendCommand(value);
                        return;
                }
            }

            xplaneCache.sendCommand(value);
        }

        private string PrepareValue(
            InputEventArgs args,
            List<ConfigRefValue> configRefs,
            XplaneCacheInterface xplaneCache)
        {
            var value = Expression;
            var replacements = new List<Tuple<string, string>>();

            if (value.Contains('@'))
            {
                var replacement = new Tuple<string, string>("@", args.Value.ToString(CultureInfo.InvariantCulture));
                replacements.Add(replacement);
            }

            if (value.Contains('$'))
            {
                var currentValue = xplaneCache.readDataRef(Path);
                var replacement = new Tuple<string, string>("$", currentValue.ToString(CultureInfo.InvariantCulture));
                replacements.Add(replacement);
            }

            replacements.AddRange(configRefs.Select(item =>
                new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value)));

            return Replace(value, replacements);
        }
    }
}