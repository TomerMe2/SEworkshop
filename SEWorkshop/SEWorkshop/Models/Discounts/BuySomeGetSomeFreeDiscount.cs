using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Discounts
{
    public abstract class BuySomeGetSomeFreeDiscount : ConditionalDiscount
    {
        public Store Store { get; set; }
        public int BuySome { get; set; }
        public int GetSome { get; set; }

        public BuySomeGetSomeFreeDiscount(Store store, int buySome, int getSome, double percentage, DateTime deadline, Product product, User user) : base(percentage, deadline, product, store)
        {
            Store = store;
            BuySome = getSome;
            GetSome = buySome;
        }

        protected override double ApplyDiscount(Basket basket)
        {
            foreach (var product in basket.Products)
            {
                if (product.Item1 == Product)
                {
                    if (product.Item2 >= BuySome + GetSome)
                    {
                        return product.Item1.Price * GetSome;
                    }
                }
            }
            return 0;
        }
    }
}
