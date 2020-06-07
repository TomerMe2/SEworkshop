using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public class BuyOverDiscount : ConditionalDiscount
    {
       
        public virtual double MinSum { get; set; }

        protected BuyOverDiscount() : base()
        {

        }

        public BuyOverDiscount(Store store, double minSum, double percentage, DateTime deadline, Product product) : base(percentage, deadline, product, store)
        {
             MinSum = minSum;
        }
       
        public override double ComputeDiscount(ICollection<ProductsInBasket> itemsList)
        {
            double sumBasket = 0;
            foreach (var product in itemsList)
            {
                sumBasket = sumBasket + (product.Product.Price * product.Quantity);
            }
            if (sumBasket >= MinSum)
            {
                return sumBasket * (Percentage / 100);
            }
            return 0;
        }
    }
}