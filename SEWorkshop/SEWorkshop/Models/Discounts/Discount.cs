using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;

namespace SEWorkshop.Models.Discounts
{
    public abstract class Discount
    {
        public (Discount, Operator)? InnerDiscount { get; set; }
        public double Percentage { get; private set; }

        public DateTime Deadline { get; set; }

        public Store Store { get; }

        public Discount(double percentage, DateTime deadline, Store store)
        {
            if (SetDiscountPercentage(percentage))
            {
                Deadline = deadline;
                Store = store;
            }
            else
            {
                throw new IllegalDiscountPercentageException();
            }
        }

        public bool SetDiscountPercentage(double percentage)
        {
            if (percentage >= 0 && percentage <= 100)
            {
                Percentage = percentage;
                return true;
            }
            return false;
        }

        // returns amount of money to subtract from the original price
        public abstract double ApplyDiscount(ICollection<(Product, int)> itemsList);

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
                Operator.Implies => ApplyImplies(itemsList),
                _ => throw new Exception("Should not get here"),
            };
        }

        public double ChooseCheaper(ICollection<(Product, int)> itemsList)
        {
            if (InnerDiscount != null)
            {
                return Math.Min(ApplyDiscount(itemsList), InnerDiscount.Value.Item1.ComposeDiscounts(itemsList));
            }
            return ApplyDiscount(itemsList);
        }

        public double ApplyImplies(ICollection<(Product, int)> itemsList)
        {
            double firstDiscount = ApplyDiscount(itemsList);
            if (firstDiscount > 0)
            {
                if (InnerDiscount != null)
                {
                    return firstDiscount + InnerDiscount.Value.Item1.ComposeDiscounts(itemsList);
                }
                return firstDiscount;
            }
            return 0;
        }
    }
}
