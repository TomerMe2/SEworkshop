using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        public void Supply(ICollection<ProductsInBasket> products, Address address, string firstname, string lastname)
        {
            var postContent = new Dictionary<string, string>
            {
                { "action_type", "supply" },
                { "name", firstname + " " + lastname },
                { "address", address.Street + " " + address.HouseNumber },
                { "city", address.City },
                { "country", address.Country },
                { "zip", address.Zip }
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
