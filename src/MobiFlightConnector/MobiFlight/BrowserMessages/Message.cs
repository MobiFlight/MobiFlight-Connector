using System;

namespace MobiFlight.BrowserMessages
{
    public class Message<T>
    {
        public String key
        {
            get; set;
        }

        public T payload { get; set; }

        public Message()
        {
        }

        public Message(string key, T payload)
        {
            this.key = key;
            this.payload = payload;
        }
        public Message(T payload)
        {
            this.key = payload.GetType().Name;
            this.payload = payload;
        }
        
    }
}
