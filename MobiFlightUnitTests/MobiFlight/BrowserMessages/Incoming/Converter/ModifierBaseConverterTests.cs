using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Modifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Incoming.Converter.Tests
{
    [TestClass()]
    public class ModifierBaseConverterTests
    {
        private JsonSerializerSettings _serializerSettings;
        private List<Type> _modifierTypes;

        [TestInitialize]
        public void Setup()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new InputActionConverter() }
            };

            _modifierTypes = new List<Type>() {
                typeof(Blink),
                typeof(Comparison),
                typeof(Interpolation),
                typeof(Padding),
                typeof(Substring),
                typeof(Transformation),
            };
        }

        [TestMethod()]
        public void WriteJsonTest()
        {
            foreach (var type in _modifierTypes)
            {
                var originalModifier = Activator.CreateInstance(type) as ModifierBase;
                SetProperties(originalModifier);
                var json = JsonConvert.SerializeObject(originalModifier, _serializerSettings);
                var deserializedModifier = JsonConvert.DeserializeObject<ModifierBase>(json, _serializerSettings);
                Assert.AreEqual(originalModifier, deserializedModifier);
            }
        }

        private void SetProperties(ModifierBase originalModifier)
        {
            if (originalModifier is Blink blink)
            {
                blink.BlinkValue = "  ";
                blink.FirstExecutionTime = 100;
                blink.OffDurationInMs = 200;
                blink.OnOffSequence = new List<int>() { 200, 300 };
            }
            else if (originalModifier is Comparison compare)
            {
                compare.IfValue = "0";
                compare.ElseValue = "1";
                compare.Value = "TestValue";
                compare.Operand = "=";
            }
            else if (originalModifier is Interpolation interpolation)
            {
                interpolation.Add(3000, 6000);
            }
            else if (originalModifier is Padding padding)
            {
                padding.Character = '0';
                padding.Length = 10;
                padding.Direction = Padding.PaddingDirection.Left;
            }
            else if (originalModifier is Substring substring)
            {
                substring.End = 10;
                substring.Start = 0;
            }
            else if (originalModifier is Transformation transformation)
            {
                transformation.Expression = "$ + 1";
            }
        }
    }
}