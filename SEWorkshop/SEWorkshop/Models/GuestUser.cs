using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;
using SEWorkshop.DAL;

namespace SEWorkshop.Models
{
    public class GuestUser : User
    {
        private static int nextId = 0;
        private static object nextIdLock = new object();
        public virtual int Id { get; set; }

        public GuestUser(AppDbContext dbContext) : base(dbContext)
        {
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
            purchase = new Purchase(null, basket, address);
            
            foreach (var product in basket.Products)
            {
                if (product.Quantity <= 0)
                    throw new NegativeQuantityException();
            }
            basket.Store.PurchaseBasket(basket, creditCardNumber, address, this);
            Cart.Baskets.Remove(basket);
            basket.Store.Purchases.Add(purchase);
            return purchase;
        }
    }
}
