using System.Collections.Generic;

namespace MobiFlight.RestWebSocketApi.Messages
{
    internal class GetAllOutputsMessage: BaseMessage
    {
        public Dictionary<string, string> OutputValues { get; set; }
        public GetAllOutputsMessage(Dictionary<string, string> outputValues) : base(MessageTypeEnum.GET_ALL_OUTPUTS) {
            OutputValues = outputValues;
        }
        public GetAllOutputsMessage(): base(MessageTypeEnum.GET_ALL_OUTPUTS) { }
    }


}
