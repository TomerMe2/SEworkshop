using NLog;
using SEWorkshop.Adapters;
using SEWorkshop.DataModels;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;
using SEWorkshop.TyposFix;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private ICollection<IServiceObserver<DataMessage>> MsgObservers { get; }
        private ICollection<IServiceObserver<DataPurchase>> PurchaseObservers { get; }
        private ICollection<IServiceObserver<DataOwnershipRequest>> OwnershipRequestObservers { get; }

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
            PurchaseObservers = new List<IServiceObserver<DataPurchase>>();
            OwnershipRequestObservers = new List<IServiceObserver<DataOwnershipRequest>>();
            ExecuteActionFromFile executeActionFromFile = new ExecuteActionFromFile(this);
            executeActionFromFile.ReadAndExecute();
        }

        [Obsolete]
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
            PurchaseObservers = new List<IServiceObserver<DataPurchase>>();
            OwnershipRequestObservers = new List<IServiceObserver<DataOwnershipRequest>>();
            ExecuteActionFromFile executeActionFromFile = new ExecuteActionFromFile(this);
            executeActionFromFile.ReadAndExecute();
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

        private void NotifyPrchsObservers(DataPurchase prchs)
        {
            foreach(var obs in PurchaseObservers)
            {
                obs.Notify(prchs);
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

        private DataLoggedInUser GetLoggedInUser(string sessionId)
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
            Log.Info(string.Format("AddProductToCart    {0}    {1}    {2}", storeName, productName, quantity));
            try
            {
                FacadesBridge.AddProductToCart(GetUser(sessionId), storeName, productName, quantity);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("AddProductToCart    {0}", e.ToString()));
                throw e;
            }
        }

        public IEnumerable<DataStore> BrowseStores()
        {
            Log.Info("BrowseStores");
            return FacadesBridge.BrowseStores();
        }

        public DataStore SearchStore(string storeName)
        {
            Log.Info("SearchStore");
            return FacadesBridge.SearchStore(storeName);
        }

        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            Log.Info("FilterProducts");
            try
            {
                return FacadesBridge.FilterProducts(products, pred);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("FilterProducts    {0}", e.ToString()));
                throw e;
            }
        }

        public void Login(string sessionId, string username, string password)
        {
            Log.Info(string.Format("Login    {0}", username));
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
            Log.Info("Logout");
            var user = GetUser(sessionId);
            if (user is DataGuestUser)
            {
                throw new UserHasNoPermissionException();
            }
            UsersDict[sessionId] = FacadesBridge.CreateGuest();
        }

        public IEnumerable<DataBasket> MyCart(string sessionId)
        {
            Log.Info("MyCart");
            return FacadesBridge.MyCart(GetUser(sessionId));
        }

        public void OpenStore(string sessionId, string storeName)
        {
            Log.Info(string.Format("OpenStore    {0}", storeName));
            try
            {
                FacadesBridge.OpenStore(GetLoggedInUser(sessionId), storeName);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("OpenStore    {0}", e.ToString()));
                throw e;
            }
        }

        public void Purchase(string sessionId, DataBasket basket, string creditCardNumber, Address address)
        {
            Log.Info(string.Format("Purchase"));
            var prchs = FacadesBridge.Purchase(GetUser(sessionId), basket, creditCardNumber, address);
            NotifyPrchsObservers(prchs);
        }

        public void Register(string sessionId, string username, string password)
        {
            Log.Info(string.Format("Register    {0}", username));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new UsernameOrPasswordAreEmpty();
            }
            if(username.Contains(" ") || password.Contains(" "))
            {
                throw new IllegalCharactersException();
            }
            var user = GetUser(sessionId);
            if (!(user is DataGuestUser))
            {
                throw new UserAlreadyLoggedInException();
            }
            try
            {
                FacadesBridge.Register(username, SecurityAdapter.Encrypt(password));
            }
            catch (Exception e)
            {
                Log.Info(string.Format("Register    {0}", e.ToString()));
                throw e;
            }
        }

        public void RemoveProductFromCart(string sessionId, string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("RemoveProductFromCart    {0}    {1}    {2}", storeName, productName, quantity));
            try
            {
                FacadesBridge.RemoveProductFromCart(GetUser(sessionId), storeName, productName, quantity);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("RemoveProductFromCart    {0}", e.ToString()));
                throw e;
            }
        }

        public IEnumerable<DataProduct> SearchProductsByName(ref string input)
        {
            Log.Info("SearchProductsByName    {0}", input);
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
            Log.Info("SearchProductsByCategory    {0}", input);
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
            Log.Info("SearchProductsByKeywords    {0}", input);
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
            Log.Info("PurchaseHistory");
            return FacadesBridge.PurchaseHistory(GetLoggedInUser(sessionId));
        }

        public void WriteReview(string sessionId, string storeName, string productName, string description)
        {
            Log.Info(string.Format("WriteReview    {0}    {1}    {2}", storeName, productName, description));
            try
            {
                FacadesBridge.WriteReview(GetLoggedInUser(sessionId), storeName, productName, description);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("WriteReview    {0}", e.ToString()));
                throw e;
            }
        }

        public int WriteMessage(string sessionId, string storeName, string description)
        {
            Log.Info(string.Format("WriteMessage    {0}    {1}", storeName, description));
            try
            {
                var msg = FacadesBridge.WriteMessage(GetLoggedInUser(sessionId), storeName, description);
                NotifyMsgObservers(msg);
                return msg.Id;
            }
            catch (Exception e)
            {
                Log.Info(string.Format("WriteMessage    {0}", e.ToString()));
                throw e;
            }
        }

        public IEnumerable<DataPurchase> UserPurchaseHistory(string sessionId, string userNm)
        {
            Log.Info(string.Format("UserPurchaseHistory    {0}", userNm));
            try
            {
                return FacadesBridge.UserPurchaseHistory(GetLoggedInUser(sessionId), userNm);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("UserPurchaseHistory    {0}", e.ToString()));
                throw e;
            }
        }

        public IEnumerable<DataPurchase> StorePurchaseHistory(string sessionId, string storeName)
        {
            Log.Info(string.Format("StorePurchaseHistory    {0}", storeName));
            try
            {
                return FacadesBridge.StorePurchaseHistory(GetLoggedInUser(sessionId), storeName);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("StorePurchaseHistory    {0}", e.ToString()));
                throw e;
            }
        }

        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string sessionId, string storeName)
        {
            Log.Info(string.Format("ManagingPurchaseHistory    {0}", storeName));
            return FacadesBridge.ManagingPurchaseHistory(GetLoggedInUser(sessionId), storeName);
        }

        public DataProduct AddProduct(string sessionId, string storeName, string productName, string description,
                                        string category, double price, int quantity)
        {
            Log.Info(string.Format("AddProduct    {0}    {1}    {2}    {3}    {4}    {5}",
                storeName, productName, description, category, price, quantity));
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
            Log.Info(string.Format("RemoveProduct    {0}    {1}", storeName, productName));
            FacadesBridge.RemoveProduct(GetLoggedInUser(sessionId), storeName, productName);
            // we don't need to remove the product's description cus there are lots of produts with possibly similar descriptions
            // same applies for category
            TyposFixerNames.RemoveFromDictionary(productName);
        }

        public void EditProductDescription(string sessionId, string storeName, string productName, string description)
        {
            Log.Info(string.Format("RemoveProduct    {0}    {1}    {2}", storeName, productName, description));
            FacadesBridge.EditProductDescription(GetLoggedInUser(sessionId), storeName, productName, description);
        }

        public void EditProductPrice(string sessionId, string storeName, string productName, double price)
        {
            Log.Info(string.Format("EditProductPrice    {0}    {1}    {2}", storeName, productName, price));
            FacadesBridge.EditProductPrice(GetLoggedInUser(sessionId), storeName, productName, price);
        }

        public void EditProductCategory(string sessionId, string storeName, string productName, string category)
        {
            Log.Info(string.Format("EditProductCategory    {0}    {1}    {2}", storeName, productName, category));
            FacadesBridge.EditProductCategory(GetLoggedInUser(sessionId), storeName, productName, category);
        }

        public void EditProductName(string sessionId, string storeName, string productName, string name)
        {
            Log.Info(string.Format("EditProductName    {0}    {1}    {2}", storeName, productName, name));
            FacadesBridge.EditProductName(GetLoggedInUser(sessionId), storeName, productName, name);
        }

        public void EditProductQuantity(string sessionId, string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("EditProductQuantity    {0}    {1}    {2}", storeName, productName, quantity));
            FacadesBridge.EditProductQuantity(GetLoggedInUser(sessionId), storeName, productName, quantity);
        }

        public void AnswerOwnershipRequest(string sessionId, string storeName, string newOwnerUserName, RequestState answer)
        {
            Log.Info(string.Format("AnswerOwnershipRequest    {0}    {1}    {2}", storeName, newOwnerUserName, nameof(answer)));
            FacadesBridge.AnswerOwnershipRequest(GetLoggedInUser(sessionId), storeName, newOwnerUserName, answer);
        }

        public void AddStoreOwner(string sessionId, string storeName, string username)
        {
            Log.Info(string.Format("AddStoreOwner    {0}    {1}", storeName, username));
            var request = FacadesBridge.AddStoreOwner(GetLoggedInUser(sessionId), storeName, username);
            if (request == null)
            {
                return;
            }
            foreach(var obsrv in OwnershipRequestObservers)
            {
                obsrv.Notify(request);
            }
        }

        public void AddStoreManager(string sessionId, string storeName, string username)
        {
            Log.Info(string.Format("AddStoreManager    {0}    {1}", storeName, username));
            FacadesBridge.AddStoreManager(GetLoggedInUser(sessionId), storeName, username);
        }

        public void SetPermissionsOfManager(string sessionId, string storeName, string username, string auth)
        {
            Log.Info(string.Format("SetPermissionsOfManager    {0}    {1}    {2}", storeName, username, auth));
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
            Log.Info(string.Format("SetPermissionsOfManager    {0}    {1}    {2}", storeName, username, auth));
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
            Log.Info(string.Format("RemoveStoreManager    {0}    {1}", storeName, username));
            FacadesBridge.RemoveStoreManager(GetLoggedInUser(sessionId), storeName, username);
        }

        public void RemoveStoreOwner(string sessionId, string storeName, string username)
        {
            Log.Info(string.Format("RemoveStoreOwner    {0}    {1}", storeName, username));
            FacadesBridge.RemoveStoreOwner(GetLoggedInUser(sessionId), storeName, username);
        }

        public IEnumerable<DataMessage> ViewMessage(string sessionId, string storeName)
        {
            Log.Info(string.Format("ViewMessage    {0}", storeName));
            return FacadesBridge.ViewMessage(GetLoggedInUser(sessionId), storeName);
        }

        public DataMessage MessageReply(string sessionId, DataMessage message, string storeName, string description)
        {
            Log.Info(string.Format("MessageReply    {0}    {1}    {2}", storeName, message.Id, description));
            try
            {
                var msg = FacadesBridge.MessageReply(GetLoggedInUser(sessionId), message, storeName, description);
                NotifyMsgObservers(msg);
                return msg;
            }
            catch(Exception e)
            {
                Log.Info(string.Format("MessageReply    {0}", e.ToString()));
                throw e;
            }
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
            Log.Info(string.Format("AddAlwaysTruePolicy    {0}    {1}", storeName, op));
            FacadesBridge.AddAlwaysTruePolicy(GetLoggedInUser(sessionId), storeName, op);
        }

        public void AddSingleProductQuantityPolicy(string sessionId, string storeName, Operator op, string productName, int minQuantity, int maxQuantity)
        {
            Log.Info(string.Format("AddSingleProductQuantityPolicy    {0}    {1}    {2}    {3}    {4}",
                storeName, productName, op, minQuantity, maxQuantity));
            FacadesBridge.AddSingleProductQuantityPolicy(GetLoggedInUser(sessionId), storeName, op, productName, minQuantity, maxQuantity);
        }

        public void AddSystemDayPolicy(string sessionId, string storeName, Operator op, Weekday cantBuyIn)
        {
            Log.Info(string.Format("AddSystemDayPolicy    {0}    {1}    {2}", storeName, op, cantBuyIn.ToString()));
            FacadesBridge.AddSystemDayPolicy(GetLoggedInUser(sessionId), storeName, op, cantBuyIn);
        }

        public void AddUserCityPolicy(string sessionId, string storeName, Operator op, string requiredCity)
        {
            Log.Info(string.Format("AddUserCityPolicy    {0}    {1}    {2}", storeName, op, requiredCity));
            FacadesBridge.AddUserCityPolicy(GetLoggedInUser(sessionId), storeName, op, requiredCity);
        }

        public void AddUserCountryPolicy(string sessionId, string storeName, Operator op, string requiredCountry)
        {
            Log.Info(string.Format("AddUserCountryPolicy    {0}    {1}    {2}", storeName, op, requiredCountry));
            FacadesBridge.AddUserCountryPolicy(GetLoggedInUser(sessionId), storeName, op, requiredCountry);
        }

        public void AddWholeStoreQuantityPolicy(string sessionId, string storeName, Operator op, int minQuantity, int maxQuantity)
        {
            Log.Info(string.Format("AddWholeStoreQuantityPolicy    {0}    {1}    {2}    {3}",
                storeName, op, minQuantity, maxQuantity));
            FacadesBridge.AddWholeStoreQuantityPolicy(GetLoggedInUser(sessionId), storeName, op, minQuantity, maxQuantity);
        }

        public void RemovePolicy(string sessionId, string storeName, int indexInChain)
        {
            Log.Info(string.Format("RemovePolicy    {0}    {1}", storeName, indexInChain));
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
            Log.Info(string.Format("MarkAllDiscussionAsRead    {0}    {1}", storeName, msg.Id));
            FacadesBridge.MarkAllDiscussionAsRead(GetLoggedInUser(sessionId), storeName, msg);
        }

        public void AddProductCategoryDiscount(string sessionId, string storeName, string categoryName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            Log.Info(string.Format("AddProductCategoryDiscount    {0}    {1}    {2}    {3}    {4}    {5}",
                storeName, categoryName, deadline, percentage, op, indexInChain));
            FacadesBridge.AddProductCategoryDiscount(GetLoggedInUser(sessionId), storeName, categoryName, deadline, percentage, op, indexInChain, disId, toLeft);
        }
        public void AddBuyOverDiscount(double minSum, string sessionId, string storeName, string productName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            Log.Info(string.Format("AddBuyOverDiscount    {0}    {1}    {2}    {3}    {4}    {5}    {6}",
                  storeName, productName, deadline, percentage, op, indexInChain, minSum));
            FacadesBridge.AddBuyOverDiscount(GetLoggedInUser(sessionId), storeName, productName, minSum,deadline, percentage, op, indexInChain, disId, toLeft);
        }
        public void AddBuySomeGetSomeDiscount(int buySome, int getSome, string sessionId, string conditionProdName, string underDiscountProdName, string storeName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            Log.Info(string.Format("AddBuySomeGetSomeDiscount    {0}    {1}    {2}    {3}    {4}    {5}    {6}    {7}",
                    storeName, conditionProdName, deadline, percentage, op, indexInChain, buySome, getSome));
            FacadesBridge.AddBuySomeGetSomeDiscount(GetLoggedInUser(sessionId), storeName, conditionProdName, underDiscountProdName, buySome, getSome, deadline, percentage, op, indexInChain, disId, toLeft);
        }

        public void AddSpecificProductDiscount(string sessionId, string storeName, string productName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            Log.Info(string.Format("AddSpecificProductDiscount    {0}    {1}    {2}    {3}    {4}    {5}",
                    storeName, productName, deadline, percentage, op, indexInChain));
            FacadesBridge.AddSpecificProductDiscount(GetLoggedInUser(sessionId), storeName, productName, deadline, percentage, op, indexInChain, disId, toLeft);
        }

        public void RemoveDiscount(string sessionId, string storeName, int indexInChain)
        {
            Log.Info(string.Format("RemoveDiscount    {0}    {1}", storeName, indexInChain));
            FacadesBridge.RemoveDiscount(GetLoggedInUser(sessionId), storeName, indexInChain);
        }

        public IEnumerable<string> GetAllUsers(string sessionId)
        {
            return FacadesBridge.GetRegisteredUsers();
        }

        public void RegisterPurchaseObserver(IServiceObserver<DataPurchase> obsrv)
        {
            PurchaseObservers.Add(obsrv);
        }

        public double GetIncomeInDate(string sessionId, DateTime date)
        {
            GetAdmin(sessionId);   //if it throws an exception, the user is not an admin and it should not be served
            return FacadesBridge.GetIncomeInDate(date);
        }

        public void RegisterOwnershipObserver(IServiceObserver<DataOwnershipRequest> obsrv)
        {
            OwnershipRequestObservers.Add(obsrv);
        }

        public string GetLoggedInUsername(string sessionId)
        {
            try
            {
                return GetDataLoggedInUser(sessionId).Username;
            }
            catch(UserIsNotLoggedInException)
            {
                return "";
            }
        }
    }
}
