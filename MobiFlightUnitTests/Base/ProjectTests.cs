using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.Fail();
        }
    }
}