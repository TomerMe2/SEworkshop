using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class Store
    {
        public IEnumerable<Product> Products { get; private set; }
        public IEnumerable<LoggedInUser> Managers { get; private set; }
        public IEnumerable<LoggedInUser> Owners { get; private set; }
        public IEnumerable<Message> Messages { get; private set; }
        public IEnumerable<Discount> Discounts { get; private set; }
        public string Store_name {get; private set; }
        public Policy Policy { get; private set; }

        public Store(LoggedInUser owner, string store_name, Policy policy)
        {
            Products = new List<Product>();
            Managers = new List<LoggedInUser>();
            Owners = new List<LoggedInUser>() { owner };
            Messages = new List<Message>();
            Discounts = new List<Discount>();
            Store_name = store_name;
            Policy = policy;
        }
    }
}
