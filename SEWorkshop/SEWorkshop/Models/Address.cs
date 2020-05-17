using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
