using System;
using System.Collections.Generic;
using SEWorkshop.Exceptions;

namespace SEWorkshop.Facades
{
    public class UserFacade : IUserFacade
    {
        private ICollection<LoggedInUser> Users {get; set;}
        private ICollection<Purchase> Purchases {get; set;}
        public bool HasPermission {get; private set;}
        private static UserFacade Instance = null;

        public static UserFacade GetInstance()
        {
            if (Instance == null)
                Instance = new UserFacade();
            return Instance;
        }

        private UserFacade()
        {
            Users = new List<LoggedInUser>();
            Purchases = new List<Purchase>();
            HasPermission = false;
        }
        
        public LoggedInUser Register(string username, string password)
        {
            if(HasPermission)
            {
                throw new UserAlreadyLoggedInException();
            }
            foreach(var user in Users)
            {
                if(user.Username.Equals(username))
                {
                    throw new UserAlreadyExistsException();
                }
            }
            LoggedInUser newUser = new LoggedInUser(username, password);
            Users.Add(newUser);
            return newUser;
        }

        public LoggedInUser Login(string username, string password)
        {
            if(HasPermission)
            {
                throw new UserAlreadyLoggedInException();
            }
            foreach(var user in Users)
            {
                if(user.Username.Equals(username))
                {
                    if(user.Password.Equals(password))
                    {
                        HasPermission = true;
                        return user;
                    }
                    break;
                }
            }
            throw new UserDoesNotExistException();
        }

        public void Logout()
        {
            if(!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            HasPermission = false;
        }
        public IEnumerable<Basket> MyCart(User user)
        {
            return user.Cart.Baskets;
        }
        
        public void AddProductToCart(User user, Product product)
        {
            Cart cart = user.Cart;
            foreach(var basket in cart.Baskets)
            {
                if(product.Store == basket.Store)
                {
                    basket.Products.Add(product);
                }
            }
            Basket newBasket = new Basket(product.Store);
            newBasket.Products.Add(product);
            user.Cart.Baskets.Add(newBasket);
        }

        public void RemoveProductFromCart(User user, Product product)
        {
            foreach(var basket in user.Cart.Baskets)
            {
                if(product.Store == basket.Store)
                {
                    basket.Products.Remove(product);
                }
            }
            throw new ProductIsNotInCartException();
        }

        public void Purchase(User user, Basket basket)
        {
            Purchase purchase;
            if(HasPermission)
            {
                purchase = new Purchase(user, basket);
            }
            else
            {
                purchase = new Purchase(new GuestUser(), basket);
            }
            Purchases.Add(purchase);
        }

        public IEnumerable<Purchase> WatchPurcahseHistory(User user)
        {
            if(!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            ICollection<Purchase> userPurchases = new List<Purchase>();
            foreach(var purchase in Purchases)
            {
                if(purchase.User == user)
                {
                    userPurchases.Add(purchase);
                }
            }
            return userPurchases;
        }
    }
}