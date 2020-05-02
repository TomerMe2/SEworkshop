using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Adapters
{
    interface ISupplyAdapter
    {
        //houseNum is string because of house numbers like 18א
        public void Supply(ICollection<(Product, int)> products, Address address);

        public bool CanSupply(ICollection<(Product, int)> products, Address address);
    }
}
