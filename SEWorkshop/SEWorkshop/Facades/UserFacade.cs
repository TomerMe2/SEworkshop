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
        private ICollection<LoggedInUser> RegisteredUsers {get; }
        private ICollection<GuestUser> GuestUsers { get; }
        private ICollection<LoggedInUser> Administrators { get; }
        private ICollection<Purchase> Purchases {get; }


        private readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public UserFacade(IStoreFacade storeFacade)
        {
            StoreFacade = storeFacade;
            RegisteredUsers = new List<LoggedInUser>();
            Purchases = new List<Purchase>();
            Administrators = new List<LoggedInUser>(){new Administrator("admin", securityAdapter.Encrypt("sadnaTeam"))};
            GuestUsers = new List<GuestUser>();
        }

        public void AddPurchaseToList(Purchase p)
        {
            Purchases.Add(p);
        }

        public LoggedInUser GetLoggedInUser(string username)
        {
            var user = RegisteredUsers.FirstOrDefault(usr => usr.Username.Equals(username));
            if (user == null)
            {
                var admin = Administrators.FirstOrDefault(usr => usr.Username.Equals(username));
                if (admin == null)
                {
                    throw new UserDoesNotExistException();
                }
                return admin;
            }
            return user;
        }

        public LoggedInUser GetLoggedInUser(string username, byte[] password)
        {
            var user = RegisteredUsers.FirstOrDefault(usr => usr.Username.Equals(username) && usr.Password.SequenceEqual(password));
            if (user == null)
            {
                var admin = Administrators.FirstOrDefault(usr => usr.Username.Equals(username) && usr.Password.SequenceEqual(password));
                if (admin == null)
                {
                    throw new UserDoesNotExistException();
                }
                return admin;
            }
            return user;
        }

        public GuestUser GetGuestUser(int id)
        {
            var guest = GuestUsers.FirstOrDefault(usr => usr.Id == id);
            if (guest == null)
            {
                throw new UserDoesNotExistException();
            }
            return guest;
        }
        
        /// <summary>
        /// Password is SHA256 encrypted
        /// </summary>
        public LoggedInUser Register(string username, byte[] password)
        {
            log.Info("User tries to register with the username: {0}", username);
            foreach(var user in RegisteredUsers)
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
            RegisteredUsers.Add(newUser);
            log.Info("Registration has been completed successfully");
            return newUser;
        }

        public IEnumerable<Basket> MyCart(User user)
        {
            log.Info("User fetches cart data");
            return user.Cart.Baskets;
        }
        
        public IEnumerable<Purchase> PurchaseHistory(LoggedInUser user)
        {
            log.Info("User tries to seek its purchase history");
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
            var user = RegisteredUsers.Concat(Administrators).FirstOrDefault(user => user.Username.Equals(userNmToView));
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

        public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store)
        {
            log.Info("Admin tries to seek {0}'s purchase history", store.Name);
            if (!Administrators.Contains(requesting))
            {
                log.Info("A unauthorized user tried to fetch data without permission");
                throw new UserHasNoPermissionException();
            }
            ICollection<Purchase> purchaseHistory = new List<Purchase>();
            foreach (var user in RegisteredUsers)
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

        public void WriteReview(LoggedInUser user, Product product, string description)
        {
            log.Info("User tries to write a review");
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

        public void WriteMessage(LoggedInUser user, Store store, string description)
        {
            log.Info("User tries to write a message");
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

        public GuestUser CreateGuestUser()
        {
            var guest = new GuestUser();
            GuestUsers.Add(guest);
            return guest;
        }
    }
}