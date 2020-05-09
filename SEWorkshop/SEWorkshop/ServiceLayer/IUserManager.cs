﻿using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.DataModels;

namespace SEWorkshop.ServiceLayer
{
    public interface IUserManager
    {
        public void AddProductToCart(string storeName, string productName, int quantity);
        public IEnumerable<DataStore> BrowseStores();
        public DataStore SearchStore(string storeName);
        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public void Login(string username, string password);
        public void Logout();
        public IEnumerable<DataBasket> MyCart();
        public void OpenStore(string storeName);
        public void Purchase(DataBasket basket, string creditCardNumber, Address address);
        public void Register(string username, string password);
        public void RemoveProductFromCart(string storeName, string productName, int quantity);

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

        public IEnumerable<DataPurchase> PurchaseHistory();
        public void WriteReview(string storeName, string productName, string description);
        public void WriteMessage(string storeName, string description);
        public IEnumerable<DataPurchase> UserPurchaseHistory(string userNm);
        public IEnumerable<DataPurchase> StorePurchaseHistory(string storeNm);
        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeNm);
        public DataProduct AddProduct(string storeNm, string productName, string description, string category, double price, int quantity);
        public void RemoveProduct(string storeNm, string productName);
        public void AddStoreOwner(string storeNm, string username);
        public void AddStoreManager(string storeNm, string username);
        public void SetPermissionsOfManager(string storeNm, string username, string authorization);
        public void RemoveStoreManager(string storeNm, string username);
        public IEnumerable<DataMessage> ViewMessage(string storeNm);
        public DataMessage MessageReply(DataMessage message, string storeNm, string description);
        public void EditProductName(string storeName, string productName, string Name);
        public void EditProductCategory(string storeName, string productName, string category);
        public void EditProductPrice(string storeName, string productName, double price);
        public void EditProductQuantity(string storeName, string productName, int quantity);
        public void EditProductDescription(string storeName, string productName, string description);
    }
}
