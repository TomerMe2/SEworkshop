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
        
        public abstract double ApplyDiscount(ICollection<(Product, int)> itemsList);
    }
}
