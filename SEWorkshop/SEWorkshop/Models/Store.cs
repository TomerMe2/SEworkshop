using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Store
    {
        public ICollection<Product> Products { get; private set; }
        public ICollection<RegisteredUser> Managers { get; private set; }
        public ICollection<RegisteredUser> Owners { get; private set; }
        public IList<Message> Messages { get; private set; }
        public string Store_name {get; private set; }

        public Store(RegisteredUser owner, string store_name)
        {
            Products = new List<Product>();
            Managers = new List<RegisteredUser>();
            Owners = new List<RegisteredUser>() { owner };
            Messages = new List<Message>();
            Store_name = store_name;
        }
    }
}
