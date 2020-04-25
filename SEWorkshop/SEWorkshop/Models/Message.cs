﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Message
    {
        public User WrittenBy { get; private set; }
        public string Description {get; private set;}
        public Message? Prev { get; private set; }
        public Message? Next { get; set; }

        public Message(User writtenBy, string description, Message? prev = null)
        {
            WrittenBy = writtenBy;
            Description = description;
            Prev = prev;
        }
    }
}