using System;
using System.Collections.Generic;
using SEWorkshop.Facades;
using SEWorkshop.Exceptions;
using NLog;
using System.Linq;
using SEWorkshop.Adapters;
using SEWorkshop.TyposFix;
using SEWorkshop.DataModels;

namespace SEWorkshop.ServiceLayer
{
    public class UserManager : IUserManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private const string EVENT_LOG_NM = "event_log.txt";
        private const string ERROR_LOG_NM = "error_log.txt";

        private IFacadesBridge FacadesBridge { get; }
        private ITyposFixerProxy TyposFixerNames { get; }
        private ITyposFixerProxy TyposFixerCategories { get; }
        private ITyposFixerProxy TyposFixerKeywords { get; }
        private ISecurityAdapter SecurityAdapter { get; }

        public UserManager()
        {
            ConfigLog();
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
            SecurityAdapter = new SecurityAdapter();
            FacadesBridge = new FacadesBridge();
        }

        public UserManager(IFacadesBridge facadesBridge)
        {
            ConfigLog();
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
            FacadesBridge = facadesBridge;
            SecurityAdapter = new SecurityAdapter();
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

        public void AddProductToCart(string storeName, string productName, int quantity)
        {
            FacadesBridge.AddProductToCart(storeName, productName, quantity);
        }

        public IEnumerable<DataStore> BrowseStores()
        {
            Log.Info("BrowseStores was invoked");
            return FacadesBridge.BrowseStores();
        }

        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            Log.Info("FilterProducts was invoked");
            return FacadesBridge.FilterProducts(products, pred);
        }

        public void Login(string username, string password)
        {
            Log.Info(string.Format("Login was invoked with username {0}", username));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("username or password are empty");
            }
            FacadesBridge.Login(username, SecurityAdapter.Encrypt(password));
        }

        public void Logout()
        {
            Log.Info("Logout was invoked");
            FacadesBridge.Logout();
        }

        public IEnumerable<DataBasket> MyCart()
        {
            Log.Info("MyCart was invoked");
            return FacadesBridge.MyCart();
        }

        public void OpenStore(string storeName)
        {
            Log.Info(string.Format("OpenStore was invoked with storeName {0}", storeName));
            FacadesBridge.OpenStore(storeName);
        }

        public void Purchase(DataBasket basket)
        {
            Log.Info(string.Format("Purchase was invoked"));
            FacadesBridge.Purchase(basket);
        }

        public void Register(string username, string password)
        {
            Log.Info(string.Format("Register was invoked with username {0}", username));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("username or password are empty");
            }
            FacadesBridge.Register(username, SecurityAdapter.Encrypt(password));
        }

        public void RemoveProductFromCart(string storeName, string productName, int quantity)
        {
            FacadesBridge.RemoveProductFromCart(storeName, productName, quantity);
        }

        public IEnumerable<DataProduct> SearchProductsByName(ref string input)
        {
            Log.Info("SearchProductsByName was invoked with input {0}", input);
            string localInput = input;
            IEnumerable<DataProduct> products = FacadesBridge.SearchProductsByName(localInput);
            if (products.Any())
                return products;
            // the typo fixer returns '_' instead of ' ', so it will fix it
            string corrected = TyposFixerNames.Correct(localInput.Replace(' ', '_')).Replace('_', ' ');
            products = FacadesBridge.SearchProductsByName(corrected);
            input = corrected;   
            return products;
        }

        public IEnumerable<DataProduct> SearchProductsByCategory(ref string input)
        {
            Log.Info("SearchProductsByCategory was invoked with input {0}", input);
            string localInput = input;
            IEnumerable<DataProduct> products = FacadesBridge.SearchProductsByCategory(localInput);
            if (products.Any())
                return products;
            string corrected = TyposFixerNames.Correct(localInput.Replace(' ', '_')).Replace('_', ' ');
            products = FacadesBridge.SearchProductsByCategory(corrected);
            input = corrected;   // the typo fixer returns '_' instead of ' ', so it will fix it
            return products;
        }
        
        public IEnumerable<DataProduct> SearchProductsByKeywords(ref string input)
        {
            Log.Info("SearchProductsByKeywords was invoked with input {0}", input);
            string localInput = input;
            var products = FacadesBridge.SearchProductsByKeywords(localInput);
            if (products.Any())
                return products;
            // Each word should be corrected seperatly because the words do not have to depend on each other
            List<string> corrected = input.Split(' ').Select(word => TyposFixerKeywords.Correct(word)).ToList();
            string correctedStr = string.Join(' ', corrected);
            input = correctedStr;
            return FacadesBridge.SearchProductsByKeywords(correctedStr);
        }

        public IEnumerable<DataPurchase> PurchaseHistory()
        {
            Log.Info("PurchaseHistory was invoked");
            return FacadesBridge.PurchaseHistory();
        }

        public void WriteReview(string storeName, string productName, string description)
        {
            Log.Info(string.Format("WriteReview was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            FacadesBridge.WriteReview(storeName, productName, description);
        }

        public void WriteMessage(string storeName, string description)
        {
            Log.Info(string.Format("WriteMessage was invoked with storeName {0}, description {1}",
                storeName, description));
            FacadesBridge.WriteMessage(storeName, description);
        }

        public IEnumerable<DataPurchase> UserPurchaseHistory(string userNm)
        {
            Log.Info(string.Format("WriteReview was invoked with userName {0}", userNm));
            return FacadesBridge.UserPurchaseHistory(userNm);
        }

        public IEnumerable<DataPurchase> StorePurchaseHistory(string storeName)
        {
            Log.Info(string.Format("StorePurchaseHistory was invoked with storeName {0}", storeName));
            return FacadesBridge.StorePurchaseHistory(storeName);
        }

        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeName)
        {
            Log.Info(string.Format("ManagingPurchaseHistory was invoked with storeName {0}", storeName));
            return FacadesBridge.ManagingPurchaseHistory(storeName);
        }

        public DataProduct AddProduct(string storeName, string productName, string description, string category, double price, int quantity)
        {
            Log.Info(string.Format("AddProduct was invoked with storeName {0}, productName {1}, description {2}," +
                " category {3}, price {4}, quantity{5}", storeName, productName, description, category, price, quantity));
            var prod = FacadesBridge.AddProduct(storeName, productName, description, category, price, quantity);
            //replacing spaces with _, so different words will be related to one product name in the typos fixer algorithm
            TyposFixerNames.AddToDictionary(productName);
            TyposFixerCategories.AddToDictionary(category);
            // for keywods, we are treating each word in an un-connected way, because each word is a keyword
            foreach (string word in productName.Split(' '))
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
            return prod;
        }

        public void RemoveProduct(string storeName, string productName)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}", storeName, productName));
            FacadesBridge.RemoveProduct(storeName, productName);
            // we don't need to remove the product's description cus there are lots of produts with possibly similar descriptions
            // same applies for category
            TyposFixerNames.RemoveFromDictionary(productName);

        }

        public void EditProductDescription(string storeName, string productName, string description)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            FacadesBridge.EditProductDescription(storeName, productName, description);
        }

        public void EditProductPrice(string storeName, string productName, double price)
        {
            Log.Info(string.Format("EditProductPrice was invoked with storeName {0}, productName {1}, price {2}",
                storeName, productName, price));
            FacadesBridge.EditProductPrice(storeName, productName, price);
        }

        public void EditProductCategory(string storeName, string productName, string category)
        {
            Log.Info(string.Format("EditProductCategory was invoked with storeName {0}, productName {1}, category {2}",
                storeName, productName, category));
            FacadesBridge.EditProductCategory(storeName, productName, category);
        }

        public void EditProductName(string storeName, string productName, string name)
        {
            Log.Info(string.Format("EditProductName was invoked with storeName {0}, productName {1}, name {2}",
                storeName, productName, name));
            FacadesBridge.EditProductName(storeName, productName, name);
        }

        public void EditProductQuantity(string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("EditProductQuantity was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            FacadesBridge.EditProductQuantity(storeName, productName, quantity);
        }

        public void AddStoreOwner(string storeName, string username)
        {
            FacadesBridge.AddStoreOwner(storeName, username);
        }

        public void AddStoreManager(string storeName, string username)
        {
            Log.Info(string.Format("AddStoreManager was invoked with storeName {0}, username {1}",
                storeName, username));
            FacadesBridge.AddStoreManager(storeName, username);
        }

        public void SetPermissionsOfManager(string storeName, string username, string auth)
        {
            Log.Info(string.Format("SetPermissionsOfManager was invoked with storeName {0}, username {1}, auth {2}",
                storeName, username, auth));
            var authorization = auth switch
            {
                "Products" => Authorizations.Products,
                "Owner" => Authorizations.Owner,
                "Manager" => Authorizations.Manager,
                "Authorizing" => Authorizations.Authorizing,
                "Replying" => Authorizations.Replying,
                "Watching" => Authorizations.Watching,
                _ => throw new AuthorizationDoesNotExistException(),
            };
            FacadesBridge.SetPermissionsOfManager(storeName, username, authorization);
          
        }

        public void RemoveStoreManager(string storeName, string username)
        {
            Log.Info(string.Format("RemoveStoreManager was invoked with storeName {0}, username {1}",
                storeName, username));
            FacadesBridge.RemoveStoreManager(storeName, username);
        }

        public IEnumerable<DataMessage> ViewMessage(string storeName)
        {
            Log.Info(string.Format("ViewMessage was invoked with storeName {0}", storeName));
            return FacadesBridge.ViewMessage(storeName);
        }

        public DataMessage MessageReply(DataMessage message, string storeName, string description)
        {
            Log.Info(string.Format("MessageReply was invoked with storeName {0}", storeName));
            return FacadesBridge.MessageReply(message, storeName, description);
        }
    }
}
