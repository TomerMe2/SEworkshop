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
        public virtual ICollection<Purchase> Purchases { get; set;}

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Address()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        public Address(string country, string city, string street, string houseNumber)
        {
            this.City = city;
            this.Street = street;
            this.HouseNumber = houseNumber;
            this.Country = country;
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
