namespace MobiFlight.RestWebSocketApi.Messages
{
    internal class OutputChangedMessage: BaseMessage
    {
        public string Output { get; set; }
        public string Value { get; set; }

        public OutputChangedMessage(string output, string value): base(MessageTypeEnum.OUTPUT_CHANGED)
        {
            this.Output = output;
            this.Value = value;
        }

        public OutputChangedMessage(): base(MessageTypeEnum.OUTPUT_CHANGED) { }
    }


}
