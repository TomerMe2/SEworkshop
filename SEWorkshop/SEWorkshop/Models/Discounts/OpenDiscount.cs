using System;

namespace SEWorkshop.Models.Discounts
{
    public abstract class OpenDiscount : Discount
    {
        public Product Product { get; set; }
        protected OpenDiscount(double percentage, DateTime deadline, Product product, Store store) : 
                                                                                    base(percentage, deadline, store)
        {
            Product = product;
        }
    }
}