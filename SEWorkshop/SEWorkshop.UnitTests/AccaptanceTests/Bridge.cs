using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Tests.AccaptanceTests
{
    abstract class Bridge
    {
        public abstract void AddProductToCart(string storeName, string productName, int quantity);
        public abstract IEnumerable<Store> BrowseStores();
        public abstract IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred);
        public abstract void Login(string username, string password);
        public abstract void Logout();
        public abstract IEnumerable<Basket> MyCart();
        public abstract void OpenStore(string storeName);
        public abstract void Purchase(Basket basket);
        public abstract void Register(string username, string password);
        public abstract void RemoveProductFromCart(string storeName, string productName, int quantity);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public abstract IEnumerable<Product> SearchProductsByName(ref string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public abstract IEnumerable<Product> SearchProductsByCategory(ref string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public abstract IEnumerable<Product> SearchProductsByKeywords(ref string input);

        public abstract IEnumerable<Purchase> PurcahseHistory();
        public abstract void WriteReview(string storeName, string productName, string description);
        public abstract void WriteMessage(string storeName, string description);
        public abstract IEnumerable<Purchase> UserPurchaseHistory(string userNm);
        public abstract IEnumerable<Purchase> StorePurchaseHistory(string storeNm);
        public abstract IEnumerable<Purchase> ManagingPurchaseHistory(string storeNm);
        public abstract Product AddProduct(string storeNm, string productName, string description, string category, double price, int quantity);
        public abstract void RemoveProduct(string storeNm, string productName);
        public abstract void AddStoreOwner(string storeNm, string username);
        public abstract void AddStoreManager(string storeNm, string username);
        public abstract void SetPermissionsOfManager(string storeNm, string username, string authorization);
        public abstract void RemoveStoreManager(string storeNm, string username);
        public abstract IEnumerable<Message> ViewMessage(string storeNm);
        public abstract Message MessageReply(Message message, string storeNm, string description);
        public abstract void EditProductName(string storeName, string productName, string Name);
        public abstract void EditProductCategory(string storeName, string productName, string category);
        public abstract void EditProductPrice(string storeName, string productName, double price);
        public abstract void EditProductQuantity(string storeName, string productName, int quantity);
        public abstract void EditProductDescription(string storeName, string productName, string description);
    }
}
