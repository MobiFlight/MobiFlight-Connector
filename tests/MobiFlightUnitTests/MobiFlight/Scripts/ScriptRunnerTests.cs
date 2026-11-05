using MobiFlightMoza.Protocol;
using MobiFlightWwFcu;
using Newtonsoft.Json;

namespace MobiFlight.Scripts.Tests
{
    [TestClass()]
    public class ScriptRunnerTests
    {
        private const string WINCTRL_VENDOR_ID = "0x4098";
        private const string MOZA_VENDOR_ID = "0x346E";
        private const string UNKNOWN_VENDOR_ID = "0x9999";

        [TestMethod()]
        public void ResolveHardwareIds_WithWinCtrlVendorId_ReturnsAllWinCtrlCduProductIds()
        {
            var result = ScriptRunner.ResolveHardwareIds(new[] { WINCTRL_VENDOR_ID }).ToList();

            Assert.HasCount(WinCtrlConstants.CDU_PRODUCTIDS.Length, result);
            foreach (var productId in WinCtrlConstants.CDU_PRODUCTIDS)
            {
                Assert.Contains(id => id.VendorId == WINCTRL_VENDOR_ID && id.ProductId == productId.ToString("X"), result,
                    $"Expected product ID {productId:X} for vendor {WINCTRL_VENDOR_ID} to be present.");
            }
        }

        [TestMethod()]
        public void ResolveHardwareIds_WithMozaVendorId_ReturnsAllMozaCduProductIds()
        {
            var result = ScriptRunner.ResolveHardwareIds(new[] { MOZA_VENDOR_ID }).ToList();

            Assert.HasCount(MozaHardwareIds.McduProductIds.Length, result);
            foreach (var productId in MozaHardwareIds.McduProductIds)
            {
                Assert.Contains(id => id.VendorId == MOZA_VENDOR_ID && id.ProductId == productId.ToString("X"), result,
                    $"Expected product ID {productId:X} for vendor {MOZA_VENDOR_ID} to be present.");
            }
        }

        [TestMethod()]
        public void ResolveHardwareIds_WithBothVendorIds_ReturnsUnionOfBothVendorsProductIds()
        {
            var result = ScriptRunner.ResolveHardwareIds(new[] { WINCTRL_VENDOR_ID, MOZA_VENDOR_ID }).ToList();

            Assert.HasCount(WinCtrlConstants.CDU_PRODUCTIDS.Length + MozaHardwareIds.McduProductIds.Length, result);
            Assert.Contains(id => id.VendorId == WINCTRL_VENDOR_ID, result);
            Assert.Contains(id => id.VendorId == MOZA_VENDOR_ID, result);
        }

        [TestMethod()]
        public void ResolveHardwareIds_WithUnknownVendorId_ReturnsEmpty()
        {
            var result = ScriptRunner.ResolveHardwareIds(new[] { UNKNOWN_VENDOR_ID }).ToList();

            Assert.IsEmpty(result);
        }

        [TestMethod()]
        public void ResolveHardwareIds_WithInvalidVendorIdFormat_SkipsInvalidEntryOnly()
        {
            var result = ScriptRunner.ResolveHardwareIds(new[] { "not-hex", WINCTRL_VENDOR_ID }).ToList();

            Assert.HasCount(WinCtrlConstants.CDU_PRODUCTIDS.Length, result);
            Assert.IsTrue(result.All(id => id.VendorId == WINCTRL_VENDOR_ID));
        }

        [TestMethod()]
        public void ResolveHardwareIds_WithEmptyVendorIds_ReturnsEmpty()
        {
            var result = ScriptRunner.ResolveHardwareIds(new string[0]).ToList();

            Assert.IsEmpty(result);
        }

        [TestMethod()]
        public void GetHardwareId_WithValidHexIds_ReturnsConcatenatedDecimalValues()
        {
            var hardwareId = ScriptRunner.GetHardwareId("0x4098", "BB36");

            Assert.AreEqual("1653647926", hardwareId);
        }

        [TestMethod()]
        public void GetHardwareId_IsCaseAndPrefixInsensitive()
        {
            var withPrefixAndUpperCase = ScriptRunner.GetHardwareId("0x4098", "BB36");
            var withoutPrefixAndLowerCase = ScriptRunner.GetHardwareId("4098", "bb36");

            Assert.AreEqual(withPrefixAndUpperCase, withoutPrefixAndLowerCase);
        }

        [TestMethod()]
        public void GetHardwareId_WithInvalidFormat_ReturnsEmptyString()
        {
            var hardwareId = ScriptRunner.GetHardwareId("not-hex", "BB36");

            Assert.AreEqual(string.Empty, hardwareId);
        }

        [TestMethod()]
        public void ResolveHardwareIds_ProductIdsMatchGetHardwareIdOfKnownDevice()
        {
            // The captain-side WinCtrl MCDU should resolve to the same hardware ID whether it's
            // looked up via a VendorId-only lookup or built directly from its known product ID.
            var expectedHardwareId = ScriptRunner.GetHardwareId(WINCTRL_VENDOR_ID, WinCtrlConstants.PRODUCT_ID_MCDU_CPT.ToString("X"));

            var resolvedHardwareIds = ScriptRunner.ResolveHardwareIds(new[] { WINCTRL_VENDOR_ID })
                .Select(id => ScriptRunner.GetHardwareId(id.VendorId, id.ProductId))
                .ToList();

            Assert.Contains(expectedHardwareId, resolvedHardwareIds);
        }

        [TestMethod()]
        public void ScriptMappings_Deserialize_VendorIdsAreSharedAtTopLevel()
        {
            var json = @"{
                ""VendorIds"": [ ""0x4098"", ""0x346E"" ],
                ""Mappings"": [
                    { ""AircraftMatchPattern"": ""pmdg 737"", ""ScriptName"": ""pmdg_737_winwing_cdu.py"" },
                    { ""AircraftMatchPattern"": ""pmdg 777"", ""ScriptName"": ""pmdg_777_winwing_cdu.py"" }
                ]
            }";

            var definitions = JsonConvert.DeserializeObject<ScriptMappings>(json);

            CollectionAssert.AreEqual(new[] { WINCTRL_VENDOR_ID, MOZA_VENDOR_ID }, definitions.VendorIds);
            Assert.HasCount(2, definitions.Mappings);
            Assert.AreEqual("pmdg_737_winwing_cdu.py", definitions.Mappings[0].ScriptName);
            Assert.AreEqual("pmdg 777", definitions.Mappings[1].AircraftMatchPattern);
        }
    }
}
