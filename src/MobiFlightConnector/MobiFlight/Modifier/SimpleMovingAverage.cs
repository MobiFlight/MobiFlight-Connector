using System;
using System.Collections.Generic;
using System.Xml;

namespace MobiFlight.Modifier
{
    internal class SimpleMovingAverage : ModifierBase
    {
        /// <summary>
        /// Number of samples in the moving average window.
        /// </summary>
        public int WindowSize { get; set; } = 8;

        /// <summary>
        /// Minimum change from last emitted value required to trigger an output update.
        /// </summary>
        public double Threshold { get; set; } = 32;

        /// <summary>
        /// If the raw value jumps by more than this in a single report the window
        /// is re-seeded immediately so fast movements are never suppressed.
        /// Default = 5 ADC steps on a 12-bit ADC mapped to 16-bit range (5 * 16 = 80).
        /// </summary>
        public double JumpResetThreshold { get; set; } = 64;

        // Circular buffer — avoids Queue heap allocations on every update
        private double[] _buffer;
        private int _bufferIndex = 0;
        private double _windowSum = 0;
        private bool _initialized = false;

        /// <summary>Current SMA output — updated on every call.</summary>
        public double SmoothedValue { get; private set; } = 0;

        /// <summary>Last emitted value — only updated when threshold is exceeded.</summary>
        public double FilteredValue { get; private set; } = 0;

        public SimpleMovingAverage() { }

        public SimpleMovingAverage(SimpleMovingAverage other)
        {
            Active = other.Active;
            WindowSize = other.WindowSize;
            Threshold = other.Threshold;
            JumpResetThreshold = other.JumpResetThreshold;
            SmoothedValue = other.SmoothedValue;
            FilteredValue = other.FilteredValue;
            _bufferIndex = other._bufferIndex;
            _windowSum = other._windowSum;
            _initialized = other._initialized;
            _buffer = other._buffer != null ? (double[])other._buffer.Clone() : null;
        }

        public override object Clone() => new SimpleMovingAverage(this);

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SimpleMovingAverage)) return false;
            var other = obj as SimpleMovingAverage;
            return Active == other.Active &&
                   WindowSize == other.WindowSize &&
                   Threshold == other.Threshold &&
                   JumpResetThreshold == other.JumpResetThreshold;
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
            double currentValue = connectorValue.Float64;

            // Large jump — window is stale, re-seed from new value
            if (_initialized && Math.Abs(currentValue - SmoothedValue) > JumpResetThreshold)
                _initialized = false;

            if (!_initialized)
            {
                _buffer = new double[WindowSize];
                for (int i = 0; i < WindowSize; i++) _buffer[i] = currentValue;
                _bufferIndex = 0;
                _windowSum = currentValue * WindowSize;
                SmoothedValue = currentValue;
                FilteredValue = currentValue;
                _initialized = true;
                result.Float64 = FilteredValue;
                return result;
            }

            // Subtract oldest sample, add newest — O(1), exact for integer values
            _windowSum -= _buffer[_bufferIndex];
            _buffer[_bufferIndex] = currentValue;
            _windowSum += currentValue;
            _bufferIndex = (_bufferIndex + 1) % WindowSize;

            SmoothedValue = _windowSum / WindowSize;

            if (Math.Abs(SmoothedValue - FilteredValue) >= Threshold)
                FilteredValue = SmoothedValue;

            result.Float64 = FilteredValue;
            return result;
        }

        public override string ToSummaryLabel()
        {
            return $"Simple Moving Average (Window={WindowSize}, Threshold={Threshold}, JumpResetThreshold={JumpResetThreshold})";
        }
    }
}