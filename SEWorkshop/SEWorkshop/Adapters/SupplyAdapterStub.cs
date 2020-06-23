using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        public void Supply(ICollection<ProductsInBasket> products, Address address)
        {
            
        }

        public bool CanSupply(ICollection<ProductsInBasket> products, Address address)
        {
            return true;
        }
    }
}
