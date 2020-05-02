using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        public void Supply(ICollection<(Product, int)> products, Address address)
        {
            
        }

        public bool CanSupply(ICollection<(Product, int)> products, Address address)
        {
            return true;
        }
    }
}
