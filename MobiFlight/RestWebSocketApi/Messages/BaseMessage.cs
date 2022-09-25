using Newtonsoft.Json;
using Swan.Formatters;
using System;

namespace MobiFlight.RestWebSocketApi.Messages
{
    internal abstract class BaseMessage: IMessage
    {
        public string MessageType { get; set; }
        public MessageTypeEnum msgType { 
            get { 
                return (MessageTypeEnum)Enum.Parse(typeof(MessageTypeEnum), MessageType, true); 
            } 
        }

        public BaseMessage(MessageTypeEnum messageType)
        {
            switch(messageType)
            {
                case MessageTypeEnum.ERROR:
                    MessageType = "ERROR";
                    break;
                case MessageTypeEnum.OUTPUT_CHANGED:
                    MessageType = "OUTPUT_CHANGED";
                    break;
                case MessageTypeEnum.TRIGGER_INPUT:
                    MessageType = "TRIGGER_INPUT";
                    break;
                case MessageTypeEnum.GET_OUTPUT:
                    MessageType = "GET_OUTPUT";
                    break;
                case MessageTypeEnum.GET_ALL_OUTPUTS:
                    MessageType = "GET_ALL_OUTPUTS";
                    break;
                case MessageTypeEnum.SUCCESS:
                    MessageType = "SUCCESS";
                    break;
            }
        }


        public string Serialize()
        {
            return Json.Serialize(this, excludedNames: new[] {"msgType"});
        }

        public static IMessage Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<IMessage>(json, new MessageConverter());
        }

    }


}
