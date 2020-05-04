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
        public override Product AddProduct(string storeNm, string productName, string description, string category, double price, int quantity)
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

        public override IEnumerable<Store> BrowseStores()
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

        public override IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
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

        public override IEnumerable<Purchase> ManagingPurchaseHistory(string storeNm)
        {
            return userManager.ManagingPurchaseHistory(storeNm);
        }

        public override Message MessageReply(Message message, string storeNm, string description)
        {
            return userManager.MessageReply(message, storeNm, description);
        }

        public override IEnumerable<Basket> MyCart()
        {
            return userManager.MyCart();
        }

        public override void OpenStore(string storeName)
        {
            userManager.OpenStore(storeName);
        }

        public override IEnumerable<Purchase> PurcahseHistory()
        {
            return userManager.PurcahseHistory();
        }

        public override void Purchase(Basket basket)
        {
            userManager.Purchase(basket);
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

        public override IEnumerable<Product> SearchProductsByCategory(ref string input)
        {
            return userManager.SearchProductsByCategory(ref input);
        }

        public override IEnumerable<Product> SearchProductsByKeywords(ref string input)
        {
            return userManager.SearchProductsByKeywords(ref input);
        }

        public override IEnumerable<Product> SearchProductsByName(ref string input)
        {
            return userManager.SearchProductsByName(ref input);
        }

        public override void SetPermissionsOfManager(string storeNm, string username, string authorization)
        {
            userManager.SetPermissionsOfManager(storeNm, username, authorization);
        }

        public override IEnumerable<Purchase> StorePurchaseHistory(string storeNm)
        {
            return userManager.StorePurchaseHistory(storeNm);
        }

        public override IEnumerable<Purchase> UserPurchaseHistory(string userNm)
        {
            return userManager.UserPurchaseHistory(userNm);
        }

        public override IEnumerable<Message> ViewMessage(string storeNm)
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
