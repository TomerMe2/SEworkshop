using System;
using System.Collections.Generic;
using SEWorkshop.Facades;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using NLog;
using System.Linq;
using SEWorkshop.Adapters;
using SEWorkshop.TyposFix;

namespace SEWorkshop.ServiceLayer
{
    public class UserManager : IUserManager
    {
        User currUser = new GuestUser();
        readonly StoreFacade StoreFacadeInstance = StoreFacade.GetInstance();
        ManageFacade ManageFacadeInstance = ManageFacade.GetInstance();
        UserFacade UserFacadeInstance = UserFacade.GetInstance();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();
        private ITyposFixerProxy TyposFixerNames { get; set; }
        private ITyposFixerProxy TyposFixerCategories { get; set; }
        private ITyposFixerProxy TyposFixerKeywords { get; set; }
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public UserManager()
        {
            //List<string> dictionary = AllActiveProducts().Select(product => product.Name.Replace(' ', '_')).ToList();
            //replacing spaces with _, so different words will be related one product name in the typos fixer algorithm
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
        }

        public void AddProductToCart(Product product)
        {
            UserFacadeInstance.AddProductToCart(currUser, product);
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
            //preserve loggedIn user's cart that he gathered as a GuestUser.
            Cart cart = currUser.Cart;
            currUser = UserFacadeInstance.Login(username, securityAdapter.Encrypt(password));
            currUser.Cart = cart;
        }

        public void Logout()
        {
            Cart cart = currUser.Cart;
            UserFacadeInstance.Logout();
            currUser = new GuestUser();
            currUser.Cart = cart;
        }

        public IEnumerable<Basket> MyCart()
        {
            return UserFacadeInstance.MyCart(currUser);
        }

        public void OpenStore(LoggedInUser owner, string storeName)
        {
            StoreFacadeInstance.CreateStore(owner, storeName);
        }

        public void Purchase(Basket basket)
        {
            UserFacadeInstance.Purchase(currUser, basket);
        }

        public void Register(string username, string password)
        {
            UserFacadeInstance.Register(username, securityAdapter.Encrypt(password));
        }

        public void RemoveProductFromCart(Product product)
        {
            UserFacadeInstance.RemoveProductFromCart(currUser, product);
        }

        private IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            return StoreFacadeInstance.SearchProducts(pred);
        }

        public IEnumerable<Product> SearchProductsByName(ref string input)
        {
            string localInput = input;
            IEnumerable<Product> products = SearchProducts(product => product.Name.Equals(localInput));
            if (products.Any())
                return products;
            string corrected = TyposFixerNames.Correct(input);
            products = SearchProducts(product => product.Name.ToLower().Replace(' ', '_').Equals(corrected));
            input = corrected;
            return products;
        }

        public IEnumerable<Product> SearchProductsByCategory(ref string input)
        {
            string localInput = input;
            IEnumerable<Product> products = SearchProducts(product => product.Category.Equals(localInput));
            if (products.Any())
                return products;
            string corrected = TyposFixerNames.Correct(input);
            products = SearchProducts(product => product.Category.ToLower().Replace(' ', '_').Equals(corrected));
            input = corrected;
            return products;
        }

        public IEnumerable<Product> SearchProductsByKeywords(ref string input)
        {
            string localInput = input;
            Func<Product, bool> predicate = product => product.Name.Equals(localInput) ||
                                            product.Category.Equals(localInput) ||
                                            product.Description.Split(' ').Contains(localInput);
            IEnumerable <Product> products = SearchProducts(predicate);
            if (products.Any())
                return products;
            string corrected = TyposFixerNames.Correct(input);
            Func<Product, bool> correctedPredicate = product => product.Name.ToLower().Replace(' ', '_').Equals(localInput) ||
                                            product.Category.ToLower().Replace(' ', '_').Equals(localInput) ||
                                            product.Description.ToLower().Replace(' ', '_').Split(' ').Contains(localInput);
            products = SearchProducts(correctedPredicate);
            input = corrected;
            return products;
        }

        public IEnumerable<Purchase> PurcahseHistory()
        {
            return UserFacadeInstance.PurcahseHistory(currUser);
        }

        public void WriteReview(Product product, string description)
        {
            UserFacadeInstance.WriteReview(currUser, product, description);
        }

        public void WriteMessage(Store store, string description)
        {
            UserFacadeInstance.WriteMessage(currUser, store, description);
        }

        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser user)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return UserFacadeInstance.UserPurchaseHistory((LoggedInUser)currUser, user);
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> StorePurchaseHistory(Store store)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return UserFacadeInstance.StorePurchaseHistory((LoggedInUser)currUser, store);
            }
            throw new UserHasNoPermissionException();
        }

        public void AddProduct(Store store, string name, string description, string category, double price)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.AddProduct((LoggedInUser)currUser, store, name, description, category, price);
                TyposFixerNames.AddToDictionary(name.Replace(' ', '_'));
                TyposFixerCategories.AddToDictionary(category.Replace(' ', '_'));
                TyposFixerKeywords.AddToDictionary(name.Replace(' ', '_'));
                TyposFixerKeywords.AddToDictionary(category.Replace(' ', '_'));
                foreach (var word in description.Split(' '))
                {
                    TyposFixerKeywords.AddToDictionary(word);
                }
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveProduct(Store store, string name)
        {
            if(UserFacadeInstance.HasPermission)
            {
                Product product = store.GetProduct(name);
                ManageFacadeInstance.RemoveProduct((LoggedInUser)currUser, store, product);
                //we don't need to remove the product's description there are lots of produts with possibly similar descriptions
                TyposFixerNames.RemoveFromDictionary(product.Name);
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreOwner(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newOwner = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreOwner((LoggedInUser)currUser, store, newOwner);
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreManager(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newManager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreManager((LoggedInUser)currUser, store, newManager);
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
                ManageFacadeInstance.SetPermissionsOfManager((LoggedInUser)currUser, store, newManager, authorization);
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveStoreManager(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser manager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.RemoveStoreManager((LoggedInUser)currUser, store, manager);
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Message> ViewMessage(Store store)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.ViewMessage((LoggedInUser)currUser, store);
            }
            throw new UserHasNoPermissionException();
        }

        public void MessageReply(Message message, Store store, string description)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.MessageReply((LoggedInUser)currUser, message, store, description);
            }
            throw new UserHasNoPermissionException();
        }
    }
}
