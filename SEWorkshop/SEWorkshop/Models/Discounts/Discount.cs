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
        public (Discount, Operator)? InnerDiscount { get; set; }
        public double Percentage { get; private set; }
        
        public DateTime Deadline { get; set; }

        public bool IsApplied { get; set; }

        public Discount(double percentage, DateTime deadline)
        {
            if (SetDiscountPercentage(percentage))
            {
                Deadline = deadline;
            }
            else
            {
                throw new Exception(); //TODO: find \ create appropriate exception
            }

            IsApplied = true;
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

        public abstract double ApplyDiscountOperator();

        public abstract double ApplyImplies(Discount other);

        public double ChooseCheaper(Discount other)
        {
            if (ApplyDiscount() > other.ApplyDiscount())
            {
                return other.ApplyDiscount();
            }

            return ApplyDiscount();
        }
        public abstract double ApplyDiscount();

        public abstract double GetOriginalPrice();
    }
}
