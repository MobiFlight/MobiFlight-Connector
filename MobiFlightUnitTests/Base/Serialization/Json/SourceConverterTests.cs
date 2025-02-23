using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MobiFlight.Base.Serialization.Json.Tests
{
    [TestClass()]
    public class SourceConverterTests
    {
        private JsonSerializerSettings _serializerSettings;
        private List<Type> _sourceTypes;

        [TestInitialize]
        public void Setup()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new SourceConverter() }
            };

            _sourceTypes = new List<Type>() {
                typeof(SimConnectSource),
                typeof(VariableSource),
                typeof(XplaneSource),
                typeof(FsuipcSource),
            };
        }

        private void SetProperties(Source original)
        {
            if (original is SimConnectSource simConnect)
            {
                simConnect.SimConnectValue = new SimConnectValue()
                {
                    UUID = Guid.NewGuid().ToString(),
                    VarType = SimConnectVarType.CODE,
                    Value = "TestValue"
                };
            }
            else if (original is VariableSource variable)
            {
                variable.MobiFlightVariable = new MobiFlightVariable()
                {
                    Name = "TestName",
                    Expression = "TestExpression",
                    Number = 123,
                    Text = "TestText",
                };
            }
            else if (original is XplaneSource xplane)
            {
                xplane.XplaneDataRef = new xplane.XplaneDataRef()
                {
                    Path = "TestPath",
                };
            }
            else if (original is FsuipcSource fsuipc)
            {
                fsuipc.FSUIPC = new FsuipcOffset()
                {
                    BcdMode = false,
                    Mask = 255,
                    Offset = 123,
                    OffsetType = FSUIPCOffsetType.Integer,
                    Size = 4,
                };
            }
        }

        [TestMethod()]
        public void ReadJsonTest()
        {
            foreach (var type in _sourceTypes)
            {
                var original = Activator.CreateInstance(type) as Source;
                SetProperties(original);
                var json = JsonConvert.SerializeObject(original, _serializerSettings);
                var deserialized = JsonConvert.DeserializeObject<Source>(json, _serializerSettings);
                Assert.AreEqual(original, deserialized);
            }
        }
    }
}