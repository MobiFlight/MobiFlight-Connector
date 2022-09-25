using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.RestWebSocketApi.Messages
{

    class MessageConverter : CustomCreationConverter<IMessage>
    {
        public override IMessage Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public IMessage Create(Type objectType, JObject jobject)
        {
            string type = (string)jobject.Property("MessageType");
            MessageTypeEnum msgType = (MessageTypeEnum)Enum.Parse(typeof(MessageTypeEnum), type, true);

            switch (msgType)
            {
                case MessageTypeEnum.OUTPUT_CHANGED:
                    return new OutputChangedMessage();
                case MessageTypeEnum.TRIGGER_INPUT:
                    return new TriggerInputMessage();
                case MessageTypeEnum.GET_ALL_OUTPUTS:
                    return new GetAllOutputsMessage();
                case MessageTypeEnum.ERROR:
                    return new ErrorMessage();
                case MessageTypeEnum.GET_OUTPUT:
                    return new GetOutputMessage();
                case MessageTypeEnum.SUCCESS:
                    return new SuccessMessage();
            }

            throw new ApplicationException($"Unknown message type! ${type}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject job = JObject.Load(reader);
            var target = Create(objectType, job);
            serializer.Populate(job.CreateReader(), target);
            return target;
        }

    }


}
