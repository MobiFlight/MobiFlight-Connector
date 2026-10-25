namespace MobiFlight.Joysticks.Moza
{
    internal class MozaMxg1000 : MozaBaseController
    {
        public override string Name => Definition?.InstanceName ?? "MOZA MGX1000 Instrument Panel";

        public MozaMxg1000(JoystickDefinition definition)
            : base(definition)
        {
        }
    }
}
