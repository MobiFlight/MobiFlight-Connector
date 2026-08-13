namespace MobiFlightWwFcu
{
    internal abstract class SimpleOutputControllerBase : WinCtrlControllerBase
    {
        protected SimpleOutputControllerBase(IWinCtrlMessageSender sender, byte[] destinationAddress)
            : base(sender, destinationAddress)
        {
        }

        public override void Connect()
        {
            OnConnect();
        }

        public override void Shutdown()
        {
            OnShutdown();
            TurnOffAllLEDs();
        }

        protected virtual void OnConnect()  { }
        protected virtual void OnShutdown() { }
    }
}
