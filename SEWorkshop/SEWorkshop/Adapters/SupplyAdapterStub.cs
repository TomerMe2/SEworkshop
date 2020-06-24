using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        private static readonly HttpClient client = new HttpClient();
        public SupplyAdapterStub()
        {
            //client.Timeout = TimeSpan.FromSeconds(30);
        }
        public async Task<int> Supply(ICollection<ProductsInBasket> products, Address address, string username)
        {
            var handshakePostContent = new Dictionary<string, string>
            {
                {"action_type", "handshake"}
            };

            var handshakeContent = new FormUrlEncodedContent(handshakePostContent);
            var handshakeResponse = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", handshakeContent);
            var handshakeResponseString = await handshakeResponse.Content.ReadAsStringAsync();

            if(handshakeResponseString.Equals("OK"))
            {
                var supplyPostContent = new Dictionary<string, string>
                {
                    { "action_type", "supply" },
                    { "name", username },
                    { "address", address.Street + " " + address.HouseNumber },
                    { "city", address.City },
                    { "country", address.Country },
                    { "zip", address.Zip }
                };

                var supplyContent = new FormUrlEncodedContent(supplyPostContent);
                var supplyResponse = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", supplyContent);
                var supplyResponseString = await supplyResponse.Content.ReadAsStringAsync();

                return int.Parse(supplyResponseString);
            }

            return -1;
        }

        public bool CanSupply(ICollection<ProductsInBasket> products, Address address)
        {
            return products.Where(prod => prod.Product.Quantity < prod.Quantity).Count() <= 0;
        }

        public async void CancelSupply(int TransactionId)
        {
            var handshakePostContent = new Dictionary<string, string>
            {
                {"action_type", "handshake"}
            };

            var handshakeContent = new FormUrlEncodedContent(handshakePostContent);
            var handshakeResponse = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", handshakeContent);
            var handshakeResponseString = await handshakeResponse.Content.ReadAsStringAsync();

            if (handshakeResponseString.Equals("OK"))
            {
                var cancelSupplyPostContent = new Dictionary<string, string>
                {
                    { "action_type", "cancel_supply" },
                    { "transaction_id", TransactionId.ToString() }
                };

                var cancelContent = new FormUrlEncodedContent(cancelSupplyPostContent);
                var cancelResponse = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", cancelContent);
                var cancelResponseString = await cancelResponse.Content.ReadAsStringAsync();
            }
        }
    }
}
