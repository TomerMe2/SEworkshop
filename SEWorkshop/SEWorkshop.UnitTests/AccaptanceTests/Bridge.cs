using SEWorkshop.DataModels;
using System;
using System.Collections.Generic;

namespace SEWorkshop.Tests.AccaptanceTests
{
    abstract class Bridge
    {
        public abstract void AddProductToCart(string storeName, string productName, int quantity);
        public abstract IEnumerable<DataStore> BrowseStores();
        public abstract IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public abstract void Login(string username, string password);
        public abstract void Logout();
        public abstract IEnumerable<DataBasket> MyCart();
        public abstract void OpenStore(string storeName);
        public abstract void Purchase(DataBasket basket);
        public abstract void Register(string username, string password);
        public abstract void RemoveProductFromCart(string storeName, string productName, int quantity);
        public abstract IEnumerable<DataProduct> SearchProductsByName(ref string input);
        public abstract IEnumerable<DataProduct> SearchProductsByCategory(ref string input);
        public abstract IEnumerable<DataProduct> SearchProductsByKeywords(ref string input);
        public abstract IEnumerable<DataPurchase> PurchaseHistory();
        public abstract void WriteReview(string storeName, string productName, string description);
        public abstract void WriteMessage(string storeName, string description);
        public abstract IEnumerable<DataPurchase> UserPurchaseHistory(string userNm);
        public abstract IEnumerable<DataPurchase> StorePurchaseHistory(string storeNm);
        public abstract IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeNm);
        public abstract DataProduct AddProduct(string storeNm, string productName, string description, string category, double price, int quantity);
        public abstract void RemoveProduct(string storeNm, string productName);
        public abstract void AddStoreOwner(string storeNm, string username);
        public abstract void AddStoreManager(string storeNm, string username);
        public abstract void SetPermissionsOfManager(string storeNm, string username, string authorization);
        public abstract void RemoveStoreManager(string storeNm, string username);
        public abstract IEnumerable<DataMessage> ViewMessage(string storeNm);
        public abstract DataMessage MessageReply(DataMessage message, string storeNm, string description);
        public abstract void EditProductName(string storeName, string productName, string Name);
        public abstract void EditProductCategory(string storeName, string productName, string category);
        public abstract void EditProductPrice(string storeName, string productName, double price);
        public abstract void EditProductQuantity(string storeName, string productName, int quantity);
        public abstract void EditProductDescription(string storeName, string productName, string description);
    }
}
