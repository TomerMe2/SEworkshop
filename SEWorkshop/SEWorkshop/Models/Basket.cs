using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;
using System.Linq;

namespace SEWorkshop.Models
{
    public class Basket
    {
        public virtual int Id {get; private set;}
        public virtual Store Store { get; private set; }
        public virtual Purchase? Purchase { get; set; }
        public virtual Cart? Cart { get; private set; }
        public virtual int? CartId { get; set; }
        public virtual string StoreName { get; set; }

        // Every element in this collection is a 2-tuple: (product, amountToBuy)
        public virtual ICollection<ProductsInBasket> Products { get; private set; }

        private Basket()
        {
            Products = new List<ProductsInBasket>();
            Cart = null!;
            StoreName = "";
            Store = null!;
        }

        public Basket(Store store, Cart cart)
        {
            Store = store;
            //Products = (ICollection<ProductsInBasket>)DbContext.ProductsInBaskets.Select(prod => prod.Basket.Equals(this));
            Products = new List<ProductsInBasket>();
            Cart = cart;
            StoreName = store.Name;
        }

        public double PriceWithoutDiscount()
        {
            double totalPrice = 0;
            foreach (var product in Products)
            {
                totalPrice += product.Product.Price * product.Quantity;
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
