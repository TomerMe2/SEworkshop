using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        public void Supply(ICollection<ProductsInBasket> products, Address address, string name)
        {
            var postContent = new Dictionary<string, string>
            {
                { "action_type", "supply" },
                { "name", "Israel Israelovice" },
                { "address", address.Street + " " + address.HouseNumber },
                { "city", address.City },
                { "country", address.Country },
                { "zip", "8458527" }
            };
        }

        public bool CanSupply(ICollection<ProductsInBasket> products, Address address)
        {
            return true;
        }

        public bool CancelSupply(int TransactionId)
        {
            return true;
        }
    }
}
