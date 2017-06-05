using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class MobiFlightLcdDisplayTests
    {
        [TestMethod()]
        public void ApplyTest()
        {
            MobiFlightLcdDisplay o = new MobiFlightLcdDisplay();
            OutputConfig.LcdDisplay lcdConfig = new OutputConfig.LcdDisplay();
            o.Cols = 20;
            o.Lines = 1;

            String value = "12345";
            List<Tuple<String, String>> replacements = new List<Tuple<string, string>>();

            lcdConfig.Lines.Add("COM1: $$$.$$");

            String result = "";
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        ", result, "Apply was not correct");
        }
    }
}