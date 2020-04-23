using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Facades.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        public void Supply(ICollection<Product> products, string city, string street, string houseNum)
        {
            
        }
    }
}
