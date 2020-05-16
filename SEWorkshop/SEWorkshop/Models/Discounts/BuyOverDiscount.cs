using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Discounts
{
    public class BuyOverDiscount : ConditionalDiscount
    {
        public Store Store { get; set; }
        public double MinSum { get; set; }
        public int OffSum { get; set; }

        public BuyOverDiscount(Store store, double minSum, double percentage, DateTime deadline, Product product) : base(percentage, deadline, product, store)
        {
            Store = store;
            MinSum = minSum;
        }
       
        public override double ApplyDiscount(ICollection<(Product, int)> itemsList)
        {
            double sumBasket = 0;
            foreach (var product in itemsList)
            {
                sumBasket = sumBasket + (product.Item1.Price * product.Item2);
            }
            if (sumBasket >= MinSum)
            {
                return sumBasket * (1 - Percentage / 100);

            }
            return 0;
        }
    }
}