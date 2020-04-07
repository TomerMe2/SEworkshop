using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Purchase
    {
        public Product Product { get; private set; }
        public User User { get; private set; }

        public Purchase(Product product, User user)
        {
            Product = product;
            User = user;
        }
    }
}
