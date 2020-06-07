using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public abstract class User
    {
        public virtual int Id { get; set; }

        public virtual Cart Cart { get; set; }


        public User()
        {
            Cart = new Cart(this);
        }

        public void AddProductToCart(Product product, int quantity)
        {
            if (quantity < 1)
            {
                throw new NegativeQuantityException();
            }
            if (product.Quantity - quantity < 0)
            {
                throw new NegativeInventoryException();
            }
            Cart cart = this.Cart;
            foreach (var basket in cart.Baskets)
            {
                if (product.Store == basket.Store)
                {
                    var prod = basket.Products.FirstOrDefault(tup => tup.Product.Equals(product));
                    if (!(prod is null))
                    {
                        quantity = quantity + prod.Quantity;
                        // we are doing this because of the fact that when a tuple is assigned, it's copied and int is a primitive...
                        basket.Products.Remove(prod);  //so we can add it later :)
                    }
                    basket.Products.Add(new ProductsInBasket(basket, product, quantity));
                    return;  // basket found and updated. Nothing more to do here...
                }
            }
            // if we got here, the correct basket doesn't exists now, so we should create it!
            Basket newBasket = new Basket(product.Store, cart);
            Cart.Baskets.Add(newBasket);
            newBasket.Products.Add(new ProductsInBasket(newBasket, product, quantity));
        }

        public void RemoveProductFromCart(User user, Product product, int quantity)
        {
            if (quantity < 1)
            {
                throw new NegativeQuantityException();
            }
            foreach (var basket in user.Cart.Baskets)
            {
                if (product.Store == basket.Store)
                {
                    var prod = basket.Products.FirstOrDefault(tup => tup.Product.Equals(product));
                    if (prod is null)
                    {
                        throw new ProductIsNotInCartException();
                    }
                    int quantityDelta = prod.Quantity - quantity;
                    if (quantityDelta < 0)
                    {
                        throw new ArgumentOutOfRangeException("quantity in cart minus quantity is smaller then 0");
                    }
                    basket.Products.Remove(prod);
                    if (quantityDelta > 0)
                    {
                        // The item should still be in the basket because it still has a positive quantity
                        basket.Products.Add(prod);
                    }
                    return;
                }
            }
            throw new ProductIsNotInCartException();
        }

        public abstract Purchase Purchase(Basket basket, string creditCardNumber, Address address);
    }
}