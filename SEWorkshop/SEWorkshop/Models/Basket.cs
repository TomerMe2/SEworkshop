using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class Basket
    {
        public Store Store { get; private set; }
        public ICollection<Product> Products { get; private set; }

        public Basket(Store store)
        {
            Store = store;
            Products = new List<Product>();
        }
    }
}
