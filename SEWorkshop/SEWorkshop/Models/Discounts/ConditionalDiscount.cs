using NUnit.Framework.Interfaces;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SEWorkshop.Models.Discounts
{
    public abstract class ConditionalDiscount : Discount
    {
        public Product Product { get; set; }
        protected ConditionalDiscount( double percentage, DateTime deadline, Product product) : base(percentage, deadline)
        {
            Product = product;
        }

        protected abstract double ApplyDiscount(Basket basket);

       
        public override double ApplyImplies(Discount other)
        {
            if (IsApplied)
            {
                return ApplyDiscount() + other.ApplyDiscount();
            }

            return Product.Price + other.GetOriginalPrice();
        }

        public override double ApplyDiscountOperator()
        {
            if (InnerDiscount is null)
            {
                return ApplyDiscount();
            }

            Discount other = InnerDiscount.Value.Item1;
            return InnerDiscount.Value.Item2 switch
            {
                Operator.And => ApplyDiscount() + other.ApplyDiscount(),
                Operator.Implies => ApplyImplies(other),
                Operator.Xor => ChooseCheaper(other),
                _ => -1,
            };
        }

        public override double GetOriginalPrice()
        {
            return Product.Price;
        }
    }
}