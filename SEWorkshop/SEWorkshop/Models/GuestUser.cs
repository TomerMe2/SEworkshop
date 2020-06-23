using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;
using SEWorkshop.DAL;
using System.Linq;

namespace SEWorkshop.Models
{
    public class GuestUser : User
    {
        public static int ID_COUNTER = 0;
        public int Id { get; set; }


        public GuestUser() : base()
        {
            Id = ID_COUNTER++;
        }

        public override void AddProductToCart(Product product, int quantity)
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
                        prod.Quantity += quantity;
                        return;
                    }
                    ProductsInBasket newPib = new ProductsInBasket(basket, product, quantity);
                    basket.Products.Add(newPib);
                    return;  // basket found and updated. Nothing more to do here...
                }
            }
            // if we got here, the correct basket doesn't exists now, so we should create it!
            Basket newBasket = new Basket(product.Store, cart);
            Cart.Baskets.Add(newBasket);
            ProductsInBasket pib = new ProductsInBasket(newBasket, product, quantity);
            newBasket.Products.Add(pib);
        }
        public override void RemoveProductFromCart(User user, Product product, int quantity)
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
                    if (quantityDelta > 0)
                    {
                        // The item should still be in the basket because it still has a positive quantity
                        prod.Quantity = quantityDelta;
                        return;
                    }
                    basket.Products.Remove(prod);
                    if (basket.Products.Count() == 0)
                    {
                        Cart.Baskets.Remove(basket);
                    }
                    return;
                }
            }
            throw new ProductIsNotInCartException();
        }

        override public Purchase Purchase(Basket basket, string creditCardNumber, DateTime expirationDate, string cvv, Address address, string username, string id)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            purchase = new Purchase(null, basket, address);
            
            foreach (var product in basket.Products)
            {
                if (product.Quantity <= 0)
                    throw new NegativeQuantityException();
            }
            basket.Store.PurchaseBasket(basket, creditCardNumber, expirationDate, cvv, address, this, username, id);
            basket.Store.Purchases.Add(purchase);
            DatabaseProxy.Instance.Baskets.Add(basket);
            foreach (var product in basket.Products)
            {
                DatabaseProxy.Instance.ProductsInBaskets.Add(product);
            }
            Cart.Baskets.Remove(basket);
            DatabaseProxy.Instance.Carts.Remove(Cart);
            DatabaseProxy.Instance.Purchases.Add(purchase);
            //DatabaseProxy.Instance.SaveChanges();
            return purchase;
        }
    }
}
