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
        protected ConditionalDiscount( double percentage, DateTime deadline, Product product, Store store, User user) : base(percentage, deadline, store, user)
        {
            Product = product;
        }

        protected abstract double ApplyDiscount(Basket basket);

       
       
        

    }
}