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

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public OpenDiscount() : base()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        protected OpenDiscount(double percentage, DateTime deadline, Product product, Store store) : 
                                                                                    base(percentage, deadline, store)
        {
            Product = product;
            ProductId = product.Id;
        }
    }
}