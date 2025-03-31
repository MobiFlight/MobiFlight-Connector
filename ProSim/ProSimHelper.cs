using System;
using MobiFlight;
using MobiFlight.Base;
using System.Collections.Generic;
using ProSimSDK;

namespace MobiFlight.ProSim
{
    public static class ProSimHelper
    {
        /// <summary>
        /// Reads a value from a ProSim dataref and returns it as a ConnectorValue
        /// </summary>
        public static ConnectorValue executeRead(string dataRefName, ProSimCacheInterface proSimCache)
        {
            ConnectorValue result = new ConnectorValue();
            
            try
            {
                if (string.IsNullOrEmpty(dataRefName) || proSimCache == null || !proSimCache.IsConnected())
                    return result;
                
                // Read the raw value
                object rawValue = proSimCache.readDataref(dataRefName);
                
                // Convert based on the actual type
                if (rawValue != null)
                {
                    // Handle different data types appropriately


                    result.Float64 = (double)rawValue;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error reading ProSim dataref '{dataRefName}': {ex.Message}", LogSeverity.Error);
            }
            
            return result;
        }

        /// <summary>
        /// Writes a value to a ProSim dataref
        /// </summary>
        public static void executeWrite(string value, IProSimConfigItem cfg, ProSimCacheInterface proSimCache)
        {
            try
            {
                if (cfg == null || cfg.ProSim == null || string.IsNullOrEmpty(cfg.ProSim.Path) || 
                    proSimCache == null || !proSimCache.IsConnected())
                    return;
                
                // Try different conversions based on format/context
                object convertedValue = null;
                
                // Try as float first (most common)
                if (float.TryParse(value, out float floatValue))
                {
                    convertedValue = floatValue;
                }
                // Try as boolean
                else if (bool.TryParse(value, out bool boolValue))
                {
                    convertedValue = boolValue;
                }
                // Try as integer
                else if (int.TryParse(value, out int intValue))
                {
                    convertedValue = intValue;
                }
                // Default to string
                else
                {
                    convertedValue = value;
                }
                
                // Write the converted value
                proSimCache.writeDataref(cfg.ProSim.Path, convertedValue);
                Log.Instance.log($"Written to ProSim dataref '{cfg.ProSim.Path}': {convertedValue}", LogSeverity.Debug);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error writing to ProSim dataref: {ex.Message}", LogSeverity.Error);
            }
        }
    }

    public interface IProSimConfigItem
    {
        ProSimDataRef ProSim { get; }
    }
} 