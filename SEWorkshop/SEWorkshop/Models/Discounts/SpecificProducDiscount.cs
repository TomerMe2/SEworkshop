using System;
using System.Collections.Generic;
using SEWorkshop.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public class SpecificProducDiscount : OpenDiscount
    {

        protected SpecificProducDiscount() : base()
        {

        }

        public SpecificProducDiscount(double percentage, DateTime deadline,
            Product product, Store store) :
            base(percentage, deadline, product, store)
        {

        }

        public override double ComputeDiscount(ICollection<ProductsInBasket> itemsList)
        {
            if (DateTime.Now > Deadline)
            {
                return 0;
            }

            double totalDiscount = 0;
            foreach (var prod in itemsList)
            {
                if (prod.Product.Equals(Product))
                {
                    totalDiscount += (prod.Product.Price * prod.Quantity) * (Percentage / 100);
                }
            }

            return totalDiscount;
        }
    }
}