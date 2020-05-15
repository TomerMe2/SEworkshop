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
        
        public User User { get; }


        public Discount(double percentage, DateTime deadline, Store store, User user)
        {
            if (SetDiscountPercentage(percentage))
            {
                Deadline = deadline;
                Store = store;
                User = user;
            }
            else
            {
                throw new Exception(); //TODO: find \ create appropriate exception
            }
        }

        protected Basket? GetBasket()
        {
            return User.Cart.Baskets.FirstOrDefault(bskt => bskt.Store == Store);
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
        
        public abstract double ApplyDiscount();
    }
}
