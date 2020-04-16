using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Product
    {
        public string Name {get; private set; } 
        public string Category {get; private set; }
        public Policy Policy { get; private set; }
        public ICollection<Discount> Discounts { get; private set; }
        public Store Store { get; private set; }

        public Product(string name, string category, Policy policy, Store store)
        {
            Name = name;
            Category = category;
            Policy = policy;
            Discounts = new List<Discount>();
            Store = store;
        }

        public override string ToString()
        {
            string discountDescription = "";
            foreach (var Discount in Discounts)
            {
                discountDescription += Discount.Code + "\n";
            }
            string output = "Name: " + Name +
            "\nStore: " + Store.Store_name +
            "\nCategory: " + Category +
            "\nPolicy:" + Policy.ToString() +
            "\nDiscounts:\n" + discountDescription;
            return output;
        }
    }
}
