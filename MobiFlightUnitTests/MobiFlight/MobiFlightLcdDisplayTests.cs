using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Base;
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
            String value = "12345";
            String result = "";
            
            // let the display know what size it is
            o.Cols = 20;
            o.Lines = 1;

            /// -------------
            // Test1: one line
            List<ConfigRefValue> replacements = new List<ConfigRefValue>();
            value = "12345";
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("COM1: $$$.$$");
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        ", result, "Apply was not correct");
            
            /// -------------
            // Test2: two lines, but only one configured
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("COM1: $$$.$$");
            lcdConfig.Lines.Add("COM2: ###.##");
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "#"}, Value = "12345" });            
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        ", result, "Apply was not correct");

            /// -------------
            // Test3: two lines and two lines configured
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("COM1: $$$.$$");
            lcdConfig.Lines.Add("COM2: ###.##");
            o.Lines = 2;
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        "+
                            "COM2: 123.45        ", result, "Apply was not correct");
            
            /// -------------
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("COM1: $$$.$$");
            lcdConfig.Lines.Add("COM2: ###.##");
            replacements.Clear();
            replacements.Add(new ConfigRefValue {ConfigRef = new ConfigRef { Placeholder = "#"}, Value = "123" });             
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1: 123.45        " +
                            "COM2:   1.23        ", result, "Apply was not correct. Wrong padding.");

            /// -------------
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("COM1: $$$.$$");
            lcdConfig.Lines.Add("COM2: ###.##");
            //value = "";
            replacements.Clear();
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "$"}, Value = "" });  // zero length string, replace them all
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "#"}, Value = "123" }); // too short, padding needed
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1:    .          " +
                            "COM2:   1.23        ", result, "Apply was not correct. Wrong replacement of zero length string.");
            
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("COM1: $$$.$$");
            lcdConfig.Lines.Add("COM2: ###.##");
            //value = null;
            replacements.Clear();
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "$"}, Value = null });  // zero length string, replace them all
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "#"}, Value = "123" }); // too short, padding needed
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("COM1:    .          " +
                            "COM2:   1.23        ", result, "Apply was not correct. Wrong replacement of zero length string.");

            // replace placeholder by placeholder
            o.Lines = 1;
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("aaaa bbbb");
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "a" }, Value = "abcd" });
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "b" }, Value = "12345" });
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("abcd 2345           ", result, "Apply was not correct");


            // make sure too long lines don't break the logic
            o.Lines = 2;
            o.Cols = 20;
            lcdConfig.Lines.Clear();
            lcdConfig.Lines.Add("123456789012345678901");
            lcdConfig.Lines.Add("bbbbb                ");
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "a" }, Value = "abcd" });
            replacements.Add(new ConfigRefValue { ConfigRef = new ConfigRef { Placeholder = "b" }, Value = "12345" });
            result = o.Apply(lcdConfig, value, replacements);

            Assert.AreEqual("1234567890123456789012345               ", result, "Apply was not correct");
        }
    }
}