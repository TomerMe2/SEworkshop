using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Messages")]
    public class Message
    {
        private static int counter = 0;
        private static object counterLock = new object();
        public int Id { get; }
        [ForeignKey("Stores")]
        public Store ToStore { get; }
        [ForeignKey("Users")]
        public LoggedInUser WrittenBy { get; private set; }
        public string Description {get; private set;}
        public Message? Prev { get; private set; }
        public Message? Next { get; set; }
        public bool StoreSawIt { get; set; }
        public bool ClientSawIt { get; set; }

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
