using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    [Table("BODiscount")]
    public class BuyOverDiscount : ConditionalDiscount
    {
       
        public double MinSum { get; set; }

        public BuyOverDiscount(Store store, double minSum, double percentage, DateTime deadline, Product product) : base(percentage, deadline, product, store)
        {
             MinSum = minSum;
        }
       
        public override double ComputeDiscount(ICollection<(Product, int)> itemsList)
        {
            double sumBasket = 0;
            foreach (var product in itemsList)
            {
                sumBasket = sumBasket + (product.Item1.Price * product.Item2);
            }
            if (sumBasket >= MinSum)
            {
                return sumBasket * (Percentage / 100);
            }
            return 0;
        }
    }
}