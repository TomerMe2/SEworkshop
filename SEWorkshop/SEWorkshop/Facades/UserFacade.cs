using System;
using System.Collections.Generic;
using NLog;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.Facades.Adapters;
using System.Linq;

namespace SEWorkshop.Facades
{
    public class UserFacade : IUserFacade
    {
        private ICollection<LoggedInUser> Users {get; set;}
        private ICollection<Purchase> Purchases {get; set;}
        public bool HasPermission {get; private set;}
        private static UserFacade Instance = null;

        private static readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private static readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();

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
            HasPermission = false;
        }
        
        /// <summary>
        /// Password is SHA256 encrypted
        /// </summary>
        public LoggedInUser Register(string username, byte[] password)
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
            LoggedInUser newUser = new LoggedInUser(username, password);
            Users.Add(newUser);
            return newUser;
        }

        /// <summary>
        /// Password is SHA256 encrypted
        /// </summary>
        public LoggedInUser Login(string username, byte[] password)
        {
            if(HasPermission)
            {
                throw new UserAlreadyLoggedInException();
            }
            foreach(var user in Users)
            {
                if(user.Username.Equals(username))
                {
                    if(user.Password.SequenceEqual(password))
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
            const string CREDIT_CARD_NUMBER_STUB = "1234";
            const string CITY_NAME_STUB = "Beer Sheva";
            const string STREET_NAME_STUB = "Sderot Ben Gurion";
            const string HOUSE_NUMBER_STUB = "111";
            Purchase purchase;
            if(HasPermission)
            {
                purchase = new Purchase(user, basket);
            }
            else
            {
                purchase = new Purchase(new GuestUser(), basket);
            }
            if(billingAdapter.Bill(basket.Products, CREDIT_CARD_NUMBER_STUB))
            {
                supplyAdapter.Supply(basket.Products, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB);
                Purchases.Add(purchase);
            }
            else
            {
                throw new PurchaseFailedException();
            }
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
    }
}