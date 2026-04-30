using System;
using System.Collections.Generic;
using System.Xml;

namespace MobiFlight.Modifier
{
    internal class ExponentialAverage : ModifierBase
    {
        public double Alpha { get; set; } = 0.25;
        public double Threshold { get; set; } = 3;

        private double PreviousValue = double.MinValue;
        public double FilteredValue { get; private set; } = 0;

        public ExponentialAverage() { }

        public ExponentialAverage(ExponentialAverage other)
        {
            this.Active = other.Active;
            this.Alpha = other.Alpha;
            this.Threshold = other.Threshold;
            this.PreviousValue = other.PreviousValue;
            this.FilteredValue = other.FilteredValue;
        }

        public override object Clone()
        {
            ExponentialAverage Clone = new ExponentialAverage(this);
            return Clone;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ExponentialAverage)) return false;
            var other = obj as ExponentialAverage;
            return
                this.Active == other.Active &&
                this.Alpha == other.Alpha &&
                this.PreviousValue == other.PreviousValue;
        }

        override public void ReadXml(XmlReader reader)
        {
            // this class is not XML serializable anymore.
        }

        override public void WriteXml(XmlWriter writer)
        {
            // this class is not XML serializable anymore.
        }

        public override ConnectorValue Apply(ConnectorValue connectorValue, List<ConfigRefValue> configRefs)
        {
            if (!Active) return connectorValue;

            var result = connectorValue.Clone() as ConnectorValue;

            double currentValue = connectorValue.Float64;

            if (PreviousValue == double.MinValue)
            {
                PreviousValue = currentValue;
                FilteredValue = currentValue;
                result.Float64 = FilteredValue;
                return result;
            }

            PreviousValue = Alpha * currentValue + (1 - Alpha) * PreviousValue;

            int delta = (int)Math.Abs(PreviousValue - FilteredValue);
            bool filteredValueExceedsThreshold = delta > Threshold;

            if (filteredValueExceedsThreshold)
            {
                FilteredValue = PreviousValue;
            }

            result.Float64 = FilteredValue;

            return result;
        }

        public override string ToSummaryLabel()
        {
            return $"Exponential Average with Alpha = {Alpha}";
        }
    }
}