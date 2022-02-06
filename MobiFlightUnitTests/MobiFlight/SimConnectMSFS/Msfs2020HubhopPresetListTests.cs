using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.SimConnectMSFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnectMSFS.Tests
{
    [TestClass()]
    public class Msfs2020HubhopPresetListTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\MobiFlight\SimConnectMSFS\Msfs2020HubhopPresetListTests\test01.json";
            list.Load(TestFile);
            Assert.AreEqual(3, list.Items.Count);
            Assert.AreEqual("Microsoft.Generic.Avionics.AS1000_PFD_VOL_1_INC", list.Items[0].path);
            Assert.AreEqual("Microsoft", list.Items[0].vendor);
            Assert.AreEqual("Generic", list.Items[0].aircraft);
            Assert.AreEqual("Avionics", list.Items[0].system);
            Assert.AreEqual("(>H:AS1000_PFD_VOL_1_INC)", list.Items[0].code);
            Assert.AreEqual("AS1000_PFD_VOL_1_INC", list.Items[0].label);
            Assert.AreEqual("Input", list.Items[0].presetType);
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
        public void VendorsTest()
        {
            Msfs2020HubhopPresetList list = new Msfs2020HubhopPresetList();
            String TestFile = @"assets\MobiFlight\SimConnectMSFS\Msfs2020HubhopPresetListTests\test01.json";
            list.Load(TestFile);

            Assert.AreEqual(2, list.AllVendors("Input").Count);
            Assert.AreEqual("Microsoft", list.AllVendors("Input")[0]);
            Assert.AreEqual("Just Flight", list.AllVendors("Input")[1]);
        }
    }
}