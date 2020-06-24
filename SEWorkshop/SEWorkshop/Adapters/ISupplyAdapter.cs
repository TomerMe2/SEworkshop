using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SEWorkshop.Adapters
{
    interface ISupplyAdapter
    {
        //houseNum is string because of house numbers like 18א
        public Task<int> Supply(ICollection<ProductsInBasket> products, Address address, string username);

        public bool CanSupply(ICollection<ProductsInBasket> products, Address address);

        public void CancelSupply(int TransactionId);
    }
}
