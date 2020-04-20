using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class Basket
    {
        public Store Store { get; private set; }
        public Cart Cart { get; private set; }
        public ICollection<Product> Products { get; private set; }

        public Basket(Store store, Cart cart)
        {
            Store = store;
            Cart = cart;
            Products = new List<Product>();
        }
    }
}
