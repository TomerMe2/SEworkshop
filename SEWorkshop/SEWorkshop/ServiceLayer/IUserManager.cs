using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.DataModels;
using SEWorkshop.Enums;

namespace SEWorkshop.ServiceLayer
{
    public interface IUserManager
    {
        public void Register(string sessionId, string username, string password);
        public void Login(string sessionId, string username, string password);
        public void Logout(string sessionId);
        public IEnumerable<DataBasket> MyCart(string sessionId);
        public void AddProductToCart(string sessionId, string storeName, string productName, int quantity);
        public void RemoveProductFromCart(string sessionId, string storeName, string productName, int quantity);
        public IEnumerable<DataStore> BrowseStores();
        public DataStore SearchStore(string storeName);
        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<DataProduct> SearchProductsByName(ref string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<DataProduct> SearchProductsByCategory(ref string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<DataProduct> SearchProductsByKeywords(ref string input);
        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public void Purchase(string sessionId, DataBasket basket, string creditCardNumber, Address address);
        public void OpenStore(string sessionId, string storeName);
        public void WriteReview(string sessionId, string storeName, string productName, string description);
        public int WriteMessage(string sessionId, string storeName, string description);
        public IEnumerable<DataPurchase> PurchaseHistory(string sessionId);
        public IEnumerable<DataPurchase> UserPurchaseHistory(string sessionId, string userNm);
        public IEnumerable<DataPurchase> StorePurchaseHistory(string sessionId, string storeNm);
        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string sessionId, string storeNm);
        // Store Manage
        public DataProduct AddProduct(string sessionId, string storeNm, string productName, string description, string category, double price, int quantity);
        public void EditProductName(string sessionId, string storeName, string productName, string name);
        public void EditProductCategory(string sessionId, string storeName, string productName, string category);
        public void EditProductDescription(string sessionId, string storeName, string productName, string description);
        public void EditProductPrice(string sessionId, string storeName, string productName, double price);
        public void EditProductQuantity(string sessionId, string storeName, string productName, int quantity);
        public void RemoveProduct(string sessionId, string storeNm, string productName);
        //All add policies are adding to the end
        public void AddAlwaysTruePolicy(string sessionId, string storeName, Operator op);
        public void AddSingleProductQuantityPolicy(string sessionId, string storeName, Operator op, string productName, int minQuantity, int maxQuantity);
        public void AddSystemDayPolicy(string sessionId, string storeName, Operator op, Weekday cantBuyIn);
        public void AddUserCityPolicy(string sessionId, string storeName, Operator op, string requiredCity);
        public void AddUserCountryPolicy(string sessionId, string storeName, Operator op, string requiredCountry);
        public void AddWholeStoreQuantityPolicy(string sessionId, string storeName, Operator op, int minQuantity, int maxQuantity);
        //0 based index - first policy is indexed 0
        public void RemovePolicy(string sessionId, string storeName, int indexInChain);
        public void AddProductCategoryDiscount(string sessionId, string storeName, string categoryName, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);
        public void AddSpecificProductDiscount(string sessionId, string storeName, string productName, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);
        public void AddBuySomeGetSomeDiscount(int buySome, int getSome, string sessionId, string conditionProdName, string underDiscountProdName, string storeName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft);
        public void AddBuyOverDiscount(double minSum, string sessionId, string storeName, string productName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft);
        public void RemoveDiscount(string sessionId, string storeName, int indexInChain);
        public void AddStoreOwner(string sessionId, string storeNm, string username);
        public void AddStoreManager(string sessionId, string storeNm, string username);
        public void SetPermissionsOfManager(string sessionId, string storeNm, string username, string authorization);
        public void RemovePermissionsOfManager(string sessionId, string storeName, string username, string auth);
        public void RemoveStoreOwner(string sessionId, string storeNm, string username);
        public void RemoveStoreManager(string sessionId, string storeNm, string username);
        public IEnumerable<DataMessage> ViewMessage(string sessionId, string storeNm);
        public DataMessage MessageReply(string sessionId, DataMessage message, string storeNm, string description);
        public void MarkAllDiscussionAsRead(string sessionId, string storeName, DataMessage msg);
        public void RegisterPurchaseObserver(IServiceObserver<DataPurchase> obsrv);
        public IEnumerable<string> GetAllUsers(string sessionId);
        public double GetIncomeInDate(string sessionId, DateTime date);

        //keys are days, values are the number of visitors in the day of the key
        public IDictionary<DateTime, IDictionary<KindOfUser,int>> GetUseRecord(string sessionId, DateTime dateFrom, DateTime dateTo, List<KindOfUser> kinds);
        public IDictionary<KindOfUser, int> GetUsersByCategory(string sessionId, DateTime today);
        public void AnswerOwnershipRequest(string sessionId, string storeName, string newOwnerUserName, RequestState answer);
        public void RegisterOwnershipObserver(IServiceObserver<DataOwnershipRequest> obsrv);
        public void RegisterNewUseReportObserver(IServiceObserver<KindOfUser> obsrv);
        public DataUser GetUser(string sessionId);
        public DataLoggedInUser GetDataLoggedInUser(string sessionId);
        public bool IsLoggedIn(string sessionId);
        public bool IsAdministrator(string sessionId);
        public void RegisterMessageObserver(IServiceObserver<DataMessage> obsrv);
        public string GetLoggedInUsername(string sessionId);
        public void AccessSystem(string sessionId);
    }
}
