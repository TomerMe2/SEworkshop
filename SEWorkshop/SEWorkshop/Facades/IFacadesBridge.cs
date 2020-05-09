using SEWorkshop.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public interface IFacadesBridge
    {
        public DataGuestUser CreateGuestUser();
        public void AddProductToCart(DataUser user, string storeName, string productName, int quantity);
        public IEnumerable<DataStore> BrowseStores();
        public DataStore SearchStore(string storeName);
        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public void Login(string username, byte[] password);
        public void Logout();
        public IEnumerable<DataBasket> MyCart(DataUser user);
        public void OpenStore(DataUser user, string storeName);
        public void Purchase(DataUser user, DataBasket basket, string creditCardNumber, Address address);
        public void Register(string username, byte[] password);
        public void RemoveProductFromCart(DataUser user, string storeName, string productName, int quantity);

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

        public IEnumerable<DataPurchase> PurchaseHistory(DataUser user);
        public void WriteReview(DataUser user, string storeName, string productName, string description);
        public void WriteMessage(DataUser user, string storeName, string description);
        public IEnumerable<DataPurchase> UserPurchaseHistory(DataUser user, string userName);
        public IEnumerable<DataPurchase> StorePurchaseHistory(DataUser user, string storeName);
        public IEnumerable<DataPurchase> ManagingPurchaseHistory(DataUser user, string storeName);
        public DataProduct AddProduct(DataUser user, string storeName, string productName, string description, string category, double price, int quantity);
        public void RemoveProduct(DataUser user, string storeName, string productName);
        public void AddStoreOwner(DataUser user, string storeName, string username);
        public void AddStoreManager(DataUser user, string storeName, string username);
        public void SetPermissionsOfManager(DataUser user, string storeName, string username, Authorizations authorization);
        public void RemoveStoreManager(DataUser user, string storeName, string username);
        public IEnumerable<DataMessage> ViewMessage(DataUser user, string storeNm);
        public DataMessage MessageReply(DataUser user, DataMessage message, string storeName, string description);
        public void EditProductName(DataUser user, string storeName, string productName, string name);
        public void EditProductCategory(DataUser user, string storeName, string productName, string category);
        public void EditProductPrice(DataUser user, string storeName, string productName, double price);
        public void EditProductQuantity(DataUser user, string storeName, string productName, int quantity);
        public void EditProductDescription(DataUser user, string storeName, string productName, string description);
        public void RemovePermissionsOfManager(DataUser user, string storeName, string username, Authorizations authorization);
    }
}
