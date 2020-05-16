using System;
using System.Collections.Generic;
using SEWorkshop.Enums;

namespace SEWorkshop.Models.Discounts
{
    public abstract class OpenDiscount : Discount
    {
        public (OpenDiscount, Operator)? InnerDiscount { get; set; }
        public Product Product { get;}
        protected OpenDiscount(double percentage, DateTime deadline, Product product, Store store) : 
                                                                                    base(percentage, deadline, store)
        {
            Product = product;
        }

        public double ComposeDiscounts(ICollection<(Product, int)> itemsList)
        {
            if (InnerDiscount is null)
            {
                return ApplyDiscount(itemsList);
            }

            return InnerDiscount.Value.Item2 switch
            {
                Operator.And => ApplyDiscount(itemsList) + InnerDiscount.Value.Item1.ComposeDiscounts(itemsList),
                Operator.Xor => ChooseCheaper(itemsList),
                _ => throw new Exception("Should not get here"),
            };
        }

        protected double ChooseCheaper(ICollection<(Product, int)> itemsList)
        {
            return Math.Min(ApplyDiscount(itemsList), InnerDiscount.Value.Item1.ComposeDiscounts(itemsList));
        }
    }
}