using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Message
    {
        private static int counter = 0;
        private static object counterLock = new object();
        public virtual int Id { get; set; }
        public virtual string StoreName { get; set; }
        public virtual Store ToStore { get; set; }
        public virtual string Writer { get; set; }
        public virtual LoggedInUser WrittenBy { get; set; }
        public virtual string Description {get; set;}
        public virtual Message? Prev { get; set; }
        public virtual Message? Next { get; set; }
        public virtual bool StoreSawIt { get; set; }
        public virtual bool ClientSawIt { get; set; }

        public Message()
        {

        }

        public Message(LoggedInUser writtenBy, Store toStore, string description, bool isClient, Message? prev = null)
        {
            lock(counterLock)
            {
                Id = counter;
                counter++;
            }
            WrittenBy = writtenBy;
            Description = description;
            Prev = prev;
            ToStore = toStore;
            StoreSawIt = false;
            ClientSawIt = false;
            if (isClient)
            {
                ClientSawIt = true;
            }
            else
            {
                StoreSawIt = true;
            }
        }
    }
}
