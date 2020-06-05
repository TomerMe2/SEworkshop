﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop
{
    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
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
