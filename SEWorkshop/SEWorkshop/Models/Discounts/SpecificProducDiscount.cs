using System;

namespace SEWorkshop.Models.Discounts
{
    public class SpecificProducDiscount : OpenDiscount
    {
        public SpecificProducDiscount(double percentage, DateTime deadline, 
            Product product, Store store, string category, User user) : 
            base(percentage, deadline, product, store, user)
        {
            
        }

        public override double ApplyDiscount()
        {
            double totalDiscount = 0;
            foreach (var (prod, quantity) in GetBasket().Products)
            {
                if (Product == prod)
                {
                    totalDiscount += prod.Price * (1 - Percentage / 100);
                }
            }

            return totalDiscount;
        }
    }
}