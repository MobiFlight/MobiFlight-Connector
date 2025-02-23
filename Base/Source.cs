using MobiFlight.Base.Serialization.Json;
using MobiFlight.OutputConfig;
using MobiFlight.xplane;
using Newtonsoft.Json;

namespace MobiFlight.Base
{
    [JsonConverter(typeof(SourceConverter))]
    public abstract class Source
    {
        public string Type { get { return GetType().Name; } }

        [JsonIgnore]
        public abstract string SourceType { get; }
        public abstract object Clone();

        public abstract override bool Equals(object obj);
    }

    public class FsuipcSource : Source
    {
        public override string SourceType => "FSUIPC";
        public FsuipcOffset FSUIPC { get; set; } = new FsuipcOffset();

        public override object Clone()
        {
            return new FsuipcSource
            {
                FSUIPC = (FsuipcOffset)FSUIPC.Clone()
            };
        }

        public override bool Equals(object obj)
        {
            return (obj != null) && (obj is FsuipcSource) &&
                this.FSUIPC.Equals((obj as FsuipcSource).FSUIPC);
        }
    }

    public class SimConnectSource : Source
    {
        public override string SourceType => "SIMCONNECT";
        public SimConnectValue SimConnectValue { get; set; } = new SimConnectValue();

        public override object Clone()
        {
            return new SimConnectSource
            {
                SimConnectValue = (SimConnectValue)SimConnectValue.Clone()
            };
        }
        public override bool Equals(object obj) {
            return (obj != null) && (obj is SimConnectSource) &&
                this.SimConnectValue.Equals((obj as SimConnectSource).SimConnectValue);
        }
    }

    public class VariableSource : Source
    {
        public override string SourceType => "VARIABLE";
        public MobiFlightVariable MobiFlightVariable { get; set; } = new MobiFlightVariable();

        public override object Clone()
        {
            return new VariableSource
            {
                MobiFlightVariable = (MobiFlightVariable)MobiFlightVariable.Clone()
            };
        }

        public override bool Equals(object obj)
        {
            return (obj != null) && (obj is VariableSource) &&
                this.MobiFlightVariable.Equals((obj as VariableSource).MobiFlightVariable);
        }
    }

    public class XplaneSource : Source
    {
        public override string SourceType => "XPLANE";
        public XplaneDataRef XplaneDataRef { get; set; } = new XplaneDataRef();

        public override object Clone()
        {
            return new XplaneSource
            {
                XplaneDataRef = (XplaneDataRef)XplaneDataRef.Clone()
            };
        }
        public override bool Equals(object obj)
        {
            return (obj != null) && (obj is XplaneSource) &&
                this.XplaneDataRef.Equals((obj as XplaneSource).XplaneDataRef);
        }
    }
}
