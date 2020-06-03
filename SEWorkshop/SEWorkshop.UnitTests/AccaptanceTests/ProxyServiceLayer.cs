using SEWorkshop.DataModels;
using SEWorkshop.Enums;
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

        public override DataProduct AddProduct(string sid, string storeNm, string productName, string description, string category, double price, int quantity)
        {
            return userManager.AddProduct(sid, storeNm, productName, description, category, price, quantity);
        }

        public override void AddProductToCart(string sid, string storeName, string productName, int quantity)
        {
            userManager.AddProductToCart(sid, storeName, productName, quantity);
        }

        public override void AddStoreManager(string sid, string storeNm, string username)
        {
            userManager.AddStoreManager(sid, storeNm, username);
        }

        public override void AnswerOwnershipRequest(string sid, string store, string username, string answer)
        {
            userManager.AnswerOwnershipRequest(sid,  store, username,  answer);
        }
        
        public override void AddStoreOwner(string sid, string storeNm, string username)
        {
            userManager.AddStoreOwner(sid, storeNm, username);
        }

        public override IEnumerable<DataStore> BrowseStores()
        {
            return userManager.BrowseStores();
        }

        public override void EditProductCategory(string sid, string storeName, string productName, string category)
        {
            userManager.EditProductCategory(sid, storeName, productName, category);
        }

        public override void EditProductDescription(string sid, string storeName, string productName, string description)
        {
            userManager.EditProductDescription(sid, storeName, productName, description);
        }

        public override void EditProductName(string sid, string storeName, string productName, string Name)
        {
            userManager.EditProductName(sid, storeName, productName, Name);
        }

        public override void EditProductPrice(string sid, string storeName, string productName, double price)
        {
            userManager.EditProductPrice(sid, storeName, productName, price);
        }

        public override void EditProductQuantity(string sid, string storeName, string productName, int quantity)
        {
            userManager.EditProductQuantity(sid, storeName, productName, quantity);
        }

        public override IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            return FilterProducts(products, pred);
        }

        public override void Login(string sid, string username, string password)
        {
            userManager.Login(sid, username, password);
        }

        public override void Logout(string sid)
        {
            userManager.Logout(sid);
        }

        public override IEnumerable<DataPurchase> ManagingPurchaseHistory(string sid, string storeNm)
        {
            return userManager.ManagingPurchaseHistory(sid, storeNm);
        }

        public override DataMessage MessageReply(string sid, DataMessage message, string storeNm, string description)
        {
            return userManager.MessageReply(sid, message, storeNm, description);
        }

        public override IEnumerable<DataBasket> MyCart(string sid)
        {
            return userManager.MyCart(sid);
        }

        public override void OpenStore(string sid, string storeName)
        {
            userManager.OpenStore(sid, storeName);
        }

        public override IEnumerable<DataPurchase> PurchaseHistory(string sid)
        {
            return userManager.PurchaseHistory(sid);
        }

        public override void Purchase(string sid, DataBasket basket, string creditCardNumber, Address address)
        {
            userManager.Purchase(sid, basket, creditCardNumber, address);
        }

        public override void Register(string sid, string username, string password)
        {
            userManager.Register(sid, username, password);
        }

        public override void RemoveProduct(string sid, string storeNm, string productName)
        {
            userManager.RemoveProduct(sid, storeNm, productName);
        }

        public override void RemoveProductFromCart(string sid, string storeName, string productName, int quantity)
        {
            userManager.RemoveProductFromCart(sid, storeName, productName, quantity);
        }

        public override void RemoveStoreManager(string sid, string storeNm, string username)
        {
            userManager.RemoveStoreManager(sid, storeNm, username);
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

        public override void SetPermissionsOfManager(string sid, string storeNm, string username, string authorization)
        {
            userManager.SetPermissionsOfManager(sid, storeNm, username, authorization);
        }

        public override IEnumerable<DataPurchase> StorePurchaseHistory(string sid, string storeNm)
        {
            return userManager.StorePurchaseHistory(sid, storeNm);
        }

        public override IEnumerable<DataPurchase> UserPurchaseHistory(string sid, string userNm)
        {
            return userManager.UserPurchaseHistory(sid, userNm);
        }

        public override IEnumerable<DataMessage> ViewMessage(string sid, string storeNm)
        {
            return userManager.ViewMessage(sid, storeNm);
        }

        public override void WriteMessage(string sid, string storeName, string description)
        {
            userManager.WriteMessage(sid, storeName, description);
        }

        public override void WriteReview(string sid, string storeName, string productName, string description)
        {
            userManager.WriteReview(sid, storeName, productName, description);
        }

        public override void RegisterPurchaseObserver(IServiceObserver<DataPurchase> obsrv)
        {
            userManager.RegisterPurchaseObserver(obsrv);
        }

        public override void AddAlwaysTruePolicy(string sessionId, string storeName, Operator op)
        {
            userManager.AddAlwaysTruePolicy(sessionId, storeName, op);
        }

        public override void AddSingleProductQuantityPolicy(string sessionId, string storeName, Operator op, string productName, int minQuantity, int maxQuantity)
        {
            userManager.AddSingleProductQuantityPolicy(sessionId, storeName, op, productName, minQuantity, maxQuantity);
        }

        public override void AddSystemDayPolicy(string sessionId, string storeName, Operator op, DayOfWeek cantBuyIn)
        {
            userManager.AddSystemDayPolicy(sessionId, storeName, op, cantBuyIn);
        }

        public override void AddUserCityPolicy(string sessionId, string storeName, Operator op, string requiredCity)
        {
            userManager.AddUserCityPolicy(sessionId, storeName, op, requiredCity);
        }

        public override void AddUserCountryPolicy(string sessionId, string storeName, Operator op, string requiredCountry)
        {
            userManager.AddUserCountryPolicy(sessionId, storeName, op, requiredCountry);
        }

        public override void AddWholeStoreQuantityPolicy(string sessionId, string storeName, Operator op, int minQuantity, int maxQuantity)
        {
            userManager.AddWholeStoreQuantityPolicy(sessionId, storeName, op, minQuantity, maxQuantity);
        }

        public override void RemovePolicy(string sessionId, string storeName, int indexInChain)
        {
            userManager.RemovePolicy(sessionId, storeName, indexInChain);
        }

        public override void AddProductCategoryDiscount(string sessionId, string storeName, string categoryName, DateTime deadline, double percentage, Operator op, int indexInChain, int disld, bool toLeft)
        {
            userManager.AddProductCategoryDiscount(sessionId, storeName, categoryName, deadline, percentage, op, indexInChain, disld, toLeft);
        }

        public override void AddSpecificProductDiscount(string sessionId, string storeName, string productName, DateTime deadline, double percentage, Operator op, int indexInChain, int disld, bool toLeft)
        {
            userManager.AddSpecificProductDiscount(sessionId, storeName, productName, deadline, percentage, op, indexInChain, disld, toLeft);
        }

        public override void AddBuySomeGetSomeDiscount(int buySome, int getSome, string sessionId, string conditionProductName, string underDiscountProductName, string storeName, DateTime deadline, double percentage, Operator op, int indexInChain, int disld, bool toLeft)
        {
            userManager.AddBuySomeGetSomeDiscount(buySome, getSome, sessionId, conditionProductName, underDiscountProductName, storeName, deadline, percentage, op, indexInChain, disld, toLeft);
        }

        public override void AddBuyOverDiscount(double minSum, string sessionId, string storeName, string productName, DateTime deadline, double percentage, Operator op, int indexInChain, int disld, bool toLeft)
        {
            userManager.AddBuyOverDiscount(minSum, sessionId, storeName, productName, deadline, percentage, op, indexInChain, disld, toLeft);
        }

        public override void RemoveDiscount(string sessionId, string storeName, int indexInChain)
        {
            userManager.RemoveDiscount(sessionId, storeName, indexInChain);
        }
    }
}
