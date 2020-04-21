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
        public ICollection<Message> Messages { get; private set; }
        public ICollection<Discount> Discounts { get; private set; }
        public string StoreName {get; private set; }
        public Policy Policy { get; private set; }
        public bool IsOpen { get; internal set; }

        public Store(LoggedInUser owner, string storeName)
        {
            Products = new List<Product>();
            Managers = new List<LoggedInUser>();
            Owners = new List<LoggedInUser>() { owner };
            Messages = new List<Message>();
            Discounts = new List<Discount>();
            StoreName = storeName;
            Policy = new Policy();
        }
    }
}
