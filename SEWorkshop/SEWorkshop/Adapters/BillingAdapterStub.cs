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
        public async Task<bool> Bill(ICollection<ProductsInBasket> products, string creditCardNumber, DateTime expirationDate, string cvv, double totalPrice, string username,string id)
        {
            var handshakePostContent = new Dictionary<string, string>
            {
                {"action_type", "handshake"}
            };

            var payPostContent = new Dictionary<string, string>
            {
                {"action_type", "pay"},
                {"card_number", creditCardNumber},
                {"month", expirationDate.Month.ToString()},
                {"year", expirationDate.Year.ToString()},
                {"holder", username},
                {"id", id}
            };

            var content = new FormUrlEncodedContent(payPostContent);

            var response = await client.PostAsync("https://cs-bgu-wsep.herokuapp.com/", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return true;
        }

        public bool CancelBill(int TransactionId)
        {
            return true;
        }
    }
}
