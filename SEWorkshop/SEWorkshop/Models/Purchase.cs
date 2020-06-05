using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Purchase
    {
        public User? User {get; private set;}
        public Basket Basket { get; private set; }
        public Address Address { get; }
        public DateTime TimeStamp { get; }
        public double MoneyPaid { get; }

        public Purchase(User user, Basket basket, Address adrs)
        {
            User = user;
            Basket = basket;
            TimeStamp = DateTime.Now;
            Address = adrs;
            MoneyPaid = basket.PriceAfterDiscount();
        }
    }
}
