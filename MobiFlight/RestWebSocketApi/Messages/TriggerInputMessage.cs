namespace MobiFlight.RestWebSocketApi.Messages
{
    internal class TriggerInputMessage: BaseMessage
    {
        public string Input { get; set; }
        public string Value { get; set; }

        public TriggerInputMessage(string input, string value): base(MessageTypeEnum.TRIGGER_INPUT)
        {
            Input = input;
            Value = value;
        }

        public TriggerInputMessage(): base(MessageTypeEnum.TRIGGER_INPUT) { }
    }


}
