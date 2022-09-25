namespace MobiFlight.RestWebSocketApi.Messages
{
    internal interface IMessage
    {
        string MessageType { get; set; }
        MessageTypeEnum msgType { get; }
    }


}
