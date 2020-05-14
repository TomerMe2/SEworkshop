using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataMessage : DataModel<Message>
    {
        public int Id => InnerModel.Id;
        public DataStore ToStore => new DataStore(InnerModel.ToStore);
        public DataLoggedInUser WrittenBy => new DataLoggedInUser(InnerModel.WrittenBy);
        public string Description => InnerModel.Description;
        public DataMessage? Prev => InnerModel.Prev != null ? new DataMessage(InnerModel.Prev) : null;
        public DataMessage? Next => InnerModel.Next != null ? new DataMessage(InnerModel.Next) : null;

        public DataMessage(Message msg) : base(msg) { }
    }
}
