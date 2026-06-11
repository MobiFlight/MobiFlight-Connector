namespace MobiFlightWwFcu
{
    internal abstract class SimpleOutputDeviceBase : WinwingDeviceBase
    {
        protected SimpleOutputDeviceBase(IWinwingMessageSender sender, byte[] destinationAddress)
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
