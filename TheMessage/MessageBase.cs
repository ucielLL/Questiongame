using System;

namespace TheMessage
{
    public class MessageBase
    {
        public MessageBase()
        {
            Typeobject = GetType().Name;
        }
        public string Sender { get; set; }
        public string avatar { get; set; }
        public string Receiver { get; set; }

        public string Typeobject { get; set; }
    }
}
