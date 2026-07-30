using System;
using System.Collections.Generic;
using System.Xml;

namespace MobiFlight.Modifier
{
    /// <summary>
    /// Quantizes input values to the nearest step size.
    /// Suppresses sub-step noise without any lag or windowing.
    /// Designed for ADCs with a known step size, e.g. a 12-bit ADC
    /// mapped to a 16-bit range has a step size of 16 (65535 / 4095 ≈ 16).
    /// </summary>
    internal class Quantize : ModifierBase
    {
        /// <summary>
        /// The quantization step size. Values are rounded to the nearest multiple.
        /// Default = 16 (one ADC step for a 12-bit ADC mapped to 0-65535).
        /// </summary>
        public double StepSize { get; set; } = 16;

        private double _lastEmitted = double.MinValue;

        public Quantize() { }

        public Quantize(Quantize other)
        {
            Active = other.Active;
            StepSize = other.StepSize;
            _lastEmitted = other._lastEmitted;
        }

        public override object Clone() => new Quantize(this);

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Quantize)) return false;
            var other = obj as Quantize;
            return Active == other.Active &&
                   StepSize == other.StepSize;
        }

        public override void ReadXml(XmlReader reader)
        {
            // Not XML serializable.
        }

        public override void WriteXml(XmlWriter writer)
        {
            // Not XML serializable.
        }

        public override ConnectorValue Apply(ConnectorValue connectorValue, List<ConfigRefValue> configRefs)
        {
            if (!Active) return connectorValue;

            var result = connectorValue.Clone() as ConnectorValue;

            double quantized = Math.Round(connectorValue.Float64 / StepSize) * StepSize;

            // Seed on first call
            if (_lastEmitted == double.MinValue)
            {
                _lastEmitted = quantized;
            }

            // Only emit when the quantized value actually changed
            if (quantized != _lastEmitted)
            {
                _lastEmitted = quantized;
            }

            result.Float64 = _lastEmitted;
            return result;
        }

        public override string ToSummaryLabel()
        {
            return $"Quantize (StepSize={StepSize})";
        }
    }
}