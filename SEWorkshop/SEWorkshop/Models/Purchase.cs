using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Purchase
    {
        public User User {get; private set;}
        public Basket Basket { get; private set; }
        public DateTime TimeStamp { get; }
        public Purchase(User user, Basket basket)
        {
            User = user;
            Basket = basket;
            TimeStamp = DateTime.Now;
        }
    }
}
