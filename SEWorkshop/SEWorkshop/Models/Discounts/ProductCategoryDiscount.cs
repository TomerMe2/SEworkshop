using System;
using System.Collections.Generic;
using SEWorkshop.Exceptions;

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

        public override double ComputeDiscount(ICollection<(Product, int)> itemsList)
        {
            double totalDiscount = 0;

            foreach (var (prod, quantity) in itemsList)
            {
                if (DateTime.Now > Deadline)
                {
                    return 0;
                }
                if (prod.Category.Equals(CatUnderDiscount))
                {
                    totalDiscount += (prod.Price * quantity) * (Percentage / 100);
                }
            }

            return totalDiscount;
        }
    }
}