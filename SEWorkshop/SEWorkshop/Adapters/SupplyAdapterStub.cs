using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        private static readonly HttpClient client = new HttpClient();
        public async void Supply(ICollection<ProductsInBasket> products, Address address, string username)
        {
            var handshakePostContent = new Dictionary<string, string>
            {
                {"action_type", "handshake"}
            };

            var supplyPostContent = new Dictionary<string, string>
            {
                { "action_type", "supply" },
                { "name", username },
                { "address", address.Street + " " + address.HouseNumber },
                { "city", address.City },
                { "country", address.Country },
                { "zip", address.Zip }
            };

            var content = new FormUrlEncodedContent(supplyPostContent);

            var response = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);

            var responseString = await response.Content.ReadAsStringAsync();
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
