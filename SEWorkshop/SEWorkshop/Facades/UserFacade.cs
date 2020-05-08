using System;
using System.Collections.Generic;
using NLog;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.Adapters;
using System.Linq;

namespace SEWorkshop.Facades
{
    public class UserFacade : IUserFacade
    {
        private IStoreFacade StoreFacade { get; }
        private ICollection<LoggedInUser> Users {get; set;}
        private ICollection<LoggedInUser> Administrators {get; set;}
        private ICollection<Purchase> Purchases {get; set;}
        public bool HasPermission {get; private set;}

        private readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public UserFacade(IStoreFacade storeFacade)
        {
            StoreFacade = storeFacade;
            Users = new List<LoggedInUser>();
            Purchases = new List<Purchase>();
            Administrators = new List<LoggedInUser>(){new Administrator("admin", securityAdapter.Encrypt("sadnaTeam"))};
            HasPermission = false;
        }

        public void AddPurchaseToList(Purchase p)
        {
            Purchases.Add(p);
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
        
        /// <summary>
        /// Password is SHA256 encrypted
        /// </summary>
        public LoggedInUser Register(string username, byte[] password)
        {
            log.Info("User tries to register with the username: {0}", username);
            if(HasPermission)
            {
                log.Info("Logged in user tried to register");
                throw new UserAlreadyLoggedInException();
            }
            foreach(var user in Users)
            {
                if(user.Username.Equals(username))
                {
                    log.Info("User tried to register with already existing username");
                    throw new UserAlreadyExistsException();
                }
            }
            foreach(var admin in Administrators)
            {
                if(admin.Username.Equals(username))
                {
                    log.Info("User tried to register with already existing admin username");
                    throw new UserAlreadyExistsException();
                }
            }
            LoggedInUser newUser = new LoggedInUser(username, password);
            Users.Add(newUser);
            log.Info("Registration has been completed successfully");
            return newUser;
        }

        /// <summary>
        /// Password is SHA256 encrypted
        /// </summary>
        public LoggedInUser Login(string username, byte[] password)
        {
            log.Info("User tries to login with the username: {0}", username);
            if(HasPermission)
            {
                log.Info("Logged in user tried to log in");
                throw new UserAlreadyLoggedInException();
            }
            foreach(var user in Users)
            {
                if(user.Username.Equals(username))
                {
                    if(user.Password.SequenceEqual(password))
                    {
                        log.Info("Logging in has been completed successfully");
                        HasPermission = true;
                        user.HasPermission = true;
                        return user;
                    }
                    break;
                }
            }
            foreach(var admin in Administrators)
            {
                if(admin.Username.Equals(username))
                {
                    if(admin.Password.SequenceEqual(password))
                    {
                        log.Info("Logging in has been completed successfully");
                        HasPermission = true;
                        return admin;
                    }
                    break;
                }
            }
            log.Info("User tried to log in with non existing username");
            throw new UserDoesNotExistException();
        }

        public void Logout()
        {
            log.Info("User tries to logout");
            if(!HasPermission)
            {
                log.Info("User tried to logout while being a guest user");
                throw new UserHasNoPermissionException();
            }
            log.Info("Logout has been completed successfully");
            HasPermission = false;
        }

        public IEnumerable<Basket> MyCart(User user)
        {
            log.Info("User fetches cart data");
            return user.Cart.Baskets;
        }
        
      
      
        public IEnumerable<Purchase> PurchaseHistory(User user)
        {

            log.Info("User tries to seek its purchase history");
            if(!HasPermission)
            {
                log.Info("An unsigned in user cannot have permission for that action");
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
            log.Info("Purchase history seek has been completed successfully");
            return userPurchases;
        }
        public void Purchase(User user, Basket basket, string creditCardNumber, Address address)
        {
            user.Purchase(basket, creditCardNumber, address, this);
        }
        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser requesting, string userNmToView)
        {
            log.Info("Admin tries to seek {0}'s purchase history", userNmToView);
            if (!Administrators.Contains(requesting))
            {
                log.Info("A unauthorized user tried to fetch data without permission");
                throw new UserHasNoPermissionException();
            }
            var user = Users.Concat(Administrators).FirstOrDefault(user => user.Username.Equals(userNmToView));
            if(user is null)
            {
                log.Info("User does not exist");
                throw new UserDoesNotExistException();
            }
            log.Info("Data has been fetched successfully");
            return PurchaseHistory(user);
        }
        public void AddProductToCart(User user, Product product, int quantity)
        {
            if (!StoreFacade.IsProductExists(product))
            {
                throw new ProductNotInTradingSystemException();
            }
            user.AddProductToCart(product, quantity);
        }

        public void RemoveProductFromCart(User user, Product product, int quantity)
        {
            if (!StoreFacade.IsProductExists(product))
            {
                throw new ProductNotInTradingSystemException();
            }
            user.RemoveProductFromCart(user, product, quantity);
        }

        /*public void Purchase(User user, Basket basket, string creditCardNumber, Address address)
        {
            log.Info("User tries to purchase a basket");
            if (basket.Products.Count == 0)
            {
                log.Info("User tried to purchase an empty basket");
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
                    log.Info("User tried to purchase a non positive amount of a product");
                    throw new NegativeQuantityException();
                }
            }
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (prod.Quantity - purchaseQuantity < 0)
                {
                    log.Info("User tries to purchase unavailable amount of a product");
                    throw new NegativeInventoryException();
                }
            }
            if (supplyAdapter.CanSupply(basket.Products, address)
                && billingAdapter.Bill(basket.Products, creditCardNumber))
            {
                supplyAdapter.Supply(basket.Products, address);
                user.Cart.Baskets.Remove(basket);
                basket.Store.Purchases.Add(purchase);
                // Update the quantity in the product itself
                foreach(var (prod, purchaseQuantity) in basket.Products)
                {
                    prod.Quantity = prod.Quantity - purchaseQuantity;
                }
                Purchases.Add(purchase);
                log.Info("Purchase has been completed successfully");
            }
            else
            {
                log.Info("Purchase has failed");
                throw new PurchaseFailedException();
            }
        }*/

        /*public IEnumerable<Purchase> PurcahseHistory(User user)
        {
            if (!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            if (description.Length == 0)
            {
                throw new MessageIsEmptyException();
            }
            Message message = new Message(loggedInUser, description);
            store.Messages.Add(message);
            ((LoggedInUser)loggedInUser).Messages.Add(message);
        }*/
        public IEnumerable<Purchase> UserPurchaseHistory(User LoggedInUser, string userNmToView)
        {
            if (!Administrators.Contains(LoggedInUser))
            {
                throw new UserHasNoPermissionException();
            }
            var user = Users.Concat(Administrators).FirstOrDefault(user => user.Username.Equals(userNmToView));
            if (user is null)
            {
                throw new UserDoesNotExistException();
            }
            log.Info("Data has been fetched successfully");
            return PurchaseHistory(user);
        }


        public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store)
        {
            log.Info("Admin tries to seek {0}'s purchase history", store.Name);
            if (!Administrators.Contains(requesting))
            {
                log.Info("A unauthorized user tried to fetch data without permission");
                throw new UserHasNoPermissionException();
            }
            ICollection<Purchase> purchaseHistory = new List<Purchase>();
            foreach (var user in Users)
            {
                foreach (var purchase in PurchaseHistory(user))
                {
                    if (purchase.Basket.Store.Equals(store))
                    {
                        purchaseHistory.Add(purchase);
                    }
                }
            }
            log.Info("Data has been fetched successfully");
            return purchaseHistory;
        }

        public void WriteReview(User user, Product product, string description)
        {
            log.Info("User tries to write a review");
            if (!HasPermission)
            {
                log.Info("An unsigned in user cannot have permission for that action");
                throw new UserHasNoPermissionException();
            }
            if (description.Length == 0)
            {
                log.Info("The review is empty");
                throw new ReviewIsEmptyException();
            }
            Review review = new Review(user, description);
            product.Reviews.Add(review);
            ((LoggedInUser) user).Reviews.Add(review);
            log.Info("The review has been published successfully");
        }

        public void WriteMessage(User user, Store store, string description)
        {
            log.Info("User tries to write a message");
            if (!HasPermission)
            {
                log.Info("An unsigned in user cannot have permission for that action");
                throw new UserHasNoPermissionException();
            }
            if (description.Length == 0)
            {
                log.Info("The message is empty");
                throw new MessageIsEmptyException();
            }
            Message message = new Message(user, description);
            store.Messages.Add(message);
            ((LoggedInUser) user).Messages.Add(message);
            log.Info("The message has been published successfully");
        }
    }
}