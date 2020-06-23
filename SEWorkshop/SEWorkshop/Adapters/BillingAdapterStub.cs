using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class BillingAdapterStub : IBillingAdapter
    {
        public bool Bill(ICollection<ProductsInBasket> products, string creditCardNumber, double totalPrice)
        {
            return true;
        }

        public bool CancelBill(int TransactionId)
        {
            return true;
        }
    }
}
