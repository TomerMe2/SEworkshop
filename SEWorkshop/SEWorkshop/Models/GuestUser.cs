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
        public DateTime TimeStamp { get; }

        public GuestUser() : base() 
        {
            TimeStamp = DateTime.Now;
            lock(nextIdLock)
            {
                Id = nextId;
                nextId++;
            }
        }
        
        override public Purchase Purchase(Basket basket, string creditCardNumber, Address address, UserFacade facade)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            purchase = new Purchase(this, basket, address);
            
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (purchaseQuantity <= 0)
                    throw new NegativeQuantityException();
            }
            basket.Store.PurchaseBasket(basket, creditCardNumber, address, this);
            Cart.Baskets.Remove(basket);
            basket.Store.Purchases.Add(purchase);
            return purchase;
        }
    }
}
