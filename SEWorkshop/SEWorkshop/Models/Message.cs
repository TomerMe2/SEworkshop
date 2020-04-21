using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Message
    {
        public Store Store { get; private set; }
        public Message? Prev { get; private set; }
        public Message? Next { get; private set; }
        public LoggedInUser WrittenBy { get; private set; }

        public Message(Store store, LoggedInUser writtenBy, Message? prev = null, Message? next = null)
        {
            Store = store;
            WrittenBy = writtenBy;
            Prev = prev;
            Next = next;
        }
    }
}
