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

        public Store(RegisteredUser owner)
        {
            Products = new List<Product>();
            Managers = new List<RegisteredUser>();
            Owners = new List<RegisteredUser>() { owner };
            Messages = new List<Message>();
        }
    }
}
