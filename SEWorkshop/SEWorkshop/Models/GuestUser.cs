using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Exceptions;

namespace SEWorkshop.Models
{
    public class GuestUser : User
    {
        public GuestUser() : base() { }
        
        override public void Purchase(Basket basket, string creditCardNumber, Address address)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            if (HasPermission)
                purchase = new Purchase(this, basket);
            else
                purchase = new Purchase(new GuestUser(), basket);
         
            ICollection<(Product, int)> productsToPurchase= new List<(Product, int)>();
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (purchaseQuantity <= 0)
                    throw new NegativeQuantityException();
                else
                    productsToPurchase.Add((prod, purchaseQuantity));
            }
            basket.Store.PurchaseBasket(productsToPurchase, creditCardNumber, address);
            Cart.Baskets.Remove(basket);
            basket.Store.Purchases.Add(purchase);
            // TODO when to add purchase to loggedin user purchase history
        }
    }
}
