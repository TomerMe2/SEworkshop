using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Message
    {
        private static int counter = 0;
        private static object counterLock = new object();
        public int Id { get; }
        public LoggedInUser WrittenBy { get; private set; }
        public string Description {get; private set;}
        public Message? Prev { get; private set; }
        public Message? Next { get; set; }

        public Message(LoggedInUser writtenBy, string description, Message? prev = null)
        {
            lock(counterLock)
            {
                Id = counter;
                counter++;
            }
            WrittenBy = writtenBy;
            Description = description;
            Prev = prev;
        }
    }
}
