using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class Store
    {
        public ICollection<Product> Products { get; private set; }
        public ICollection<LoggedInUser> Managers { get; private set; }
        public ICollection<LoggedInUser> Owners { get; private set; }
        public IList<Message> Messages { get; private set; }
        public string Store_name {get; private set; }

        public Store(LoggedInUser owner, string store_name)
        {
            Products = new List<Product>();
            Managers = new List<LoggedInUser>();
            Owners = new List<LoggedInUser>() { owner };
            Messages = new List<Message>();
            Store_name = store_name;
        }
    }
}
