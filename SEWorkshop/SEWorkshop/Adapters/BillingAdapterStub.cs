using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class BillingAdapterStub : IBillingAdapter
    {
        private static readonly HttpClient client = new HttpClient();
        public BillingAdapterStub()
        {
            //client.Timeout = TimeSpan.FromSeconds(30);
        }
        public async Task<int> Bill(ICollection<ProductsInBasket> products, string creditCardNumber, DateTime expirationDate, string cvv, double totalPrice, string username,string id)
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
                var payPostContent = new Dictionary<string, string>
                {
                    {"action_type", "pay"},
                    {"card_number", creditCardNumber},
                    {"month", expirationDate.Month.ToString()},
                    {"year", expirationDate.Year.ToString()},
                    {"holder", username},
                    {"ccv", cvv},
                    {"id", id}
                };

                var payContent = new FormUrlEncodedContent(payPostContent);
                var payResponse = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", payContent);
                var payResponseString = await payResponse.Content.ReadAsStringAsync();

                return int.Parse(payResponseString);
            }
            return -1;
        }

        public async void CancelBill(int TransactionId)
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
                var cancelBillingPostContent = new Dictionary<string, string>
                {
                    { "action_type", "cancel_pay" },
                    { "transaction_id", TransactionId.ToString() }
                };

                var cancelContent = new FormUrlEncodedContent(cancelBillingPostContent);
                var cancelResponse = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", cancelContent);
                var cancelResponseString = await cancelResponse.Content.ReadAsStringAsync();
            }
        }
    }
}
