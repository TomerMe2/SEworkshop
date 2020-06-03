using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Basket
    {
        public Store Store { get; private set; }

        // Every element in this collection is a 2-tuple: (product, amountToBuy)
        public ICollection<(Product, int)> Products { get; private set; }

        public Basket(Store store)
        {
            Store = store;
            Products = new List<(Product, int)>();
        }

        public double PriceWithoutDiscount()
        {
            double totalPrice = 0;
            foreach (var (product, quantity) in Products)
            {
                totalPrice += product.Price * quantity;
            }

            return totalPrice;
        }

        public double PriceAfterDiscount()
        {
            double price = PriceWithoutDiscount();
            foreach (var dis in Store.Discounts)
            {
                price -= dis.ComputeDiscount(Products);
            }

            return price;
        }
    }
}
