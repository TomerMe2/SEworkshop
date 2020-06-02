using SEWorkshop.DataModels;
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
        public abstract void AnswerOwnershipRequest(string sid,string store, string username, string answer);
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

    }
}
