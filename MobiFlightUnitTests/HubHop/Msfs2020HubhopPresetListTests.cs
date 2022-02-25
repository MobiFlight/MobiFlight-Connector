using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.HubHop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.HubHop.Tests
{
    [TestClass()]
    public class Msfs2020HubhopPresetListTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\HubHop\Msfs2020HubhopPresetListTests\test01.json";
            list.Load(TestFile);
            Assert.AreEqual(7, list.Items.Count);
            Assert.AreEqual("Microsoft.Generic.Avionics.AS1000_PFD_VOL_1_INC", list.Items[0].path);
            Assert.AreEqual("Microsoft", list.Items[0].vendor);
            Assert.AreEqual("Generic", list.Items[0].aircraft);
            Assert.AreEqual("Avionics", list.Items[0].system);
            Assert.AreEqual("(>H:AS1000_PFD_VOL_1_INC)", list.Items[0].code);
            Assert.AreEqual("AS1000_PFD_VOL_1_INC", list.Items[0].label);
            Assert.AreEqual(HubHopType.Input, list.Items[0].presetType);
            Assert.AreEqual(34, list.Items[0].version);
            Assert.AreEqual("Updated", list.Items[0].status);
            Assert.AreEqual("2021-12-13T20:49:56.522Z", list.Items[0].createdDate);
            Assert.AreEqual("Mobiflight Community", list.Items[0].author);
            Assert.AreEqual("rofl-er", list.Items[0].updatedBy);
            Assert.AreEqual(0, list.Items[0].reported);
            Assert.AreEqual(0, list.Items[0].score);
            Assert.AreEqual("a6e93c4c-58ff-4729-acbb-5045c36c20ff", list.Items[0].id);
        }

        [TestMethod()]
        public void AllVendorsTest()
        {
            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\HubHop\Msfs2020HubhopPresetListTests\test01.json";
            list.Load(TestFile);

            Assert.AreEqual(3, list.AllVendors(HubHopType.Input).Count);

            // check for order
            Assert.AreEqual("Asobo", list.AllVendors(HubHopType.Input)[0]);
            Assert.AreEqual("Just Flight", list.AllVendors(HubHopType.Input)[1]);
            Assert.AreEqual("Microsoft", list.AllVendors(HubHopType.Input)[2]);

            // check for outputs
            Assert.AreEqual(2, list.AllVendors(HubHopType.Output).Count);

            // check for potentiometers
            Assert.AreEqual(1, list.AllVendors(HubHopType.InputPotentiometer).Count);

            // check for all inputs
            Assert.AreEqual(3, list.AllVendors(HubHopType.InputPotentiometer | HubHopType.Input).Count);
        }

        [TestMethod()]
        public void AllAircraftTest()
        {
            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\HubHop\Msfs2020HubhopPresetListTests\test01.json";
            list.Load(TestFile);

            Assert.AreEqual(3, list.AllAircraft(HubHopType.Input).Count);

            // check for order
            Assert.AreEqual("Generic", list.AllAircraft(HubHopType.Input)[0]);
            Assert.AreEqual("Hawk T1", list.AllAircraft(HubHopType.Input)[1]);
            Assert.AreEqual("TBM 580", list.AllAircraft(HubHopType.Input)[2]);

            // check for outputs
            Assert.AreEqual(2, list.AllAircraft(HubHopType.Output).Count);

            // check for potentiometers
            Assert.AreEqual(1, list.AllAircraft(HubHopType.InputPotentiometer).Count);

            // check for all inputs
            Assert.AreEqual(4, list.AllAircraft(HubHopType.InputPotentiometer | HubHopType.Input).Count);
        }

        [TestMethod()]
        public void AllSystemsTest()
        {
            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\HubHop\Msfs2020HubhopPresetListTests\test01.json";
            list.Load(TestFile);

            Assert.AreEqual(3, list.AllSystems(HubHopType.Input).Count);

            // check for order
            Assert.AreEqual("Avionics", list.AllSystems(HubHopType.Input)[0]);
            Assert.AreEqual("Flight Instrumentation", list.AllSystems(HubHopType.Input)[1]);
            Assert.AreEqual("Gear", list.AllSystems(HubHopType.Input)[2]);

            // check for outputs
            Assert.AreEqual(2, list.AllSystems(HubHopType.Output).Count);

            // check for potentiometers
            Assert.AreEqual(1, list.AllSystems(HubHopType.InputPotentiometer).Count);

            // check for all inputs
            Assert.AreEqual(4, list.AllAircraft(HubHopType.InputPotentiometer | HubHopType.Input).Count);
        }

        [TestMethod()]
        public void FindByCodeTest()
        {
            Msfs2020EventPresetList deprecatedList = new Msfs2020EventPresetList();
            deprecatedList.PresetFile = @"assets\HubHop\Msfs2020HubhopPresetListTests\test-events.txt";
            deprecatedList.Load();

            String code = deprecatedList.FindCodeByEventId("ASCRJ_AIRC_AFT_CARGO_AIRCOND_SWITCH");

            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\HubHop\Msfs2020HubhopPresetListTests\test-eventids.json";
            list.Load(TestFile);

            Msfs2020HubhopPreset preset = list.FindByCode(HubHopType.InputPotentiometer | HubHopType.Input, code);

            // (L:LandingLight_Position) 0 == if{(>L:LandingLight_Position,Number) 1 (>L:LandingLight_SwitchPosition, Number)}\n
            // (L:LandingLight_Position)·0·==·if{(>L:LandingLight_Position,Number)·0·(>L:LandingLight_SwitchPosition,·Number)} 
            Assert.IsNotNull(preset);
            Assert.AreEqual("6b07a2fa-abba-4a9c-9163-b103d8eda124", preset.id);

            // This one has line breaks in hubHop
            code = deprecatedList.FindCodeByEventId("ASCRJ_APU_STARTSTOP_RELEASE");
            preset = list.FindByCode(HubHopType.InputPotentiometer | HubHopType.Input, code);

            Assert.IsNotNull(preset);
            Assert.AreEqual("5ad9abcd-8777-41cb-a344-109719c6715c", preset.id);


            code = deprecatedList.FindCodeByEventId("ASCRJ_ECAM_AICE");
            preset = list.FindByCode(HubHopType.InputPotentiometer | HubHopType.Input, code);

            Assert.IsNotNull(preset);
            Assert.AreEqual("e1fb3d0a-9fe4-4439-948d-7023809d6d73", preset.id);
        }

        [TestMethod()]
        public void FindByUUIDTest()
        {
            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\HubHop\Msfs2020HubhopPresetListTests\test-eventids.json";
            list.Load(TestFile);

            Msfs2020HubhopPreset preset = list.FindByUUID(HubHopType.AllInputs, "167a047e-eee9-48e4-8101-398dc99d6ebb");
            Assert.IsNotNull(preset);
            Assert.AreEqual("AS1000_PFD_VOL_1_DEC", preset.label) ;
        }
    }
}