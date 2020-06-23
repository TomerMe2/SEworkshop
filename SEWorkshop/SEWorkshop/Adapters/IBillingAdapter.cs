using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SEWorkshop.Adapters
{
    interface IBillingAdapter
    {
        public Task<int> Bill(ICollection<ProductsInBasket> products, string creditCardNumber, DateTime expirationDate, string cvv, double totalProce, string username, string id);

        public void CancelBill(int TransactionId);

    }
}
