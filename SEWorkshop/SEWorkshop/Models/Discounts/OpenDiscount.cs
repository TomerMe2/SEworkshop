using System;
using System.Collections.Generic;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public abstract class OpenDiscount : PrimitiveDiscount
    {
        public virtual int ProductId { get; set; }
        public virtual Product Product { get; set; }

        protected OpenDiscount() : base()
        {
            Product = null!;
        }

        protected OpenDiscount(double percentage, DateTime deadline, Product product, Store store) : 
                                                                                    base(percentage, deadline, store)
        {
            Product = product;
            ProductId = product.Id;
        }
    }
}