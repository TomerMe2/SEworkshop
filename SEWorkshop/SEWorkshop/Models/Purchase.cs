using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Purchase
    {
        public virtual string? Username { get; set; }
        public virtual LoggedInUser? User { get; private set;}
        public virtual int BasketId { get; set; }
        public virtual Basket Basket { get; private set; }
        public virtual Address Address { get; }
        [Timestamp]
        public virtual DateTime TimeStamp { get; }
        public virtual double MoneyPaid { get; }

        public Purchase()
        {

        }

        public Purchase(LoggedInUser? user, Basket basket, Address adrs)
        {
            User = user;
            Basket = basket;
            TimeStamp = DateTime.Now;
            Address = adrs;
            MoneyPaid = basket.PriceAfterDiscount();
            Basket.Purchase = this;
        }
    }
}
