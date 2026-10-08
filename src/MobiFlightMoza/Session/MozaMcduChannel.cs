using System;
using MobiFlightMoza.Cdu;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Session
{
    /// <summary>
    /// Drives the 9050 MCDU channel: sends InitConfig, waits for ClientCapability, then
    /// forwards submitted pages through <see cref="McduFrameBuilder"/>. A page submitted
    /// before capability arrives is held and sent as soon as it does.
    /// </summary>
    internal sealed class MozaMcduChannel
    {
        private readonly ReliableStreamMultiplexer Multiplexer;
        private readonly NetworkPackageExtractor Extractor = new();
        private readonly McduFrameBuilder FrameBuilder = new();

        private CduPage PendingPage;

        public McduClientCapability? Capability { get; private set; }

        public event Action<McduClientCapability> CapabilityReceived;

        public MozaMcduChannel(ReliableStreamMultiplexer multiplexer)
        {
            Multiplexer = multiplexer;
        }

        public void Start()
        {
            Capability = null;
            FrameBuilder.Reset();
            Multiplexer.TrySend(MozaConstants.ServicePortMcduTcp, MozaMcduFrame.PackInitConfig(McduInitConfig.Default));
        }

        public void OnApplicationData(byte[] data)
        {
            foreach (var package in Extractor.Feed(data))
            {
                if (package.PackageId != 0x33) continue;
                if (!MozaMcduFrame.TryParseClientCapability(package.Payload, out var capability)) continue;

                Capability = capability;
                CapabilityReceived?.Invoke(capability);
                TrySendPendingPage();
            }
        }

        public void SubmitPage(CduPage page)
        {
            PendingPage = page;
            TrySendPendingPage();
        }

        // Forces a fresh full keyframe even if PendingPage is unchanged - BuildNext()
        // normally suppresses a resend of identical content, which would otherwise silently
        // swallow a periodic keep-alive resend of the same page. Uses ForceKeyframe, not
        // Reset+BuildNext: Reset would restart Sequence back at 1, and only a strictly
        // increasing Sequence is accepted, so every resend after the first would be
        // silently rejected as stale.
        public void ForceResend()
        {
            if (!Capability.HasValue || PendingPage == null) return;
            Multiplexer.TrySend(MozaConstants.ServicePortMcduTcp, FrameBuilder.ForceKeyframe(PendingPage));
        }

        private void TrySendPendingPage()
        {
            if (!Capability.HasValue || PendingPage == null) return;

            byte[] frame = FrameBuilder.BuildNext(PendingPage);
            if (frame != null)
            {
                Multiplexer.TrySend(MozaConstants.ServicePortMcduTcp, frame);
            }
        }
    }
}
