using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Cart
    {
        public ICollection<Basket> Baskets { get; private set; }
        public User User { get; private set; }

        public Cart(User user)
        {
            User = user;
            Baskets = new List<Basket>();
        }
    }
}
