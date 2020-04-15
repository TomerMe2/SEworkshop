using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Product
    {
        public Policy Policy { get; private set; }
        public ICollection<Discount> Discounts { get; private set; }
        public Store Store { get; private set; }

        public Product(Policy policy, Store store)
        {
            Policy = policy;
            Discounts = new List<Discount>();
            Store = store;
        }
    }
}
