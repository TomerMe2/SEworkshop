using SEWorkshop.DataModels;
using SEWorkshop.Enums;
using System;
using System.Collections.Generic;

namespace SEWorkshop.Tests.AccaptanceTests
{
    abstract class Bridge
    {
        public abstract void AddProductToCart(string sid, string storeName, string productName, int quantity);
        public abstract IEnumerable<DataStore> BrowseStores();
        public abstract IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public abstract void Login(string sid, string username, string password);
        public abstract void Logout(string sid);
        public abstract IEnumerable<DataBasket> MyCart(string sid);
        public abstract void OpenStore(string sid, string storeName);
        public abstract void Purchase(string sid, DataBasket basket, string creditCardNumber, Address address);
        public abstract void Register(string sid, string username, string password);
        public abstract void RemoveProductFromCart(string sid, string storeName, string productName, int quantity);
        public abstract IEnumerable<DataProduct> SearchProductsByName(ref string input);
        public abstract IEnumerable<DataProduct> SearchProductsByCategory(ref string input);
        public abstract IEnumerable<DataProduct> SearchProductsByKeywords(ref string input);
        public abstract IEnumerable<DataPurchase> PurchaseHistory(string sid);
        public abstract void WriteReview(string sid, string storeName, string productName, string description);
        public abstract void WriteMessage(string sid, string storeName, string description);
        public abstract IEnumerable<DataPurchase> UserPurchaseHistory(string sid, string userNm);
        public abstract IEnumerable<DataPurchase> StorePurchaseHistory(string sid, string storeNm);
        public abstract IEnumerable<DataPurchase> ManagingPurchaseHistory(string sid, string storeNm);
        public abstract DataProduct AddProduct(string sid, string storeNm, string productName, string description, string category, double price, int quantity);
        public abstract void RemoveProduct(string sid, string storeNm, string productName);
        public abstract void AddStoreOwner(string sid, string storeNm, string username);
        public abstract void AddStoreManager(string sid, string storeNm, string username);
        public abstract void SetPermissionsOfManager(string sid, string storeNm, string username, string authorization);
        public abstract void RemoveStoreManager(string sid, string storeNm, string username);
        public abstract IEnumerable<DataMessage> ViewMessage(string sid, string storeNm);
        public abstract DataMessage MessageReply(string sid, DataMessage message, string storeNm, string description);
        public abstract void EditProductName(string sid, string storeName, string productName, string Name);
        public abstract void EditProductCategory(string sid, string storeName, string productName, string category);
        public abstract void EditProductPrice(string sid, string storeName, string productName, double price);
        public abstract void EditProductQuantity(string sid, string storeName, string productName, int quantity);
        public abstract void EditProductDescription(string sid, string storeName, string productName, string description);
        public abstract void RegisterPurchaseObserver(ServiceLayer.IServiceObserver<DataPurchase> obsrv);

        public abstract void AddAlwaysTruePolicy(string sessionId, string storeName, Operator op);
        public abstract void AddSingleProductQuantityPolicy(string sessionId, string storeName, Operator op, string productName, int minQuantity, int maxQuantity);
        public abstract void AddSystemDayPolicy(string sessionId, string storeName, Operator op, DayOfWeek cantBuyIn);
        public abstract void AddUserCityPolicy(string sessionId, string storeName, Operator op, string requiredCity);
        public abstract void AddUserCountryPolicy(string sessionId, string storeName, Operator op, string requiredCountry);
        public abstract void AddWholeStoreQuantityPolicy(string sessionId, string storeName, Operator op, int minQuantity, int maxQuantity);
        public abstract void RemovePolicy(string sessionId, string storeName, int indexInChain);
        public abstract void AddProductCategoryDiscount(string sessionId, string storeName, string categoryName, DateTime deadline, double percentage, Operator op, int indexInChain);
        public abstract void AddSpecificProductDiscount(string sessionId, string storeName, string productName, DateTime deadline, double percentage, Operator op, int indexInChain);
        public abstract void AddBuySomeGetSomeDiscount(int buySome, int getSome, string sessionId, string productName, string storeName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain);
        public abstract void AddBuyOverDiscount(double minSum, string sessionId, string storeName, string productName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain);
        public abstract void RemoveDiscount(string sessionId, string storeName, int indexInChain);
    }
}
