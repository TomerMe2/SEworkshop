using System;
using System.Collections.Generic;
using SEWorkshop.Facades;
using SEWorkshop.Exceptions;
using NLog;
using System.Linq;
using SEWorkshop.Adapters;
using SEWorkshop.TyposFix;
using SEWorkshop.DataModels;
using SEWorkshop.Enums;

namespace SEWorkshop.ServiceLayer
{
    public class UserManager : IUserManager
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private const string EVENT_LOG_NM = "event_log.txt";
        private const string ERROR_LOG_NM = "error_log.txt";

        private IFacadesBridge FacadesBridge { get; }
        private ITyposFixerProxy TyposFixerNames { get; }
        private ITyposFixerProxy TyposFixerCategories { get; }
        private ITyposFixerProxy TyposFixerKeywords { get; }
        private ISecurityAdapter SecurityAdapter { get; }
        private IDictionary<string, DataUser> UsersDict { get; }
        private object UserDictLock { get; }
        private ICollection<IServiceObserver<DataMessage>> MsgObservers { get; set; }

        public UserManager()
        {
            ConfigLog();
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
            SecurityAdapter = new SecurityAdapter();
            FacadesBridge = new FacadesBridge();
            UsersDict = new Dictionary<string, DataUser>();
            UserDictLock = new object();
            MsgObservers = new List<IServiceObserver<DataMessage>>();
        }

        public UserManager(IFacadesBridge facadesBridge)
        {
            ConfigLog();
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
            FacadesBridge = facadesBridge;
            SecurityAdapter = new SecurityAdapter();
            UsersDict = new Dictionary<string, DataUser>();
            UserDictLock = new object();
            MsgObservers = new List<IServiceObserver<DataMessage>>();
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

        private void NotifyMsgObservers(DataMessage msg)
        {
            foreach(var obs in MsgObservers)
            {
                obs.Notify(msg);
            }
        }

        public DataUser GetUser(string sessionId)
        {
            lock(UserDictLock)
            {
                if (UsersDict.ContainsKey(sessionId))
                {
                    return UsersDict[sessionId];
                }
                var usr = FacadesBridge.CreateGuest();
                UsersDict[sessionId] = usr;
                return usr;
            }
        }

        public DataLoggedInUser GetLoggedInUser(string sessionId)
        {
            var usr = GetUser(sessionId);
            if (usr is DataLoggedInUser)
            {
                return (DataLoggedInUser)usr;
            }
            throw new UserHasNoPermissionException();
        }

        private DataAdministrator GetAdmin(string sessionId)
        {
            var usr = GetUser(sessionId);
            if (usr is DataAdministrator)
            {
                return (DataAdministrator)usr;
            }
            throw new UserHasNoPermissionException();
        }

        public void AddProductToCart(string sessionId, string storeName, string productName, int quantity)
        {
            FacadesBridge.AddProductToCart(GetUser(sessionId), storeName, productName, quantity);
        }

        public IEnumerable<DataStore> BrowseStores()
        {
            Log.Info("BrowseStores was invoked");
            return FacadesBridge.BrowseStores();
        }

        public DataStore SearchStore(string storeName)
        {
            Log.Info("BrowseStores was invoked");
            return FacadesBridge.SearchStore(storeName);
        }

        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            Log.Info("FilterProducts was invoked");
            return FacadesBridge.FilterProducts(products, pred);
        }

        public void Login(string sessionId, string username, string password)
        {
            Log.Info(string.Format("Login was invoked with username {0}", username));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new UsernameOrPasswordAreEmpty();
            }
            DataGuestUser guest = GetUser(sessionId) switch
            {
                DataGuestUser gst => gst,
                _ => throw new UserAlreadyLoggedInException(),
            };
            var usr = FacadesBridge.GetLoggedInUserAndApplyCart(username, SecurityAdapter.Encrypt(password), guest);
            UsersDict[sessionId] = usr;
        }

        public void Logout(string sessionId)
        {
            Log.Info("Logout was invoked");
            var user = GetUser(sessionId);
            if (user is DataGuestUser)
            {
                throw new UserHasNoPermissionException();
            }
            UsersDict[sessionId] = FacadesBridge.CreateGuest();
        }

        public IEnumerable<DataBasket> MyCart(string sessionId)
        {
            Log.Info("MyCart was invoked");
            return FacadesBridge.MyCart(GetUser(sessionId));
        }

        public void OpenStore(string sessionId, string storeName)
        {
            Log.Info(string.Format("OpenStore was invoked with storeName {0}", storeName));
            FacadesBridge.OpenStore(GetLoggedInUser(sessionId), storeName);
        }

        public void Purchase(string sessionId, DataBasket basket, string creditCardNumber, Address address)
        {
            Log.Info(string.Format("Purchase was invoked"));
            FacadesBridge.Purchase(GetUser(sessionId), basket, creditCardNumber, address);
        }

        public void Register(string sessionId, string username, string password)
        {
            Log.Info(string.Format("Register was invoked with username {0}", username));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new UsernameOrPasswordAreEmpty();
            }
            var user = GetUser(sessionId);
            if (!(user is DataGuestUser))
            {
                throw new UserAlreadyLoggedInException();
            }
            FacadesBridge.Register(username, SecurityAdapter.Encrypt(password));
        }

        public void RemoveProductFromCart(string sessionId, string storeName, string productName, int quantity)
        {
            FacadesBridge.RemoveProductFromCart(GetUser(sessionId), storeName, productName, quantity);
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

        public IEnumerable<DataPurchase> PurchaseHistory(string sessionId)
        {
            Log.Info("PurchaseHistory was invoked");
            return FacadesBridge.PurchaseHistory(GetLoggedInUser(sessionId));
        }

        public void WriteReview(string sessionId, string storeName, string productName, string description)
        {
            Log.Info(string.Format("WriteReview was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            FacadesBridge.WriteReview(GetLoggedInUser(sessionId), storeName, productName, description);
        }

        public int WriteMessage(string sessionId, string storeName, string description)
        {
            Log.Info(string.Format("WriteMessage was invoked with storeName {0}, description {1}",
                storeName, description));
            var msg = FacadesBridge.WriteMessage(GetLoggedInUser(sessionId), storeName, description);
            NotifyMsgObservers(msg);
            return msg.Id;
        }

        public IEnumerable<DataPurchase> UserPurchaseHistory(string sessionId, string userNm)
        {
            Log.Info(string.Format("WriteReview was invoked with userName {0}", userNm));
            return FacadesBridge.UserPurchaseHistory(GetLoggedInUser(sessionId), userNm);
        }

        public IEnumerable<DataPurchase> StorePurchaseHistory(string sessionId, string storeName)
        {
            Log.Info(string.Format("StorePurchaseHistory was invoked with storeName {0}", storeName));
            return FacadesBridge.StorePurchaseHistory(GetLoggedInUser(sessionId), storeName);
        }

        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string sessionId, string storeName)
        {
            Log.Info(string.Format("ManagingPurchaseHistory was invoked with storeName {0}", storeName));
            return FacadesBridge.ManagingPurchaseHistory(GetLoggedInUser(sessionId), storeName);
        }

        public DataProduct AddProduct(string sessionId, string storeName, string productName, string description,
                                        string category, double price, int quantity)
        {
            Log.Info(string.Format("AddProduct was invoked with storeName {0}, productName {1}, description {2}," +
                " category {3}, price {4}, quantity{5}", storeName, productName, description, category, price, quantity));
            var prod = FacadesBridge.AddProduct(GetLoggedInUser(sessionId), storeName, productName, description, category, price, quantity);
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

        public void RemoveProduct(string sessionId, string storeName, string productName)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}", storeName, productName));
            FacadesBridge.RemoveProduct(GetLoggedInUser(sessionId), storeName, productName);
            // we don't need to remove the product's description cus there are lots of produts with possibly similar descriptions
            // same applies for category
            TyposFixerNames.RemoveFromDictionary(productName);

        }

        public void EditProductDescription(string sessionId, string storeName, string productName, string description)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            FacadesBridge.EditProductDescription(GetLoggedInUser(sessionId), storeName, productName, description);
        }

        public void EditProductPrice(string sessionId, string storeName, string productName, double price)
        {
            Log.Info(string.Format("EditProductPrice was invoked with storeName {0}, productName {1}, price {2}",
                storeName, productName, price));
            FacadesBridge.EditProductPrice(GetLoggedInUser(sessionId), storeName, productName, price);
        }

        public void EditProductCategory(string sessionId, string storeName, string productName, string category)
        {
            Log.Info(string.Format("EditProductCategory was invoked with storeName {0}, productName {1}, category {2}",
                storeName, productName, category));
            FacadesBridge.EditProductCategory(GetLoggedInUser(sessionId), storeName, productName, category);
        }

        public void EditProductName(string sessionId, string storeName, string productName, string name)
        {
            Log.Info(string.Format("EditProductName was invoked with storeName {0}, productName {1}, name {2}",
                storeName, productName, name));
            FacadesBridge.EditProductName(GetLoggedInUser(sessionId), storeName, productName, name);
        }

        public void EditProductQuantity(string sessionId, string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("EditProductQuantity was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            FacadesBridge.EditProductQuantity(GetLoggedInUser(sessionId), storeName, productName, quantity);
        }

        public void AddStoreOwner(string sessionId, string storeName, string username)
        {
            FacadesBridge.AddStoreOwner(GetLoggedInUser(sessionId), storeName, username);
        }

        public void AddStoreManager(string sessionId, string storeName, string username)
        {
            Log.Info(string.Format("AddStoreManager was invoked with storeName {0}, username {1}",
                storeName, username));
            FacadesBridge.AddStoreManager(GetLoggedInUser(sessionId), storeName, username);
        }

        public void SetPermissionsOfManager(string sessionId, string storeName, string username, string auth)
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
            FacadesBridge.SetPermissionsOfManager(GetLoggedInUser(sessionId), storeName, username, authorization);
          
        }

        public void RemovePermissionsOfManager(string sessionId, string storeName, string username, string auth)
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
            FacadesBridge.RemovePermissionsOfManager(GetLoggedInUser(sessionId), storeName, username, authorization);
          
        }

        public void RemoveStoreManager(string sessionId, string storeName, string username)
        {
            Log.Info(string.Format("RemoveStoreManager was invoked with storeName {0}, username {1}",
                storeName, username));
            FacadesBridge.RemoveStoreManager(GetLoggedInUser(sessionId), storeName, username);
        }

        public IEnumerable<DataMessage> ViewMessage(string sessionId, string storeName)
        {
            Log.Info(string.Format("ViewMessage was invoked with storeName {0}", storeName));
            return FacadesBridge.ViewMessage(GetLoggedInUser(sessionId), storeName);
        }

        public DataMessage MessageReply(string sessionId, DataMessage message, string storeName, string description)
        {
            Log.Info(string.Format("MessageReply was invoked with storeName {0}", storeName));
            var msg = FacadesBridge.MessageReply(GetLoggedInUser(sessionId), message, storeName, description);
            NotifyMsgObservers(msg);
            return msg;
        }

        public bool IsLoggedIn(string sessionId)
        {
            try
            {
                GetLoggedInUser(sessionId);
                return true;
            }
            catch(UserHasNoPermissionException)
            {
                return false;
            }
        }

        public bool IsAdministrator(string sessionId)
        {
            try
            {
                GetAdmin(sessionId);
                return true;
            }
            catch (UserHasNoPermissionException)
            {
                return false;
            }
        }

        public void AddAlwaysTruePolicy(string sessionId, string storeName, Operator op)
        {
            FacadesBridge.AddAlwaysTruePolicy(GetLoggedInUser(sessionId), storeName, op);
        }

        public void AddSingleProductQuantityPolicy(string sessionId, string storeName, Operator op, string productName, int minQuantity, int maxQuantity)
        {
            FacadesBridge.AddSingleProductQuantityPolicy(GetLoggedInUser(sessionId), storeName, op, productName, minQuantity, maxQuantity);
        }

        public void AddSystemDayPolicy(string sessionId, string storeName, Operator op, DayOfWeek cantBuyIn)
        {
            FacadesBridge.AddSystemDayPolicy(GetLoggedInUser(sessionId), storeName, op, cantBuyIn);

        }

        public void AddUserCityPolicy(string sessionId, string storeName, Operator op, string requiredCity)
        {
            FacadesBridge.AddUserCityPolicy(GetLoggedInUser(sessionId), storeName, op, requiredCity);
        }

        public void AddUserCountryPolicy(string sessionId, string storeName, Operator op, string requiredCountry)
        {
            FacadesBridge.AddUserCountryPolicy(GetLoggedInUser(sessionId), storeName, op, requiredCountry);
        }

        public void AddWholeStoreQuantityPolicy(string sessionId, string storeName, Operator op, int minQuantity, int maxQuantity)
        {
            FacadesBridge.AddWholeStoreQuantityPolicy(GetLoggedInUser(sessionId), storeName, op, minQuantity, maxQuantity);
        }

        public void RemovePolicy(string sessionId, string storeName, int indexInChain)
        {
            FacadesBridge.RemovePolicy(GetLoggedInUser(sessionId), storeName, indexInChain);
        }

        public DataLoggedInUser GetDataLoggedInUser(string sessionId)
        {
            var usr = GetUser(sessionId);
            if (usr is DataLoggedInUser)
            {
                return (DataLoggedInUser)usr;
            }
            throw new UserIsNotLoggedInException();
        }

        public void RegisterMessageObserver(IServiceObserver<DataMessage> obsrv)
        {
            MsgObservers.Add(obsrv);
        }

        public void MarkAllDiscussionAsRead(string sessionId, string storeName, DataMessage msg)
        {
            FacadesBridge.MarkAllDiscussionAsRead(GetLoggedInUser(sessionId), storeName, msg);
        }

        public void AddProductCategoryDiscount(string sessionId, string storeName, string categoryName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain)
        {
            FacadesBridge.AddProductCategoryDiscount(GetLoggedInUser(sessionId), storeName, categoryName, deadline, percentage, op, indexInChain);
        }

        public void AddSpecificProductDiscount(string sessionId, string storeName, string productName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain)
        {
            FacadesBridge.AddSpecificProductDiscount(GetLoggedInUser(sessionId), storeName, productName, deadline, percentage, op, indexInChain);
        }

        public void RemoveDiscount(string sessionId, string storeName, int indexInChain)
        {
            FacadesBridge.RemoveDiscount(GetLoggedInUser(sessionId), storeName, indexInChain);
        }
    }
}
