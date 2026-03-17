using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Incoming.Converter.Tests
{
    [TestClass()]
    public class InputActionConverterTests
    {
        private JsonSerializerSettings _serializerSettings;
        private List<Type> _inputActionTypes;

        [TestInitialize]
        public void Setup()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new InputActionConverter() }
            };

            _inputActionTypes = InputActionFactory.GetAllInputActionTypes();
        }

        private void SetProperties(InputAction action)
        {
            if (action is MSFS2020CustomInputAction msfsAction)
            {
                msfsAction.Command = "TestCommand";
                msfsAction.PresetId = "TestPresetId";
            }
            else if (action is VariableInputAction variableAction)
            {
                variableAction.Variable = new MobiFlightVariable()
                {
                    Name = "TestVariable"
                };
            }
            else if (action is EventIdInputAction eventIdAction)
            {
                eventIdAction.EventId = 12345;
            }
            else if (action is JeehellInputAction jeehellAction)
            {
                jeehellAction.EventId = 123;
                jeehellAction.Param = "TestParam";
            }
            else if (action is XplaneInputAction xplaneAction)
            {
                xplaneAction.Expression = "TestExpression";
                xplaneAction.InputType = "TestInputType";
                xplaneAction.Path = "TestPath";
            }
            else if (action is KeyInputAction keyAction)
            {
                keyAction.Key = System.Windows.Forms.Keys.V;
            }
            else if (action is LuaMacroInputAction luaAction)
            {
                luaAction.MacroValue = "TestMacro";
            }
            else if (action is RetriggerInputAction retriggerAction)
            {
            }
            else if (action is FsuipcOffsetInputAction fsuipcAction)
            {
                fsuipcAction.Value = "12345";
                fsuipcAction.FSUIPC = new OutputConfig.FsuipcOffset()
                {
                    BcdMode = false,
                    Offset = 123,
                    OffsetType = FSUIPCOffsetType.Integer,
                    Size = 1,
                    Mask = 0xBA
                };
            }
            else if (action is PmdgEventIdInputAction pmdgAction)
            {
                pmdgAction.EventId = 12345;
                pmdgAction.Param = "TestParam";
            }
            else if (action is VJoyInputAction vjoyAction)
            {
                vjoyAction.sendValue = "12345";
                vjoyAction.axisString = "TestAxisString";
                vjoyAction.buttonComand = true;
                vjoyAction.buttonNr = 12;
                vjoyAction.vJoyID = 1;
            }

            // Add other InputAction types and set their properties here
        }

        [TestMethod()]
        public void CanConvertTest()
        {
            var converter = new InputActionConverter();
            foreach (var type in _inputActionTypes)
            {
                Assert.IsTrue(converter.CanConvert(type));
            }
        }

        [TestMethod()]
        public void WriteJsonTest()
        {
            foreach (var type in _inputActionTypes)
            {
                var originalAction = new MSFS2020CustomInputAction() as InputAction;
                SetProperties(originalAction);
                var json = JsonConvert.SerializeObject(originalAction, _serializerSettings);
                Assert.IsFalse(string.IsNullOrEmpty(json));
            }
        }

        [TestMethod()]
        public void ReadJsonTest()
        {
            foreach (var type in _inputActionTypes)
            {
                var originalAction = Activator.CreateInstance(type) as InputAction;
                SetProperties(originalAction);
                var json = JsonConvert.SerializeObject(originalAction, _serializerSettings);
                var deserializedAction = JsonConvert.DeserializeObject<InputAction>(json, _serializerSettings);
                Assert.AreEqual(originalAction, deserializedAction);
            }
        }
    }
}