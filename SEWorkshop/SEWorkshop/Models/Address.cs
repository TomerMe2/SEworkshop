using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop
{
    [Table("Addresses")]
    public class Address
    {
        [Key, Column(Order = 0)]
        public string City { get; set; }
        [Key, Column(Order = 1)]
        public string Street { get; set; }
        [Key, Column(Order = 2)]
        public string HouseNumber { get; set; }
        [Key, Column(Order = 3)]
        public string Country { get; set; }

        public Address(string country, string city, string street, string houseNumber)
        {
            this.City = city;
            this.Street = street;
            this.HouseNumber = houseNumber;
            this.Country = country;
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
