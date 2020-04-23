using System;
using System.Collections.Generic;
using SEWorkshop.Facades;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using NLog;

namespace SEWorkshop.ServiceLayer
{
    public class UserManager : IUserManager
    {
        UserFacade UserFacadeInstance = UserFacade.GetInstance();
        ManageFacade ManageFacadeInstance = ManageFacade.GetInstance();
        readonly StoreFacade StoreFacadeInstance = StoreFacade.GetInstance();
        User User = new GuestUser();
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public UserManager()
        {
        }

        public void AddProductToCart(Product product)
        {
            UserFacadeInstance.AddProductToCart(User, product);
        }

        public IEnumerable<Store> BrowseStores()
        {
            return StoreFacadeInstance.BrowseStores();
        }

        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            return StoreFacadeInstance.FilterProducts(products, pred);
        }

        public void Login(string username, string password)
        {
            Cart cart = User.Cart;
            User = UserFacadeInstance.Login(username, password);
            User.Cart = cart;
        }

        public void Logout()
        {
            Cart cart = User.Cart;
            UserFacadeInstance.Logout();
            User = new GuestUser();
            User.Cart = cart;
        }

        public IEnumerable<Basket> MyCart()
        {
            return UserFacadeInstance.MyCart(User);
        }

        public void OpenStore(LoggedInUser owner, string storeName)
        {
            StoreFacadeInstance.CreateStore(owner, storeName);
        }

        public void Purchase(Basket basket)
        {
            UserFacadeInstance.Purchase(User, basket);
        }

        public void Register(string username, string password)
        {
            UserFacadeInstance.Register(username, password);
        }

        public void RemoveProductFromCart(Product product)
        {
            UserFacadeInstance.RemoveProductFromCart(User, product);
        }

        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            return StoreFacadeInstance.SearchProducts(pred);
        }

        public IEnumerable<Purchase> PurcahseHistory()
        {
            return UserFacadeInstance.PurcahseHistory(User);
        }

        public IEnumerable<Purchase> UserPurchaseHistory(User user)
        {
            return UserFacadeInstance.UserPurchaseHistory(User, user);
        }

        public IEnumerable<Purchase> StorePurchaseHistory(Store store)
        {
            return UserFacadeInstance.StorePurchaseHistory(User, store);
        }
        public void AddProduct(Store store, string name, string description, string category, double price)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.AddProduct((LoggedInUser)User, store, name, description, category, price);
            }
            throw new UserHasNoPermissionException();
        }
        public void RemoveProduct(Store store, string name)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.RemoveProduct((LoggedInUser)User, store, store.GetProduct(name));
            }
            throw new UserHasNoPermissionException();
        }
        public void AddStoreOwner(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newOwner = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreOwner((LoggedInUser)User, store, newOwner);
            }
            throw new UserHasNoPermissionException();
        }
        public void AddStoreManager(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newManager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreManager((LoggedInUser)User, store, newManager);
            }
            throw new UserHasNoPermissionException();
        }
        public void SetPermissionsOfManager(Store store, string username, string auth)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newManager = UserFacadeInstance.GetUser(username);
                Authorizations authorization;
                switch (auth)
                {
                    case "Products":
                        authorization = Authorizations.Products;
                        break;

                    case "Owner":
                        authorization = Authorizations.Owner;
                        break;

                    case "Manager":
                        authorization = Authorizations.Manager;
                        break;

                    case "Authorizing":
                        authorization = Authorizations.Authorizing;
                        break;

                    case "Replying":
                        authorization = Authorizations.Replying;
                        break;

                    case "Watching":
                        authorization = Authorizations.Watching;
                        break;
                    
                    default:
                        throw new AuthorizationDoesNotExistException();
                }
                ManageFacadeInstance.SetPermissionsOfManager((LoggedInUser)User, store, newManager, authorization);
            }
            throw new UserHasNoPermissionException();
        }
        public void RemoveStoreManager(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser manager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.RemoveStoreManager((LoggedInUser)User, store, manager);
            }
            throw new UserHasNoPermissionException();
        }
        public IEnumerable<Message> ViewMessage(Store store)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.ViewMessage((LoggedInUser)User, store);
            }
            throw new UserHasNoPermissionException();
        }
        public void MessageReply(Message message, Store store, string description)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.MessageReply((LoggedInUser)User, message, store, description);
            }
            throw new UserHasNoPermissionException();
        }
        public IEnumerable<Purchase> ViewPurchaseHistory(Store store)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.ViewPurchaseHistory((LoggedInUser)User, store);
            }
            throw new UserHasNoPermissionException();
        }
    }
}
