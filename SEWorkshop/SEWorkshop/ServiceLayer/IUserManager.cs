using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.ServiceLayer
{
    public interface IUserManager
    {
        public void AddProductToCart(string storeName, string productName, int quantity);
        public IEnumerable<Store> BrowseStores();
        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred);
        public void Login(string username, string password);
        public void Logout();
        public IEnumerable<Basket> MyCart();
        public void OpenStore(string storeName);
        public void Purchase(Basket basket);
        public void Register(string username, string password);
        public void RemoveProductFromCart(string storeName, string productName, int quantity);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<Product> SearchProductsByName(ref string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<Product> SearchProductsByCategory(ref string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<Product> SearchProductsByKeywords(ref string input);

        public IEnumerable<Purchase> PurcahseHistory();
        public void WriteReview(string storeName, string productName, string description);
        public void WriteMessage(string storeName, string description);
        public IEnumerable<Purchase> UserPurchaseHistory(string userNm);
        public IEnumerable<Purchase> StorePurchaseHistory(string storeNm);
        public IEnumerable<Purchase> ManagingPurchaseHistory(string storeNm);
        public void AddProduct(string storeNm, string productName, string description, string category, double price, int quantity);
        public void RemoveProduct(string storeNm, string productName);
        public void AddStoreOwner(string storeNm, string username);
        public void AddStoreManager(string storeNm, string username);
        public void SetPermissionsOfManager(string storeNm, string username, string authorization);
        public void RemoveStoreManager(string storeNm, string username);
        public IEnumerable<Message> ViewMessage(string storeNm);
        public void MessageReply(Message message, string storeNm, string description);
        public void EditProductName(string storeName, string productName, string Name);
        public void EditProductCategory(string storeName, string productName, string category);
        public void EditProductPrice(string storeName, string productName, double price);
        public void EditProductQuantity(string storeName, string productName, int quantity);
        public void EditProductDescription(string storeName, string productName, string description);
    }
}
