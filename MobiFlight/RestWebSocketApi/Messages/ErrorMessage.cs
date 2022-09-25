namespace MobiFlight.RestWebSocketApi.Messages
{
    internal class ErrorMessage: BaseMessage
    {
        public string ErrorMsg { get; set; }
        public ErrorMessage(string msg) : base(MessageTypeEnum.ERROR)
        {
            this.ErrorMsg = msg;
        }

        public ErrorMessage(): base(MessageTypeEnum.ERROR) { }
    }


}
