using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Facades.Adapters
{
    class BillingAdapterStub : IBillingAdapter
    {
        public bool Bill(ICollection<Product> products, string creditCardNumber)
        {
            return true;
        }
    }
}
