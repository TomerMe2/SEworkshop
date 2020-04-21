using System;
using System.Collections.Generic;
using NLog;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;

namespace SEWorkshop.Facades
{
    public class UserFacade : IUserFacade
    {
        private ICollection<LoggedInUser> Users {get; set;}
        private ICollection<LoggedInUser> Administrators {get; set;}

        private ICollection<Purchase> Purchases {get; set;}
        public bool HasPermission {get; private set;}
        private static UserFacade? Instance = null;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

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
            Administrators = new List<LoggedInUser>(){new Administrator("admin", "sadnaTeam")};
            HasPermission = false;
        }

        public LoggedInUser GetUser(string username)
        {
            foreach(var user in Users)
            {
                if(user.Username.Equals(username))
                {
                    return user;
                }
            }
            throw new UserDoesNotExistException();
        }
        
        public LoggedInUser Register(string username, string password)
        {
            if(HasPermission)
            {
                log.Info("Logged in user tried to register");
                throw new UserAlreadyLoggedInException();
            }
            foreach(var user in Users)
            {
                if(user.Username.Equals(username))
                {
                    throw new UserAlreadyExistsException();
                }
            }
            foreach(var admin in Administrators)
            {
                if(admin.Username.Equals(username))
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
            foreach(var admin in Administrators)
            {
                if(admin.Username.Equals(username))
                {
                    if(admin.Password.Equals(password))
                    {
                        HasPermission = true;
                        return admin;
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
            if (!StoreFacade.GetInstance().IsProductExists(product))
                throw new ProductNotInTradingSystemException();
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
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            if(HasPermission)
            {
                purchase = new Purchase(user, basket);
            }
            else
            {
                purchase = new Purchase(new GuestUser(), basket);
            }
            user.Cart.Baskets.Remove(basket);
            basket.Store.Purchases.Add(purchase);
            Purchases.Add(purchase);
        }

        public IEnumerable<Purchase> PurcahseHistory(User user)
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

        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser requesting, LoggedInUser user)
        {
            if (!Administrators.Contains(requesting))
            {
                throw new UserHasNoPermissionException();
            }
            return PurcahseHistory(user);
        }

        public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store)
        {
            if (!Administrators.Contains(requesting))
            {
                throw new UserHasNoPermissionException();
            }
            ICollection<Purchase> purchaseHistory = new List<Purchase>();
            foreach (var user in Users)
            {
                foreach (var purchase in PurcahseHistory(user))
                {
                    if (purchase.Basket.Store.Equals(store))
                    {
                        purchaseHistory.Add(purchase);
                    }
                }
            }
            return purchaseHistory;
        }

        public void WriteReview(User user, Product product, string description)
        {
            if(!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            Review review = new Review(user, description);
            product.Reviews.Add(review);
            ((LoggedInUser) user).Reviews.Add(review);
        }
        public void WriteMessage(User user, Store store, string description)
        {
            if(!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            Message message = new Message(user, description);
            store.Messages.Add(new Message(user, description));
            ((LoggedInUser) user).Messages.Add(message);
        }
    }
}