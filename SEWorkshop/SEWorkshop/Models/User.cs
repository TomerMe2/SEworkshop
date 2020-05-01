using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Models
{
    public class User
    {
        public Cart Cart { get; set; }
<<<<<<< Updated upstream
        public bool HasPermission {get; private set;}
=======
        public bool HasPermission { get; private set; }
>>>>>>> Stashed changes
        private static readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private static readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private static readonly ISecurityAdapter securityAdapter = new SecurityAdapter();

        public User()
        {
            Cart = new Cart();
<<<<<<< Updated upstream
            HasPermission=false;
        }

        public void AddProductToCart(Product product, int quantity)
        { 
            //TODO ask amit about store products
            if (!StoreFacade.GetInstance().IsProductExists(product))
            {
               throw new ProductNotInTradingSystemException();
            }
            if (quantity < 1)
            {
              throw new NegativeQuantityException();
            }
            if (product.Quantity - quantity < 0)
            {
              throw new NegativeInventoryException();
=======
            HasPermission = false;
        }

        public void AddProductToCart(Product product, int quantity)
        {
            //TODO ask amit about store products
            if (!StoreFacade.GetInstance().IsProductExists(product))
            {
                throw new ProductNotInTradingSystemException();
            }
            if (quantity < 1)
            {
                throw new NegativeQuantityException();
            }
            if (product.Quantity - quantity < 0)
            {
                throw new NegativeInventoryException();
>>>>>>> Stashed changes
            }
            Cart cart = this.Cart;
            foreach (var basket in cart.Baskets)
            {
                if (product.Store == basket.Store)
                {
                    var (recordProd, recordQuan) = basket.Products.FirstOrDefault(tup => tup.Item1 == product);
                    if (!(recordProd is null))
                    {
                        quantity = quantity + recordQuan;
                        // we are doing this because of the fact that when a tuple is assigned, it's copied and int is a primitive...
                        basket.Products.Remove((recordProd, recordQuan));  //so we can add it later :)
                    }
                    basket.Products.Add((product, quantity));
                    return;  // basket found and updated. Nothing more to do here...
                }
            }
            // if we got here, the correct basket doesn't exists now, so we should create it!
            Basket newBasket = new Basket(product.Store);
            Cart.Baskets.Add(newBasket);
            newBasket.Products.Add((product, quantity));
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
                    var (recordProd, recordQuan) = basket.Products.FirstOrDefault(tup => tup.Item1 == product);
                    if (recordProd is null)
                    {
                        throw new ProductIsNotInCartException();
                    }
                    int quantityDelta = recordQuan - quantity;
                    if (quantityDelta < 0)
                    {
                        throw new ArgumentOutOfRangeException("quantity in cart minus quantity is smaller then 0");
                    }
                    basket.Products.Remove((recordProd, recordQuan));
                    if (quantityDelta > 0)
                    {
                        // The item should still be in the basket because it still has a positive quantity
                        basket.Products.Add((product, quantityDelta));
                    }
                    return;
                }
            }
            throw new ProductIsNotInCartException();
        }

        public void Purchase(User user, Basket basket)
        {
            const string CREDIT_CARD_NUMBER_STUB = "1234";
            const string CITY_NAME_STUB = "Beer Sheva";
            const string STREET_NAME_STUB = "Shderot Ben Gurion";
            const string HOUSE_NUMBER_STUB = "111";
            if (basket.Products.Count == 0)
            {
                throw new BasketIsEmptyException();
            }
            Purchase purchase;
            if (HasPermission)
                purchase = new Purchase(user, basket);
            else
                purchase = new Purchase(new GuestUser(), basket);
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (purchaseQuantity <= 0)
                {
                    throw new NegativeQuantityException();
                }
            }
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (prod.Quantity - purchaseQuantity < 0)
                {
                    throw new NegativeInventoryException();
                }
            }
            if (supplyAdapter.CanSupply(basket.Products, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB)
                && billingAdapter.Bill(basket.Products, CREDIT_CARD_NUMBER_STUB))
            {
                supplyAdapter.Supply(basket.Products, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB);
                user.Cart.Baskets.Remove(basket);
                basket.Store.Purchases.Add(purchase);
                // Update the quantity in the product itself
                foreach (var (prod, purchaseQuantity) in basket.Products)
                {
                    prod.Quantity = prod.Quantity - purchaseQuantity;
                }
                //TODO ask if user is loggedin add it to user purchases
                // add purchase to store purchase history
                //Purchases.Add(purchase);
            }
            else
            {
                throw new PurchaseFailedException();
            }
        }
    }
}