using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Purchase
    {
        public Cart Cart { get; private set; }
        public User User { get; private set; }

        public Purchase(Cart cart, User user)
        {
            Cart = cart;
            User = user;
        }
    }
}
