using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Discounts
{
    public abstract class BuyOverDiscount : ConditionalDiscount
    {
        public Store Store { get; set; }
        public int MinSum { get; set; }
        public int OffSum { get; set; }

        public BuyOverDiscount(Store store, int minSum, int offSum, double percentage, DateTime deadline, Product product) : base(percentage, deadline, product, store)
        {
            Store = store;
            MinSum = minSum;
            OffSum = offSum;
        }
        protected override double ApplyDiscount(Basket basket)
        {
            double sumBasket = 0;
            foreach (var product in basket.Products)
            {
                sumBasket = sumBasket + (product.Item1.Price * product.Item2);
            }
            if (sumBasket >= MinSum)
            {
                return sumBasket * (1 - OffSum / 100);

            }
            return 0;
        }
    }
}