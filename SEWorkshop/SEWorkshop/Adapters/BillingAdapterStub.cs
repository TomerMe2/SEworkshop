using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class BillingAdapterStub : IBillingAdapter
    {
        public bool Bill(ICollection<(Product, int)> products, string creditCardNumber)
        {
            return true;
        }
    }
}
