using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Adapters
{
    interface IBillingAdapter
    {
        public bool Bill(ICollection<ProductsInBasket> products, string creditCardNumber, DateTime expirationDate, string cvv, double totalProce, string username, string id);

        public bool CancelBill(int TransactionId);

    }
}
