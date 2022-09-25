namespace MobiFlight.RestWebSocketApi.Messages
{
    internal class SuccessMessage: BaseMessage
    {
        public string Message { get; set; }
        public SuccessMessage(): base(MessageTypeEnum.SUCCESS) {
            Message = "ok";
        }
    }


}
