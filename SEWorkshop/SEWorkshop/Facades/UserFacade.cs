using System;
using System.Collections.Generic;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.Adapters;
using SEWorkshop.DAL;
using System.Linq;

namespace SEWorkshop.Facades
{
    public class UserFacade : IUserFacade
    {
        private IStoreFacade StoreFacade { get; }
        private ICollection<GuestUser> GuestUsers { get; }
        private readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();

        public UserFacade(IStoreFacade storeFacade)
        {
            StoreFacade = storeFacade;
            GuestUsers = new List<GuestUser>();
            if (!DatabaseProxy.Instance.Administrators.Any())
            {
                DatabaseProxy.Instance.Administrators.Add(new Administrator(Administrator.ADMIN_USER_NAME, securityAdapter.Encrypt("sadnaTeam" )));
                DatabaseProxy.Instance.SaveChanges();
            }
        }

        public LoggedInUser GetLoggedInUser(string username)
        {
            var user = DatabaseProxy.Instance.LoggedInUsers.FirstOrDefault(usr => usr.Username.Equals(username));
            if (user == null)
            {
                var admin = DatabaseProxy.Instance.Administrators.OfType<Administrator>().FirstOrDefault(usr =>
                                                                                usr.Username.Equals(username));
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
            var user = DatabaseProxy.Instance.LoggedInUsers.FirstOrDefault(usr =>
                                        usr.Username.Equals(username) && usr.Password == password);
            if (user == null)
            {
                var admin = DatabaseProxy.Instance.Administrators.FirstOrDefault(usr => 
                                        usr.Username.Equals(username) && usr.Password == password);
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
            foreach(var user in DatabaseProxy.Instance.LoggedInUsers)
            {
                if(user.Username.Equals(username))
                {
                    throw new UserAlreadyExistsException();
                }
            }
            foreach(var admin in DatabaseProxy.Instance.Administrators)
            {
                if(admin.Username.Equals(username))
                {
                    throw new UserAlreadyExistsException();
                }
            }
            LoggedInUser newUser = new LoggedInUser(username, password);
            DatabaseProxy.Instance.LoggedInUsers.Add(newUser);
            DatabaseProxy.Instance.Carts.Add(newUser.Cart);
            DatabaseProxy.Instance.SaveChanges();
            return newUser;
        }

        public IEnumerable<Basket> MyCart(User user)
        {
            return user.Cart.Baskets;
        }
        
        public IEnumerable<Purchase> PurchaseHistory(LoggedInUser user)
        {
            ICollection<Purchase> userPurchases = new List<Purchase>();
            foreach(var purchase in DatabaseProxy.Instance.Purchases)
            {
                if(purchase.User == user)
                {
                    userPurchases.Add(purchase);
                }
            }
            return userPurchases;
        }

        public Purchase Purchase(User user, Basket basket, string creditCardNumber, Address address)
        {
            var prchs = user.Purchase(basket, creditCardNumber, address);
            DatabaseProxy.Instance.SaveChanges();
            return prchs;
        }

        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser requesting, string userNmToView)
        {
            if (DatabaseProxy.Instance.Administrators.FirstOrDefault(admin => admin.Username.Equals(requesting.Username)) == null)
            {
                throw new UserHasNoPermissionException();
            }
            var user = DatabaseProxy.Instance.LoggedInUsers.FirstOrDefault(user =>
                                                                            user.Username.Equals(userNmToView));
            if(user is null)
            {
                throw new UserDoesNotExistException();
            }
            return PurchaseHistory(user);
        }

        public void AddProductToCart(User user, Product product, int quantity)
        {
            if (!StoreFacade.IsProductExists(product))
            {
                throw new ProductNotInTradingSystemException();
            }
            user.AddProductToCart(product, quantity);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void RemoveProductFromCart(User user, Product product, int quantity)
        {
            if (!StoreFacade.IsProductExists(product))
            {
                throw new ProductNotInTradingSystemException();
            }
            user.RemoveProductFromCart(user, product, quantity);
            DatabaseProxy.Instance.SaveChanges();
        }

        public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store)
        {
            if (DatabaseProxy.Instance.Administrators.FirstOrDefault(admin => admin.Username.Equals(requesting.Username)) == null)
            {
                throw new UserHasNoPermissionException();
            }
            return DatabaseProxy.Instance.Purchases.Where(prchs => prchs.Basket.StoreName.Equals(store.Name));
        }

        public void WriteReview(LoggedInUser user, Product product, string description)
        {
            if (description.Length == 0)
            {
                throw new ReviewIsEmptyException();
            }
            Review review = new Review(user, description, product);
            product.Reviews.Add(review);
            user.Reviews.Add(review);
            DatabaseProxy.Instance.Reviews.Add(review);
            DatabaseProxy.Instance.SaveChanges();
        }

        public Message WriteMessage(LoggedInUser user, Store store, string description)
        {
            if (description.Length == 0)
            {
                throw new MessageIsEmptyException();
            }
            Message message = new Message(user, store, description, true);
            store.Messages.Add(message);
            user.Messages.Add(message);
            DatabaseProxy.Instance.Messages.Add(message);
            DatabaseProxy.Instance.SaveChanges();
            return message;
        }

        public GuestUser CreateGuestUser()
        {
            var guest = new GuestUser();
            GuestUsers.Add(guest);
            return guest;
        }

        public IEnumerable<string> GetRegisteredUsers()
        {
            return DatabaseProxy.Instance.LoggedInUsers.Select(user => user.Username);
        }

        public double GetIncomeInDate(DateTime date)
        {
            var relevants = DatabaseProxy.Instance.Purchases.Where(prchs => prchs.TimeStamp.Year == date.Year
                                                    && prchs.TimeStamp.Month == date.Month
                                                    && prchs.TimeStamp.Day == date.Day);
            double output = 0.0;
            foreach(var prchs in relevants)
            {
                output += prchs.MoneyPaid;
            }
            return output;
        }
    }
}