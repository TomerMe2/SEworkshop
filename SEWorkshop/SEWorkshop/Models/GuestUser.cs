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

        public GuestUser() : base()
        { 

        }
        
        override public Purchase Purchase(Basket basket, string creditCardNumber, Address address)
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
            DatabaseProxy.Instance.Purchases.Add(purchase);
            DatabaseProxy.Instance.SaveChanges();
            return purchase;
        }
    }
}
