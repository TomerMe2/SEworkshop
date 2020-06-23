using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.Models;

namespace SEWorkshop
{
    public class Address
    {
        public virtual int Id { get; set; }
        public virtual string City { get; set; }
        public virtual string Street { get; set; }
        public virtual string HouseNumber { get; set; }
        public virtual string Country { get; set; }
        public virtual string zip { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set;}

        public Address()
        {
            /*City = "";
            Street = "";
            HouseNumber = "";
            Country = "";
            Purchases = new List<Purchase>();*/
        }

        public Address(string country, string city, string street, string houseNumber, string zip)
        {
            this.City = city;
            this.Street = street;
            this.HouseNumber = houseNumber;
            this.Country = country;
            this.zip = zip;
            Purchases = new List<Purchase>();
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Address))
            {
                return false;
            }
            var other = (Address)obj;
            return Country.Equals(other.Country) && City.Equals(other.City) && Street.Equals(other.Street)
                && HouseNumber.Equals(other.HouseNumber);
        }

        public override int GetHashCode()
        {
            return Country.GetHashCode() ^ City.GetHashCode() ^ Street.GetHashCode() ^ HouseNumber.GetHashCode();
        }
    }
}
