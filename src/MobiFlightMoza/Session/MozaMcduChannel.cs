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
        private bool PlaceholderSent;

        public McduClientCapability? Capability { get; private set; }

        public event Action<McduClientCapability> CapabilityReceived;

        public MozaMcduChannel(ReliableStreamMultiplexer multiplexer)
        {
            Multiplexer = multiplexer;
        }

        public void Start()
        {
            Capability = null;
            PlaceholderSent = false;
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
        // Reset+BuildNext: Reset would restart Sequence back at 1, and the guide only
        // accepts a strictly increasing Sequence, so every resend after the first would be
        // silently rejected as stale.
        public void ForceResend()
        {
            if (!Capability.HasValue || PendingPage == null) return;
            Multiplexer.TrySend(MozaConstants.ServicePortMcduTcp, FrameBuilder.ForceKeyframe(PendingPage));
        }

        private void TrySendPendingPage()
        {
            if (!Capability.HasValue || PendingPage == null) return;

            // EXPERIMENTAL - not in the guide. A USB capture of MOZA's own Cockpit app
            // showed it never sends the same static content twice on connect: with no sim
            // running it still alternates between its own placeholder screens ("WAITING FOR
            // GAME DATA" / its MCDU menu) rather than resending one unchanged page. Testing
            // whether the device needs a genuine content change to repaint after falling
            // back to its own HOMEPAGE idle state - MobiFlight has always sent the exact
            // same bytes on every connect, cold or warm, and only the cold one ever renders.
            if (!PlaceholderSent)
            {
                PlaceholderSent = true;
                byte[] placeholder = FrameBuilder.BuildNext(CduPage.CreateBlank());
                if (placeholder != null)
                {
                    Multiplexer.TrySend(MozaConstants.ServicePortMcduTcp, placeholder);
                }
            }

            byte[] frame = FrameBuilder.BuildNext(PendingPage);
            if (frame != null)
            {
                Multiplexer.TrySend(MozaConstants.ServicePortMcduTcp, frame);
            }
        }
    }
}
