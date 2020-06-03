using System;
using System.Collections.Generic;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    [Table("OpenDiscounts")]
    public abstract class OpenDiscount : PrimitiveDiscount
    {
        [ForeignKey("Products")]
        public Product Product { get;}
        protected OpenDiscount(double percentage, DateTime deadline, Product product, Store store) : 
                                                                                    base(percentage, deadline, store)
        {
            Product = product;
        }
    }
}