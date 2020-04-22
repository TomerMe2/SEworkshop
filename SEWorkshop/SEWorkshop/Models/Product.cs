using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Product
    {
        public Store Store { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price { get   ; set; }

        public Product(Store store, string name, string description, string category, double price)
        {
            Store = store;
            Name = name;
            Description = description;
            Category = category;
            Price = price;
        }

        public override string ToString()
        {
            string output = "Name: " + Name +
            "\nDescription: " + Description + 
            "\nStore: " + Store.Name +
            "\nCategory: " + Category +
            "\nPrice: " + Price;
            return output;
        }
    }
}
