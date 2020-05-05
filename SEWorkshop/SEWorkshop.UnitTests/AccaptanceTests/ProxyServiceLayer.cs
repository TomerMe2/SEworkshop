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
        public override DataProduct AddProduct(string storeNm, string productName, string description, string category, double price, int quantity)
        {
            return userManager.AddProduct(storeNm, productName, description, category, price, quantity);
        }

        public override void AddProductToCart(string storeName, string productName, int quantity)
        {
            userManager.AddProductToCart(storeName, productName, quantity);
        }

        public override void AddStoreManager(string storeNm, string username)
        {
            userManager.AddStoreManager(storeNm, username);
        }

        public override void AddStoreOwner(string storeNm, string username)
        {
            userManager.AddStoreOwner(storeNm, username);
        }

        public override IEnumerable<DataStore> BrowseStores()
        {
            return userManager.BrowseStores();
        }

        public override void EditProductCategory(string storeName, string productName, string category)
        {
            userManager.EditProductCategory(storeName, productName, category);
        }

        public override void EditProductDescription(string storeName, string productName, string description)
        {
            userManager.EditProductDescription(storeName, productName, description);
        }

        public override void EditProductName(string storeName, string productName, string Name)
        {
            userManager.EditProductName(storeName, productName, Name);
        }

        public override void EditProductPrice(string storeName, string productName, double price)
        {
            userManager.EditProductPrice(storeName, productName, price);
        }

        public override void EditProductQuantity(string storeName, string productName, int quantity)
        {
            userManager.EditProductQuantity(storeName, productName, quantity);
        }

        public override IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            return FilterProducts(products, pred);
        }

        public override void Login(string username, string password)
        {
            userManager.Login(username, password);
        }

        public override void Logout()
        {
            userManager.Logout();
        }

        public override IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeNm)
        {
            return userManager.ManagingPurchaseHistory(storeNm);
        }

        public override DataMessage MessageReply(DataMessage message, string storeNm, string description)
        {
            return userManager.MessageReply(message, storeNm, description);
        }

        public override IEnumerable<DataBasket> MyCart()
        {
            return userManager.MyCart();
        }

        public override void OpenStore(string storeName)
        {
            userManager.OpenStore(storeName);
        }

        public override IEnumerable<DataPurchase> PurchaseHistory()
        {
            return userManager.PurchaseHistory();
        }

        public override void Purchase(DataBasket basket)
        {
            userManager.Purchase(basket, "555", new Address("Beer Sheva", "Ben Gurion", "99"));
        }

        public override void Register(string username, string password)
        {
            userManager.Register(username, password);
        }

        public override void RemoveProduct(string storeNm, string productName)
        {
            userManager.RemoveProduct(storeNm, productName);
        }

        public override void RemoveProductFromCart(string storeName, string productName, int quantity)
        {
            userManager.RemoveProductFromCart(storeName, productName, quantity);
        }

        public override void RemoveStoreManager(string storeNm, string username)
        {
            userManager.RemoveStoreManager(storeNm, username);
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
            userManager.SetPermissionsOfManager(storeNm, username, authorization);
        }

        public override IEnumerable<DataPurchase> StorePurchaseHistory(string storeNm)
        {
            return userManager.StorePurchaseHistory(storeNm);
        }

        public override IEnumerable<DataPurchase> UserPurchaseHistory(string userNm)
        {
            return userManager.UserPurchaseHistory(userNm);
        }

        public override IEnumerable<DataMessage> ViewMessage(string storeNm)
        {
            return userManager.ViewMessage(storeNm);
        }

        public override void WriteMessage(string storeName, string description)
        {
            userManager.WriteMessage(storeName, description);
        }

        public override void WriteReview(string storeName, string productName, string description)
        {
            userManager.WriteReview(storeName, productName, description);
        }
    }
}
