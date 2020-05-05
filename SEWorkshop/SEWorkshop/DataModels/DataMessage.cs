using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataMessage : DataModel<Message>
    {
        public DataUser WrittenBy => new DataUser(InnerModel.WrittenBy);
        public string Description => InnerModel.Description;
        public DataMessage? Prev => InnerModel.Prev != null ? new DataMessage(InnerModel.Prev) : null;
        public DataMessage? Next => InnerModel.Next != null ? new DataMessage(InnerModel.Next) : null;

        public DataMessage(Message msg) : base(msg) { }
    }
}
