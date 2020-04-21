using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class Product
    {
        public string Name {get; private set; } 
        public string Category {get; private set; }
        public float Price { get; private set; }
        public Store Store { get; private set; }

        public Product(string name, string category, float price, Store store)
        {
            Name = name;
            Category = category;
            Price = price;
            Store = store;
        }

        public override string ToString()
        {
            string output = "Name: " + Name +
            "\nStore: " + Store.Store_name +
            "\nCategory: " + Category +
            "\nPrice: " + Price;
            return output;
        }
    }
}
