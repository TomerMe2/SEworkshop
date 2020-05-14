using SEWorkshop.DataModels;
using SEWorkshop.Models;
using SEWorkshop.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Tests.AccaptanceTests
{
    class ProxyServiceLayer : Bridge
    {
        private UserManager userManager = new UserManager();
        private const string DEF_ID = "AccTests";

        public override DataProduct AddProduct(string storeNm, string productName, string description, string category, double price, int quantity)
        {
            return userManager.AddProduct(DEF_ID, storeNm, productName, description, category, price, quantity);
        }

        public override void AddProductToCart(string storeName, string productName, int quantity)
        {
            userManager.AddProductToCart(DEF_ID, storeName, productName, quantity);
        }

        public override void AddStoreManager(string storeNm, string username)
        {
            userManager.AddStoreManager(DEF_ID, storeNm, username);
        }

        public override void AddStoreOwner(string storeNm, string username)
        {
            userManager.AddStoreOwner(DEF_ID, storeNm, username);
        }

        public override IEnumerable<DataStore> BrowseStores()
        {
            return userManager.BrowseStores();
        }

        public override void EditProductCategory(string storeName, string productName, string category)
        {
            userManager.EditProductCategory(DEF_ID, storeName, productName, category);
        }

        public override void EditProductDescription(string storeName, string productName, string description)
        {
            userManager.EditProductDescription(DEF_ID, storeName, productName, description);
        }

        public override void EditProductName(string storeName, string productName, string Name)
        {
            userManager.EditProductName(DEF_ID, storeName, productName, Name);
        }

        public override void EditProductPrice(string storeName, string productName, double price)
        {
            userManager.EditProductPrice(DEF_ID, storeName, productName, price);
        }

        public override void EditProductQuantity(string storeName, string productName, int quantity)
        {
            userManager.EditProductQuantity(DEF_ID, storeName, productName, quantity);
        }

        public override IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            return FilterProducts(products, pred);
        }

        public override void Login(string username, string password)
        {
            userManager.Login(DEF_ID, username, password);
        }

        public override void Logout()
        {
            userManager.Logout(DEF_ID);
        }

        public override IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeNm)
        {
            return userManager.ManagingPurchaseHistory(DEF_ID, storeNm);
        }

        public override DataMessage MessageReply(DataMessage message, string storeNm, string description)
        {
            return userManager.MessageReply(DEF_ID, message, storeNm, description);
        }

        public override IEnumerable<DataBasket> MyCart()
        {
            return userManager.MyCart(DEF_ID);
        }

        public override void OpenStore(string storeName)
        {
            userManager.OpenStore(DEF_ID, storeName);
        }

        public override IEnumerable<DataPurchase> PurchaseHistory()
        {
            return userManager.PurchaseHistory(DEF_ID);
        }

        public override void Purchase(DataBasket basket, string creditCardNumber, Address address)
        {
            userManager.Purchase(DEF_ID, basket, creditCardNumber, address);
        }

        public override void Register(string username, string password)
        {
            userManager.Register(DEF_ID, username, password);
        }

        public override void RemoveProduct(string storeNm, string productName)
        {
            userManager.RemoveProduct(DEF_ID, storeNm, productName);
        }

        public override void RemoveProductFromCart(string storeName, string productName, int quantity)
        {
            userManager.RemoveProductFromCart(DEF_ID, storeName, productName, quantity);
        }

        public override void RemoveStoreManager(string storeNm, string username)
        {
            userManager.RemoveStoreManager(DEF_ID, storeNm, username);
        }

        public override IEnumerable<DataProduct> SearchProductsByCategory(ref string input)
        {
            return userManager.SearchProductsByCategory(ref input);
        }

        public override IEnumerable<DataProduct> SearchProductsByKeywords(ref string input)
        {
            return userManager.SearchProductsByKeywords(ref input);
        }

        public override IEnumerable<DataProduct> SearchProductsByName(ref string input)
        {
            return userManager.SearchProductsByName(ref input);
        }

        public override void SetPermissionsOfManager(string storeNm, string username, string authorization)
        {
            userManager.SetPermissionsOfManager(DEF_ID, storeNm, username, authorization);
        }

        public override IEnumerable<DataPurchase> StorePurchaseHistory(string storeNm)
        {
            return userManager.StorePurchaseHistory(DEF_ID, storeNm);
        }

        public override IEnumerable<DataPurchase> UserPurchaseHistory(string userNm)
        {
            return userManager.UserPurchaseHistory(DEF_ID, userNm);
        }

        public override IEnumerable<DataMessage> ViewMessage(string storeNm)
        {
            return userManager.ViewMessage(DEF_ID, storeNm);
        }

        public override void WriteMessage(string storeName, string description)
        {
            userManager.WriteMessage(DEF_ID, storeName, description);
        }

        public override void WriteReview(string storeName, string productName, string description)
        {
            userManager.WriteReview(DEF_ID, storeName, productName, description);
        }
    }
}
