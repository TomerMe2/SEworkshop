using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Product
    {
        public Policy Policy { get; private set; }
        public ICollection<Discount> Discounts { get; private set; }


        public Product(Policy policy)
        {
            Policy = policy;
            Discounts = new List<Discount>();
        }
    }
}
