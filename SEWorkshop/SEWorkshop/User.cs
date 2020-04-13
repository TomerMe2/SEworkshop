using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class User
    {
        public ICollection<Cart> Carts { get; private set; }

        public ICollection<Purchase> Purchases { get; private set; }

        public User()
        {
            Carts = new List<Cart>();
            Purchases = new List<Purchase>();
        }
    }
}
