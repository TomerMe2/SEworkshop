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

        private static readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private static readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private static readonly ISecurityAdapter securityAdapter = new SecurityAdapter();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public UserFacade(IStoreFacade storeFacade)
        {
            StoreFacade = storeFacade;
            Users = new List<LoggedInUser>();
            Purchases = new List<Purchase>();
            Administrators = new List<LoggedInUser>(){new Administrator("admin", securityAdapter.Encrypt("sadnaTeam"))};
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
        
        public void AddProductToCart(User user, Product product, int quantity)
        {
            log.Info("User tries to add a product to cart");
            if (!StoreFacade.IsProductExists(product))
            {
                log.Info("User tried to add an unexisting product to cart");
                throw new ProductNotInTradingSystemException();
            }
            if (quantity < 1)
            {
                log.Info("User tried to add a product with no quantity to cart");
                throw new NegativeQuantityException();
            }
            if (product.Quantity - quantity < 0)
            {
                log.Info("User tried to add a product with unavailable amount to cart");
                throw new NegativeInventoryException();
            }
            Cart cart = user.Cart;
            foreach(var basket in cart.Baskets)
            {
                if(product.Store == basket.Store)
                {
                    var (recordProd, recordQuan) = basket.Products.FirstOrDefault(tup => tup.Item1 == product);
                    if (!(recordProd is null))
                    {
                        quantity = quantity + recordQuan;
                        // we are doing this because of the fact that when a tuple is assigned, it's copied and int is a primitive...
                        basket.Products.Remove((recordProd, recordQuan));  //so we can add it later :)
                    }
                    basket.Products.Add((product, quantity));
                    log.Info("Product has been added to cart successfully");
                    return;  // basket found and updated. Nothing more to do here...
                }
            }
            // if we got here, the correct basket doesn't exists now, so we should create it!
            log.Info("Product has been added to a new basket in cart");
            Basket newBasket = new Basket(product.Store);
            user.Cart.Baskets.Add(newBasket);
            newBasket.Products.Add((product, quantity));
        }

        public void RemoveProductFromCart(User user, Product product, int quantity)
        {
            log.Info("User tries to remove a product from cart");
            if (quantity < 1)
            {
                log.Info("User tried to remove a product with no quantity from cart");
                throw new NegativeQuantityException();
            }
            foreach (var basket in user.Cart.Baskets)
            {
                if(product.Store == basket.Store)
                {
                    var (recordProd, recordQuan) = basket.Products.FirstOrDefault(tup => tup.Item1 == product);
                    if (recordProd is null)
                    {
                        log.Info("User tried to remove a product that is not in the cart");
                        throw new ProductIsNotInCartException();
                    }
                    int quantityDelta = recordQuan - quantity;
                    if (quantityDelta < 0)
                    {
                        log.Info("User tried to remove too much of this product");
                        throw new ArgumentOutOfRangeException("quantity in cart minus quantity is smaller then 0");
                    }
                    basket.Products.Remove((recordProd, recordQuan));
                    if (quantityDelta > 0)
                    {
                        // The item should still be in the basket because it still has a positive quantity
                        basket.Products.Add((product, quantityDelta));
                    }
                    log.Info("Product has been removed from cart successfully");
                    return;
                }
            }
            log.Info("User tried to remove a product that is not in the cart");
            throw new ProductIsNotInCartException();
        }

        public void Purchase(User user, Basket basket, string creditCardNumber, Address address)
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
        }

        public IEnumerable<Purchase> PurcahseHistory(User user)
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
            return PurcahseHistory(user);
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
                foreach (var purchase in PurcahseHistory(user))
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
            store.Messages.Add(new Message(user, description));
            ((LoggedInUser) user).Messages.Add(message);
            log.Info("The message has been published successfully");
        }
    }
}