using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;

namespace SEWorkshop.Models
{
    public class GuestUser : User
    {
        private static int nextId = 0;
        private static object nextIdLock = new object();
        public int Id { get; }

        public GuestUser() : base() 
        {
            lock(nextIdLock)
            {
                Id = nextId;
                nextId++;
            }
        }
        
        override public void Purchase(Basket basket, string creditCardNumber, Address address, UserFacade facade)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            purchase = new Purchase(new GuestUser(), basket, address);
         
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
        }
    }
}
