using System;
using System.Xml;
using System.Collections.Generic;

namespace MobiFlight.InputConfig
{
    public class ProSimInputAction : InputAction, ICloneable
    {
        new public const string Label = "ProSim";
        public const string TYPE = "ProSimInputAction";

        public string Path { get; set; } = "";
        public string Expression { get; set; } = "$";

        public override object Clone()
        {
            ProSimInputAction clone = new ProSimInputAction();
            clone.Path = this.Path;
            clone.Expression = this.Expression;
            return clone;
        }

        public override void ReadXml(XmlReader reader)
        {
            // Read attributes from XML
            Path = reader.GetAttribute("path");
            Expression = reader.GetAttribute("expression");

            // If values are null, set defaults
            if (Path == null) Path = "";
            if (Expression == null) Expression = "$";

            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            // Write attributes to XML
            writer.WriteAttributeString("path", Path);
            writer.WriteAttributeString("expression", Expression);
        }

        public override void execute(
            CacheCollection cacheCollection, 
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            if (Path == "" || cacheCollection?.proSimCache == null || !cacheCollection.proSimCache.IsConnected())
                return;

            try
            {
                // Start with the expression
                string expression = Expression;
                
                // Process replacements
                List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();

                // Add event value replacement
                if (expression.Contains("@"))
                {
                    replacements.Add(new Tuple<string, string>("@", args.Value.ToString()));
                }

                // Add current value replacement if using $ placeholder
                if (expression.Contains("$"))
                {
                    // Get the current value from dataref (could be any type)
                    object currentValue = cacheCollection.proSimCache.readDataref(Path);
                    replacements.Add(new Tuple<string, string>("$", currentValue?.ToString() ?? "0"));
                }

                // Add config reference replacements
                if (configRefs != null)
                {
                    foreach (ConfigRefValue item in configRefs)
                    {
                        if (item.ConfigRef != null && !string.IsNullOrEmpty(item.ConfigRef.Placeholder))
                        {
                            replacements.Add(new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value));
                        }
                    }
                }

                // Apply all replacements
                string value = Replace(expression, replacements);

                // Try different conversions based on the value format
                object convertedValue = null;
                
                // Try as float first (most common)
                if (float.TryParse(value, out float floatValue))
                {
                    convertedValue = floatValue;
                }
                // Try as boolean if value is "true" or "false"
                else if (bool.TryParse(value, out bool boolValue))
                {
                    convertedValue = boolValue;
                }
                // Try as integer
                else if (int.TryParse(value, out int intValue))
                {
                    convertedValue = intValue;
                }
                // If all else fails, use the string value
                else
                {
                    convertedValue = value;
                }
                
                // Write the converted value to the dataref
                cacheCollection.proSimCache.writeDataref(Path, convertedValue);
                Log.Instance.log($"ProSim Input Action executed: Writing {convertedValue} to {Path}", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"ProSim Input Action failed: {ex.Message}", LogSeverity.Error);
            }
        }


        public override bool Equals(object obj)
        {
            return obj != null && obj is ProSimInputAction &&
                   Path == (obj as ProSimInputAction).Path &&
                   Expression == (obj as ProSimInputAction).Expression;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode() ^ Expression.GetHashCode();
        }
    }
} 