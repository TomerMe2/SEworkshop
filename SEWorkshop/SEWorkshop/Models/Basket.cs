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
        public virtual int Id {get; set;}
        public virtual Store Store { get; set; }
        public virtual int? PurchaseId { get; set; }
        public virtual Purchase? Purchase { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual int? CartId { get; set; }
        public virtual string StoreName { get; set; }

        // Every element in this collection is a 2-tuple: (product, amountToBuy)
        public virtual ICollection<ProductsInBasket> Products { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Basket()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        public Basket(Store store, Cart cart)
        {
            Store = store;
            Products = new List<ProductsInBasket>();
            Cart = cart;
            StoreName = store.Name;
        }

        public double PriceWithoutDiscount()
        {
            double totalPrice = 0;
            if(Products is null)
                Products = DatabaseProxy.Instance.ProductsInBaskets.Where(pib => pib.BasketId == Id).ToList();
            foreach (var product in Products)
            {
                if (product.Product is null)
                    product.Product = DatabaseProxy.Instance.Products.First(prod => prod.Id == product.ProductId);
                totalPrice += product.Product.Price * product.Quantity;
            }

            return totalPrice;
        }

        public double PriceAfterDiscount()
        {
            double price = PriceWithoutDiscount();
            foreach (var dis in Store.Discounts.ToList())
            {
                if(dis.Father is null)
                    price -= dis.ComputeDiscount(Products);
            }

            return price;
        }
    }
}
