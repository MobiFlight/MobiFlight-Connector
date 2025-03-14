using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ProjectTests
    {
        [TestMethod()]
        public void OpenFileTest_Single_Xml()
        {
            string inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            var o = new Project();
            Assert.IsNotNull(o);

            o.FilePath = inFile;
            o.OpenFile();

            Assert.IsNotNull(o.ConfigFiles);
            Assert.IsTrue(o.ConfigFiles.Count > 0);

            var config = o.ConfigFiles[0];
            var inputConfigs = config.ConfigItems.Where(i => i is InputConfigItem);

            Assert.IsNotNull(inputConfigs);
            Assert.IsTrue(inputConfigs.Count() > 0);

            var outputConfigs = config.ConfigItems.Where(i => i is OutputConfigItem);
            Assert.IsNotNull(outputConfigs);
            Assert.IsTrue(outputConfigs.Count() > 0);
        }

        [TestMethod()]
        public void OpenFileTest_Single_Json_Embedded()
        {
            string inFile = @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";
            var o = new Project();
            Assert.IsNotNull(o);

            o.FilePath = inFile;
            o.OpenFile();

            Assert.IsNotNull(o.ConfigFiles);
            Assert.IsTrue(o.ConfigFiles.Count > 0);

            var config = o.ConfigFiles[0];
            var inputConfigs = config.ConfigItems.Where(i => i is InputConfigItem);

            Assert.IsNotNull(inputConfigs);
            Assert.IsTrue(inputConfigs.Count() > 0);

            var outputConfigs = config.ConfigItems.Where(i => i is OutputConfigItem);
            Assert.IsNotNull(outputConfigs);
            Assert.IsTrue(outputConfigs.Count() > 0);
        }

        [TestMethod()]
        public void SaveFileTest()
        {
            string inFile = @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";
            var o = new Project();
            o.FilePath = inFile;
            o.OpenFile();

            string outFile = @"assets\Base\ConfigFile\Json\SaveProjectTest.mfproj";
            o.FilePath = outFile;
            o.SaveFile();

            var o2 = new Project();
            o2.FilePath = outFile;
            o2.OpenFile();
            Assert.IsNotNull(o2.ConfigFiles);
            Assert.AreEqual(o, o2);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var o = new Project();
            o.Name = "Test";
            o.FilePath = "TestPath";

            var o2 = new Project();
            o2.Name = "Test";
            o2.FilePath = "TestPath";
            Assert.IsTrue(o2.Equals(o));

            o.ConfigFiles.Add(new ConfigFile());
            Assert.IsFalse(o2.Equals(o));

            o2.ConfigFiles.Add(new ConfigFile());
            Assert.IsTrue(o2.Equals(o));

            var ici1 = new InputConfigItem();
            var ici2 = new InputConfigItem(ici1);

            o2.ConfigFiles[0].ConfigItems.Add(ici1);
            Assert.IsFalse(o2.Equals(o));

            o.ConfigFiles[0].ConfigItems.Add(ici2);
            Assert.IsTrue(o2.Equals(o));
        }
    }
}