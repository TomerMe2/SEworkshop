using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Policy
    {
        public ICollection<Product> Products { get; private set; }
        
        public Policy()
        {
            Products = new List<Product>();
        }
    }
}
