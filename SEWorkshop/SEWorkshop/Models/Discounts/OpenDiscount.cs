using System;

namespace SEWorkshop.Models.Discounts
{
    public abstract class OpenDiscount : Discount
    {
        public Product Product { get; set; }
        protected OpenDiscount(double percentage, DateTime deadline, Product product) : base(percentage, deadline)
        {
            Product = product;
        }

        public override double ApplyDiscount()
        {
            return Product.Price * (1 - Percentage / 100);
        }

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