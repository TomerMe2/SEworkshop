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
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private const string EVENT_LOG_NM = "event_log.txt";
        private const string ERROR_LOG_NM = "error_log.txt";

        private User CurrUser { get; set; }
        private IStoreFacade StrFacade { get; } 
        private IManageFacade MngFacade { get; }
        private IUserFacade UsrFacade { get; }
        private ITyposFixerProxy TyposFixerNames { get; set; }
        private ITyposFixerProxy TyposFixerCategories { get; set; }
        private ITyposFixerProxy TyposFixerKeywords { get; set; }
        private ISecurityAdapter SecurityAdapter { get; }

        public UserManager()
        {
            ConfigLog();
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
            StrFacade = StoreFacade.GetInstance();
            MngFacade = ManageFacade.GetInstance();
            UsrFacade = UserFacade.GetInstance();
            SecurityAdapter = new SecurityAdapter();
            CurrUser = new GuestUser();
        }

        public UserManager(IStoreFacade storeFacade, IManageFacade manageFacade, IUserFacade userFacade)
        {
            ConfigLog();
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
            StrFacade = storeFacade;
            MngFacade = manageFacade;
            UsrFacade = userFacade;
            SecurityAdapter = new SecurityAdapter();
            CurrUser = new GuestUser();
        }

        private static void ConfigLog()
        {
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            var eventLogFile = new NLog.Targets.FileTarget(EVENT_LOG_NM) { FileName = EVENT_LOG_NM + ".txt" };
            var errorLogFile = new NLog.Targets.FileTarget(ERROR_LOG_NM) { FileName = ERROR_LOG_NM + ".txt" };
            // Rules for mapping loggers to targets    
            config.AddRule(LogLevel.Debug, LogLevel.Info, eventLogFile);
            config.AddRule(LogLevel.Error, LogLevel.Fatal, errorLogFile);

            
            // Apply config           
            LogManager.Configuration = config;
        }

        private Product GetProduct(string storeName, string productName)
        {
            Log.Info(string.Format("GetProduct was invoked with storeName {0} and productName {1}", storeName, productName));
            var product = StrFacade.SearchProducts(prod => prod.Store.Name.Equals(storeName) && prod.Name.Equals(productName))
                .FirstOrDefault();
            if (product is null)
            {
                Log.Info(string.Format("Someone searched for a non existing product with name {0} and store name {1}",
                    productName, storeName));
                throw new ProductNotInTheStoreException();
            }
            return product;
        }

        private Store GetStore(string storeName)
        {
            Log.Info(string.Format("GetStore was invoked with storeName {0}", storeName));
            var store = StrFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone tried to add product to cart with a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            return store;
        }

        public void AddProductToCart(string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("AddProductToCart was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            var store = StrFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone searched for a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            UsrFacade.AddProductToCart(CurrUser, GetProduct(storeName, productName), quantity);
        }

        public IEnumerable<Store> BrowseStores()
        {
            Log.Info("BrowseStores was invoked");
            return StrFacade.BrowseStores();
        }

        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            Log.Info("FilterProducts was invoked");
            return StrFacade.FilterProducts(products, pred);
        }

        public void Login(string username, string password)
        {
            Log.Info(string.Format("Login was invoked with username {0}", username));
            //preserve loggedIn user's cart that he gathered as a GuestUser.
            Cart cart = CurrUser.Cart;
            CurrUser = UsrFacade.Login(username, SecurityAdapter.Encrypt(password));
            CurrUser.Cart = cart;
        }

        public void Logout()
        {
            Log.Info("Logout was invoked");
            Cart cart = CurrUser.Cart;
            UsrFacade.Logout();
            CurrUser = new GuestUser();
        }

        public IEnumerable<Basket> MyCart()
        {
            Log.Info("MyCart was invoked");
            return UsrFacade.MyCart(CurrUser);
        }

        public void OpenStore(string storeName)
        {
            Log.Info(string.Format("OpenStore was invoked with storeName {0}", storeName));
            if (CurrUser is GuestUser)
            {
                Log.Info(string.Format("GuestUser invoked OpenStore"));
                throw new UserHasNoPermissionException();
            }
            StrFacade.CreateStore((LoggedInUser)CurrUser, storeName);
        }

        public void Purchase(Basket basket)
        {
            Log.Info(string.Format("Purchase was invoked"));
            UsrFacade.Purchase(CurrUser, basket);
        }

        public void Register(string username, string password)
        {
            Log.Info(string.Format("Register was invoked with username {0}", username));
            UsrFacade.Register(username, SecurityAdapter.Encrypt(password));
        }

        public void RemoveProductFromCart(string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("RemoveProductFromCart was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            var store = StrFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone tried to remove product from cart with a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            UsrFacade.RemoveProductFromCart(CurrUser, GetProduct(storeName, productName), quantity);
        }

        private IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            Log.Info("SearchProducts was invoked");
            return StrFacade.SearchProducts(pred);
        }

        public IEnumerable<Product> SearchProductsByName(ref string input)
        {
            Log.Info("SearchProductsByName was invoked with input {0}", input);
            string localInput = input;
            IEnumerable<Product> products = SearchProducts(product => product.Name.Equals(localInput));
            if (products.Any())
                return products;
            string corrected = TyposFixerNames.Correct(input);
            products = SearchProducts(product => product.Name.ToLower().Replace(' ', '_').Equals(corrected));
            input = corrected.Replace('_', ' ');   // the typo fixer returns '_' instead of ' ', so it will fix it
            return products;
        }

        public IEnumerable<Product> SearchProductsByCategory(ref string input)
        {
            Log.Info("SearchProductsByCategory was invoked with input {0}", input);
            string localInput = input;
            IEnumerable<Product> products = SearchProducts(product => product.Category.Equals(localInput));
            if (products.Any())
                return products;
            string corrected = TyposFixerCategories.Correct(input);
            Log.Info("Input correction has occured, changed {0} into {1}", input, corrected);
            products = SearchProducts(product => product.Category.ToLower().Replace(' ', '_').Equals(corrected));
            input = corrected.Replace('_', ' ');   // the typo fixer returns '_' instead of ' ', so it will fix it
            return products;
        }
        
        public IEnumerable<Product> SearchProductsByKeywords(ref string input)
        {
            Log.Info("SearchProductsByKeywords was invoked with input {0}", input);
            string localInput = input;
            bool HasWordInsideOther(string[] words1, List<string> words2)
            {
                foreach (string word1 in words1)
                {
                    foreach (string word2 in words2)
                    {
                        if (word1.ToLower().Equals(word2.ToLower()))
                        {
                            return true;
                        }
                    }
                }
                return false;
            };
            bool hasWordInsideInput(string[] words) => HasWordInsideOther(words, localInput.Split(' ').ToList());   // curry version
            bool predicate(Product product) => hasWordInsideInput(product.Name.Split(' ')) ||
                                            hasWordInsideInput(product.Category.Split(' ')) ||
                                            hasWordInsideInput(product.Description.Split(' '));
            IEnumerable <Product> products = SearchProducts(predicate);
            if (products.Any())
                return products;
            // Each word should be corrected seperatly because the words do not have to depend on each other
            List<string> corrected = input.Split(' ').Select(word => TyposFixerKeywords.Correct(word)).ToList();
            bool hasWordInsideCorrected(string[] words) => HasWordInsideOther(words, corrected);  // curry version
            bool correctedPredicate(Product product) => hasWordInsideCorrected(product.Name.Split(' ')) ||
                                            hasWordInsideCorrected(product.Category.Split(' ')) ||
                                            hasWordInsideCorrected(product.Description.Split(' '));
            products = SearchProducts(correctedPredicate);
            string correctedStr = string.Join(' ', corrected);
            Log.Info("Input correction has occured, changed {0} into {1}", input, correctedStr);
            input = correctedStr;
            return products;
        }

        public IEnumerable<Purchase> PurcahseHistory()
        {
            Log.Info("PurchaseHistory was invoked");
            return UsrFacade.PurcahseHistory(CurrUser);
        }

        public void WriteReview(string storeName, string productName, string description)
        {
            Log.Info(string.Format("WriteReview was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            UsrFacade.WriteReview(CurrUser, GetProduct(storeName, productName), description);
        }

        public void WriteMessage(string storeName, string description)
        {
            Log.Info(string.Format("WriteMessage was invoked with storeName {0}, description {1}",
                storeName, description));
            UsrFacade.WriteMessage(CurrUser, GetStore(storeName), description);
        }

        public IEnumerable<Purchase> UserPurchaseHistory(string userNm)
        {
            Log.Info(string.Format("WriteReview was invoked with userName {0}", userNm));
            if (UsrFacade.HasPermission)
            {
                return UsrFacade.UserPurchaseHistory((LoggedInUser)CurrUser, userNm);
            }
            Log.Info(string.Format("userName {0} has no permission", userNm));
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> StorePurchaseHistory(string storeName)
        {
            Log.Info(string.Format("StorePurchaseHistory was invoked with storeName {0}", storeName));
            if (UsrFacade.HasPermission)
            {
                return UsrFacade.StorePurchaseHistory((LoggedInUser)CurrUser, GetStore(storeName));
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> ManagingPurchaseHistory(string storeName)
        {
            Log.Info(string.Format("ManagingPurchaseHistory was invoked with storeName {0}", storeName));
            if (UsrFacade.HasPermission)
            {
                return MngFacade.ViewPurchaseHistory((LoggedInUser)CurrUser, GetStore(storeName));
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public Product AddProduct(string storeName, string productName, string description, string category, double price, int quantity)
        {
            Log.Info(string.Format("AddProduct was invoked with storeName {0}, productName {1}, description {2}," +
                " category {3}, price {4}, quantity{5}", storeName, productName, description, category, price, quantity));
            if (UsrFacade.HasPermission)
            {
                Product product = MngFacade.AddProduct((LoggedInUser)CurrUser, GetStore(storeName), productName, description, category, price, quantity);
                //replacing spaces with _, so different words will be related to one product name in the typos fixer algorithm
                TyposFixerNames.AddToDictionary(productName);
                TyposFixerCategories.AddToDictionary(category);
                // for keywods, we are treating each word in an un-connected way, because each word is a keyword
                foreach(string word in productName.Split(' '))
                {
                    TyposFixerKeywords.AddToDictionary(word);
                }
                foreach (string word in category.Split(' '))
                {
                    TyposFixerKeywords.AddToDictionary(word);
                }
                foreach (string word in description.Split(' '))
                {
                    TyposFixerKeywords.AddToDictionary(word);
                }
                return product;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void RemoveProduct(string storeName, string productName)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}", storeName, productName));
            if (UsrFacade.HasPermission)
            {
                Store store = GetStore(storeName);
                Product product = GetProduct(storeName, productName);
                MngFacade.RemoveProduct((LoggedInUser)CurrUser, store, product);
                // we don't need to remove the product's description cus there are lots of produts with possibly similar descriptions
                // same applies for category
                TyposFixerNames.RemoveFromDictionary(product.Name);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void EditProductDescription(string storeName, string productName, string description)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            if (UsrFacade.HasPermission)
             {
                ManageFacade.GetInstance().EditProductDescription((LoggedInUser)CurrUser, store, product, description);
                return;
             }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void EditProductPrice(string storeName, string productName, double price)
        {
            Log.Info(string.Format("EditProductPrice was invoked with storeName {0}, productName {1}, price {2}",
                storeName, productName, price));
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            if (UsrFacade.HasPermission)
            {            
                ManageFacade.GetInstance().EditProductPrice((LoggedInUser)CurrUser, store, product, price);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void EditProductCategory(string storeName, string productName, string category)
        {
            Log.Info(string.Format("EditProductCategory was invoked with storeName {0}, productName {1}, category {2}",
                storeName, productName, category));
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            if (UsrFacade.HasPermission)
            {
                ManageFacade.GetInstance().EditProductCategory((LoggedInUser)CurrUser, store, product, category);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void EditProductName(string storeName, string productName, string name)
        {
            Log.Info(string.Format("EditProductName was invoked with storeName {0}, productName {1}, name {2}",
                storeName, productName, name));
            if (UsrFacade.HasPermission)
            {
                ManageFacade.GetInstance().EditProductName((LoggedInUser)CurrUser, GetStore(storeName),
                    GetProduct(storeName, productName), name);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void EditProductQuantity(string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("EditProductQuantity was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            if (UsrFacade.HasPermission)
            {
                ManageFacade.GetInstance().EditProductQuantity((LoggedInUser)CurrUser, GetStore(storeName),
                    GetProduct(storeName, productName), quantity);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void AddStoreOwner(string storeName, string username)
        {
            Log.Info(string.Format("AddStoreOwner was invoked with storeName {0}, username {1}",
                storeName, username));
            if (UsrFacade.HasPermission)
            {
                LoggedInUser newOwner = UsrFacade.GetUser(username);
                MngFacade.AddStoreOwner((LoggedInUser)CurrUser, GetStore(storeName), newOwner);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void AddStoreManager(string storeName, string username)
        {
            Log.Info(string.Format("AddStoreManager was invoked with storeName {0}, username {1}",
                storeName, username));
            if (UsrFacade.HasPermission)
            {
                LoggedInUser newManager = UsrFacade.GetUser(username);
                MngFacade.AddStoreManager((LoggedInUser)CurrUser, GetStore(storeName), newManager);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void SetPermissionsOfManager(string storeName, string username, string auth)
        {
            Log.Info(string.Format("SetPermissionsOfManager was invoked with storeName {0}, username {1}, auth {2}",
                storeName, username, auth));
            if (UsrFacade.HasPermission)
            {
                LoggedInUser newManager = UsrFacade.GetUser(username);
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
                MngFacade.SetPermissionsOfManager((LoggedInUser)CurrUser, GetStore(storeName), newManager, authorization);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public void RemoveStoreManager(string storeName, string username)
        {
            Log.Info(string.Format("RemoveStoreManager was invoked with storeName {0}, username {1}",
                storeName, username));
            if (UsrFacade.HasPermission)
            {
                LoggedInUser manager = UsrFacade.GetUser(username);
                MngFacade.RemoveStoreManager((LoggedInUser)CurrUser, GetStore(storeName), manager);
                return;
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Message> ViewMessage(string storeName)
        {
            Log.Info(string.Format("ViewMessage was invoked with storeName {0}", storeName));
            if (UsrFacade.HasPermission)
            {
                return MngFacade.ViewMessage((LoggedInUser)CurrUser, GetStore(storeName));
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }

        public Message MessageReply(Message message, string storeName, string description)
        {
            Log.Info(string.Format("MessageReply was invoked with storeName {0}", storeName));
            if (UsrFacade.HasPermission)
            {
                return MngFacade.MessageReply((LoggedInUser)CurrUser, message, GetStore(storeName), description);
            }
            Log.Info("user has no permission");
            throw new UserHasNoPermissionException();
        }
    }
}
