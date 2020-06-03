using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Purchases")]
    public class Purchase
    {
        [ForeignKey("Users"), Key, Column(Order = 0)]
        public User? User {get; private set;}
        [ForeignKey("Baskets")]
        public Basket Basket { get; private set; }
        [ForeignKey("Addresses")]
        public Address Address { get; }
        [Key, Column(Order = 1)]
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
