using MobiFlight;

namespace MobiFlightUnitTests.Helpers
{
    // Testable subclass to expose internal members for testing
    public class TestableMobiFlightCache : MobiFlightCache
    {
        public void AddTestModule(string serial, MobiFlightModule module)
        {
            // Use reflection to access the private Modules dictionary
            var modulesField = typeof(MobiFlightCache).GetField("Modules",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var modules = modulesField.GetValue(this) as System.Collections.Concurrent.ConcurrentDictionary<string, MobiFlightModule>;
            modules.TryAdd(serial, module);
        }

        public void AddTestModuleAsIfItWasDetected(MobiFlightModule module)
        {
            base.OnMobiFlightBoardDetected(module);
        }
    }
}
