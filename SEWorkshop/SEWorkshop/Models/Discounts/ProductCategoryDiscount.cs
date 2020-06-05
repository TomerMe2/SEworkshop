using System;
using System.Collections.Generic;
using SEWorkshop.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public class ProductCategoryDiscount : PrimitiveDiscount
    {
        public string CatUnderDiscount;
        
        public ProductCategoryDiscount(double percentage, DateTime deadline, 
                                         Store store, string category) : 
                                                                    base(percentage, deadline, store)
        {
            CatUnderDiscount = category;
        }

        public override double ComputeDiscount(ICollection<ProductsInBasket> itemsList)
        {
            double totalDiscount = 0;

            foreach (var prod in itemsList)
            {
                if (DateTime.Now > Deadline)
                {
                    return 0;
                }
                if (prod.Product.Category.Equals(CatUnderDiscount))
                {
                    totalDiscount += (prod.Product.Price * prod.Quantity) * (Percentage / 100);
                }
            }

            return totalDiscount;
        }
    }
}