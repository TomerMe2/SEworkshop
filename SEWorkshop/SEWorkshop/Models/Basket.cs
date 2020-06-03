using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Baskets")]
    public class Basket
    {
        public static int Counter_ID = 0;
        public int Id {get; private set;}
        [ForeignKey("Stores"), Key, Column(Order = 0)]
        public Store Store { get; private set; }
        [ForeignKey("Carts"), Key, Column(Order = 1)]
        public Cart Cart { get; private set; }

        // Every element in this collection is a 2-tuple: (product, amountToBuy)
        public ICollection<(Product, int)> Products { get; private set; }

        public Basket(Store store, Cart cart)
        {
            Store = store;
            Products = new List<(Product, int)>();
            Counter_ID++;
            Id = Counter_ID;
            Cart = cart;
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
