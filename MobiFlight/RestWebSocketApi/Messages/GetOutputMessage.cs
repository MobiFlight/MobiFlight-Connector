namespace MobiFlight.RestWebSocketApi.Messages
{
    internal class GetOutputMessage: BaseMessage
    {
        public string Output { get; set; }
        public string Value { get; set; }

        public GetOutputMessage(string output, string value) : base(MessageTypeEnum.GET_OUTPUT)
        {
            Output = output;
            Value = value;
        }

        public GetOutputMessage(string output) : base(MessageTypeEnum.GET_OUTPUT)
        {
            Output = output;
        }

        public GetOutputMessage(): base(MessageTypeEnum.GET_OUTPUT) { }
    }


}
