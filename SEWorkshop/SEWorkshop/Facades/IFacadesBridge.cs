using SEWorkshop.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public interface IFacadesBridge
    {
        public void AddProductToCart(string storeName, string productName, int quantity);
        public IEnumerable<DataStore> BrowseStores();
        public DataStore SearchStore(string storeName);
        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public void Login(string username, byte[] password);
        public void Logout();
        public IEnumerable<DataBasket> MyCart();
        public void OpenStore(string storeName);
        public void Purchase(DataBasket basket, string creditCardNumber, Address address);
        public void Register(string username, byte[] password);
        public void RemoveProductFromCart(string storeName, string productName, int quantity);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<DataProduct> SearchProductsByName(string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<DataProduct> SearchProductsByCategory(string input);

        /// <summary>
        /// input can be corrected inside this method. If it's corrected, the value of input will be changed.
        /// </summary>
        public IEnumerable<DataProduct> SearchProductsByKeywords(string input);

        public IEnumerable<DataPurchase> PurchaseHistory();
        public void WriteReview(string storeName, string productName, string description);
        public void WriteMessage(string storeName, string description);
        public IEnumerable<DataPurchase> UserPurchaseHistory(string userName);
        public IEnumerable<DataPurchase> StorePurchaseHistory(string storeName);
        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeName);
        public DataProduct AddProduct(string storeName, string productName, string description, string category, double price, int quantity);
        public void RemoveProduct(string storeName, string productName);
        public void AddStoreOwner(string storeName, string username);
        public void AddStoreManager(string storeName, string username);
        public void SetPermissionsOfManager(string storeName, string username, Authorizations authorization);
        public void RemoveStoreManager(string storeName, string username);
        public IEnumerable<DataMessage> ViewMessage(string storeNm);
        public DataMessage MessageReply(DataMessage message, string storeName, string description);
        public void EditProductName(string storeName, string productName, string name);
        public void EditProductCategory(string storeName, string productName, string category);
        public void EditProductPrice(string storeName, string productName, double price);
        public void EditProductQuantity(string storeName, string productName, int quantity);
        public void EditProductDescription(string storeName, string productName, string description);
        public void RemovePermissionsOfManager(string storeName, string username, Authorizations authorization);
    }
}
