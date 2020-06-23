using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Message
    {
        public virtual int Id { get; set; }
        public virtual string StoreName { get; set; }
        public virtual Store ToStore { get; set; }
        public virtual string Writer { get; set; }
        public virtual LoggedInUser WrittenBy { get; set; }
        public virtual string Description {get; set; }
        //public virtual int? PrevId { get; set; }
        public virtual Message? Prev { get; set; }
        //public virtual int? NextId { get; set; }
        public virtual Message? Next { get; set; }
        public virtual bool StoreSawIt { get; set; }
        public virtual bool ClientSawIt { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Message()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public Message(LoggedInUser writtenBy, Store toStore, string description, bool isClient, Message? prev = null)
        {
            WrittenBy = writtenBy;
            Description = description;
            Prev = prev;
            ToStore = toStore;
            StoreName = toStore.Name;
            Writer = writtenBy.Username;
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
