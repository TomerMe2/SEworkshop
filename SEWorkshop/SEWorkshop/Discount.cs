using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Discount
    {
        public ICollection<Product> Products { get; private set; }

        public Discount()
        {
            Products = new HashSet<Product>();
        }
    }
}
