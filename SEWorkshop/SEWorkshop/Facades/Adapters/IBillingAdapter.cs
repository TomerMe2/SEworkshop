using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades.Adapters
{
    interface IBillingAdapter
    {
        public bool Bill(ICollection<Product> products, string creditCardNumber);
    }
}
