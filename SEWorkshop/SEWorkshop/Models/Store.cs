using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Store
    {
        public ICollection<Product> Products { get; private set; }
        public IDictionary<LoggedInUser, LoggedInUser> Managers { get; private set; }
        public IDictionary<LoggedInUser, LoggedInUser> Owners { get; private set; }
        public IList<Message> Messages { get; private set; }
        public ICollection<Discount> Discounts { get; private set; }
        public bool IsOpen { get; private set; }
        public string Name { get; private set; }
        public Policy Policy { get; private set; }
        public ICollection<Purchase> Purchases {get; private set; }

        public Store(LoggedInUser owner, string name)
        {
            Products = new List<Product>();
            Managers = new Dictionary<LoggedInUser, LoggedInUser>();
            Owners = new Dictionary<LoggedInUser, LoggedInUser>();
            Messages = new List<Message>();
            IsOpen = true;
            Discounts = new List<Discount>();
            Name = name;
            Policy = new Policy();
            Purchases = new List<Purchase>();

            Owners.Add(owner, owner);
        }

        public void CloseStore()
        {
            IsOpen = false;
        }
    }
}
