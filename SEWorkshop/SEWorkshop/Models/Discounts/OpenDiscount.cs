using System;
using System.Collections.Generic;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public abstract class OpenDiscount : PrimitiveDiscount
    {
        public virtual string ProdName { get; set; }
        public virtual string ProdStoreName { get; set; }
        public virtual Product Product { get; set; }
        protected OpenDiscount(double percentage, DateTime deadline, Product product, Store store) : 
                                                                                    base(percentage, deadline, store)
        {
            Product = product;
            ProdName = product.Name;
            ProdStoreName = product.Store.Name;
        }
    }
}