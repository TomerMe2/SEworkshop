using System;
using System.Collections.Generic;
using SEWorkshop.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    [Table("SpecificProductDiscounts")]
    public class SpecificProducDiscount : OpenDiscount
    {
        public SpecificProducDiscount(double percentage, DateTime deadline,
            Product product, Store store) :
            base(percentage, deadline, product, store)
        {

        }

        public override double ComputeDiscount(ICollection<(Product, int)> itemsList)
        {
            if (DateTime.Now > Deadline)
            {
                return 0;
            }

            double totalDiscount = 0;
            foreach (var (prod, quantity) in itemsList)
            {
                if (Product == prod)
                {
                    totalDiscount += (prod.Price * quantity) * (Percentage / 100);
                }
            }

            return totalDiscount;
        }
    }
}