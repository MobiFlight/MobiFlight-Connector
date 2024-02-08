using System;

namespace MobiFlight.BrowserMessages
{
    public class Message<T>
    {
        public String key
        {
            get
            {
                return payload.GetType().Name;
            }
        }

        public T payload { get; set; }
        public Message(T payload)
        {
            this.payload = payload;
        }
        
    }
}
