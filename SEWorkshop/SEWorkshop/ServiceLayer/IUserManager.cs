using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.DataModels;
using SEWorkshop.Enums;

namespace SEWorkshop.ServiceLayer
{
    public interface IUserManager
    {
        public void AddProductToCart(string sessionId, string storeName, string productName, int quantity);
        public IEnumerable<DataStore> BrowseStores();
        public DataStore SearchStore(string storeName);
        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public void Login(string sessionId, string username, string password);
        public void Logout(string sessionId);
        public DataUser GetUser(string sessionId);
        public IEnumerable<DataBasket> MyCart(string sessionId);
        public void OpenStore(string sessionId, string storeName);
        public void Purchase(string sessionId, DataBasket basket, string creditCardNumber, Address address);
        public void Register(string sessionId, string username, string password);
        public void RemoveProductFromCart(string sessionId, string storeName, string productName, int quantity);

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

        public IEnumerable<DataPurchase> PurchaseHistory(string sessionId);
        public void WriteReview(string sessionId, string storeName, string productName, string description);
        public int WriteMessage(string sessionId, string storeName, string description);
        public IEnumerable<DataPurchase> UserPurchaseHistory(string sessionId, string userNm);
        public IEnumerable<DataPurchase> StorePurchaseHistory(string sessionId, string storeNm);
        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string sessionId, string storeNm);
        public DataProduct AddProduct(string sessionId, string storeNm, string productName, string description, string category, double price, int quantity);
        public void RemoveProduct(string sessionId, string storeNm, string productName);
        public void AddStoreOwner(string sessionId, string storeNm, string username);
        public void AddStoreManager(string sessionId, string storeNm, string username);
        public void SetPermissionsOfManager(string sessionId, string storeNm, string username, string authorization);
        public void RemoveStoreManager(string sessionId, string storeNm, string username);
        public IEnumerable<DataMessage> ViewMessage(string sessionId, string storeNm);
        public DataMessage MessageReply(string sessionId, DataMessage message, string storeNm, string description);
        public void EditProductName(string sessionId, string storeName, string productName, string Name);
        public void EditProductCategory(string sessionId, string storeName, string productName, string category);
        public void EditProductPrice(string sessionId, string storeName, string productName, double price);
        public void EditProductQuantity(string sessionId, string storeName, string productName, int quantity);
        public void EditProductDescription(string sessionId, string storeName, string productName, string description);
        public bool IsLoggedIn(string sessionId);
        public bool IsAdministrator(string sessionId);

        //All add policies are adding to the end
        public void AddAlwaysTruePolicy(string sessionId, string storeName, Operator op);
        public void AddSingleProductQuantityPolicy(string sessionId, string storeName, Operator op, string productName, int minQuantity, int maxQuantity);
        public void AddSystemDayPolicy(string sessionId, string storeName, Operator op, DayOfWeek cantBuyIn);
        public void AddUserCityPolicy(string sessionId, string storeName, Operator op, string requiredCity);
        public void AddUserCountryPolicy(string sessionId, string storeName, Operator op, string requiredCountry);
        public void AddWholeStoreQuantityPolicy(string sessionId, string storeName, Operator op, int minQuantity, int maxQuantity);
        
        //0 based index - first policy is indexed 0
        public void RemovePolicy(string sessionId, string storeName, int indexInChain);
        public DataLoggedInUser GetDataLoggedInUser(string sessionId);
        public void RegisterMessageObserver(IServiceObserver<DataMessage> obsrv);
        public void MarkAllDiscussionAsRead(string sessionId, string storeName, DataMessage msg);

        public void AddProductCategoryDiscount(string sessionId, string storeName, string categoryName, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);
        public void AddSpecificProductDiscount(string sessionId, string storeName, string productName, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);
        public void AddBuySomeGetSomeDiscount(int buySome, int getSome, string sessionId, string conditionProdName, string underDiscountProdName, string storeName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft);
        public void AddBuyOverDiscount(double minSum, string sessionId, string storeName, string productName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft);

        public void RemoveDiscount(string sessionId, string storeName, int indexInChain);
        public void RemovePermissionsOfManager(string sessionId, string storeName, string username, string auth);
        public void RegisterPurchaseObserver(IServiceObserver<DataPurchase> obsrv);
        public IEnumerable<string> GetAllUsers(string sessionId);
    }
}
