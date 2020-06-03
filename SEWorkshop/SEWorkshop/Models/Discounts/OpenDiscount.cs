using System;
using System.Collections.Generic;
using SEWorkshop.Enums;

namespace SEWorkshop.Models.Discounts
{
    public abstract class OpenDiscount : PrimitiveDiscount
    {
        public Product Product { get;}
        protected OpenDiscount(double percentage, DateTime deadline, Product product, Store store) : 
                                                                                    base(percentage, deadline, store)
        {
            Product = product;
        }
    }
}