using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    class DataMessage
    {
        public DataUser WrittenBy => new DataUser(InnerMessage.WrittenBy);
        public string Description => InnerMessage.Description;
        public DataMessage? Prev => InnerMessage.Prev != null ? new DataMessage(InnerMessage.Prev) : null;
        public DataMessage? Next => InnerMessage.Next != null ? new DataMessage(InnerMessage.Next) : null;
        private Message InnerMessage { get; }

        public DataMessage(Message msg)
        {
            InnerMessage = msg;
        }
    }
}
