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
            // let the display know what size it is
            o.Cols = 20;
            o.Lines = 1;

            // Test1: one line
            String value = "12345";
            List<Tuple<String, String>> replacements = new List<Tuple<string, string>>();

            lcdConfig.Lines.Add("COM1: $$$.$$");

            String result = "";
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        ", result, "Apply was not correct");
            
            // Test2: two lines, but only one configured
            lcdConfig.Lines.Add("COM2: ###.##");
            replacements.Add(new Tuple<string, string> ("#", "12345"));
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        ", result, "Apply was not correct");

            // Test3: two lines and two lines configured
            o.Lines = 2;
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        "+
                            "COM2: 123.45        ", result, "Apply was not correct");
            
            replacements.Clear();
            replacements.Add(new Tuple<string, string>("#", "123")); // too short, padding needed
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        " +
                            "COM2:   1.23        ", result, "Apply was not correct. Wrong padding.");

            replacements.Clear();
            replacements.Add(new Tuple<string, string>("$", "")); // zero length string, replace them all
            replacements.Add(new Tuple<string, string>("#", "123")); // too short, padding needed
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1:    .          " +
                            "COM2:   1.23        ", result, "Apply was not correct. Wrong replacement of zero length string.");

            replacements.Clear();
            replacements.Add(new Tuple<string, string>("$", null)); // zero length string, replace them all
            replacements.Add(new Tuple<string, string>("#", "123")); // too short, padding needed
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1:    .          " +
                            "COM2:   1.23        ", result, "Apply was not correct. Wrong replacement of zero length string.");

        }
    }
}