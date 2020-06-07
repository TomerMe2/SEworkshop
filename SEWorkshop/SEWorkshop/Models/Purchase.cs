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
        /*
        public virtual string City { get; set; }
        public virtual string Street { get; set; }
        public virtual string HouseNumber { get; set; }
        public virtual string Country { get; set; }
        */
        public virtual Address Address { get; set;  }
        public virtual DateTime TimeStamp { get; set;  }
        public virtual double MoneyPaid { get; set; }

        public virtual string Country { get; set; }
        public virtual string City { get; set; }
        public virtual string Street { get; set; }
        public virtual string HouseNumber { get; set; }

        private Purchase()
        {
            Basket = null!;
            Address = null!;
            TimeStamp = default;
            Country = "";
            City = "";
            Street = "";
            HouseNumber = "";
        }


        public Purchase(LoggedInUser? user, Basket basket, Address adrs)
        {
            User = user;
            Basket = basket;
            TimeStamp = DateTime.Now;
            Address = adrs;
            MoneyPaid = basket.PriceAfterDiscount();
            Basket.Purchase = this;

            Country = adrs.Country;
            City = adrs.City;
            Street = adrs.Street;
            HouseNumber = adrs.HouseNumber;
        }
    }
}
