using MobiFlight.RestWebSocketApi.Messages;
using Swan;
using System;
using System.Collections.Generic;

namespace MobiFlight.RestWebSocketApi
{

    internal class MessageProcessor
    {
        public event ButtonEventHandler TriggerInputEvent;
        public List<string> InputEndpoints { get; set; }
        public Dictionary<string, string> OutputCache { get; set; }

        public MessageProcessor(Dictionary<string, string> outputCache)
        {
            OutputCache = outputCache;
        }

        public string ProcessJsonMessage(string msg)
        {
            try
            {
                IMessage jsonMsg = BaseMessage.Deserialize(msg);
                return ProcessMessage(jsonMsg);
            } catch(Exception e)
            {
                return new ErrorMessage($"Could not deserialize message ({e.Message})").Serialize();
            }
        }

        public string ProcessMessage(IMessage msg)
        {
            switch(msg.msgType)
            {
                case MessageTypeEnum.TRIGGER_INPUT:
                    return triggerInput((TriggerInputMessage)msg);
                case MessageTypeEnum.GET_OUTPUT:
                    return getOutput((GetOutputMessage)msg);
                case MessageTypeEnum.GET_ALL_OUTPUTS:
                    return getAllOutputs((GetAllOutputsMessage)msg);
            }

            return new ErrorMessage("Message could not be processed.").Serialize();
        }

        private string triggerInput(TriggerInputMessage msg)
        {
            if (!InputEndpoints.Contains(msg.Input))
            {
                return new ErrorMessage("Input not found").Serialize();
            }

            var inpEv = new InputEventArgs()
            {
                Serial = RestApiManager.serial,
                Type = DeviceType.AnalogInput,
                DeviceId = msg.Input,
                StrValue = msg.Value,
            };


            try
            {
                inpEv.Value = int.Parse(msg.Value);
            }
            catch (FormatException e) { }

            TriggerInputEvent.Invoke(null, inpEv);
            return new SuccessMessage().Serialize();
        }

        private string getOutput(GetOutputMessage msg)
        {
            if (!OutputCache.ContainsKey(msg.Output))
            {
                return new ErrorMessage("Output not found").Serialize();
            }

            return new GetOutputMessage(msg.Output, OutputCache.GetValueOrDefault(msg.Output)).Serialize();
        }

        private string getAllOutputs(GetAllOutputsMessage msg)
        {
            return new GetAllOutputsMessage(OutputCache).Serialize();
        }
    }
}
