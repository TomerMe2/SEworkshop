using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Adapters
{
    interface ISupplyAdapter
    {
        //houseNum is string because of house numbers like 18א
        public void Supply(ICollection<ProductsInBasket> products, Address address, string username);

        public bool CanSupply(ICollection<ProductsInBasket> products, Address address);

        public bool CancelSupply(int TransactionId);
    }
}
