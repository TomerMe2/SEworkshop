using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Facades;

namespace SEWorkshop.Models.Discounts
{
    public enum Operator
    {
        And,
        Xor,
        Implies
    }
    public abstract class Discount
    {
        public double Percentage { get; private set; }
        
        public DateTime Deadline { get; set; }
        
        public Store Store { get; }

        public (Discount, Operator)? InnerDiscount { get; set; }

        public Discount(double percentage, DateTime deadline, Store store)
        {
            if (SetDiscountPercentage(percentage))
            {
                Deadline = deadline;
                Store = store;
            }
            else
            {
                throw new Exception(); //TODO: find \ create appropriate exception
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
            return Math.Min(ApplyDiscount(itemsList), InnerDiscount.Value.Item1.ComposeDiscounts(itemsList));
        }

        public double ApplyImplies(ICollection<(Product, int)> itemsList)
        {
            double firstDiscount = ApplyDiscount(itemsList);
            if (firstDiscount > 0)
            {
                return firstDiscount + InnerDiscount.Value.Item1.ComposeDiscounts(itemsList);
            }

            return 0;
        }

        public abstract double ApplyDiscount(ICollection<(Product, int)> itemsList);
    }
}
