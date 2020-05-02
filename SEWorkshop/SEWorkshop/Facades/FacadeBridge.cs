using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.DataModels;

namespace SEWorkshop.Facades
{
    public class FacadeBridge : IFacadesBridge
    {
        private IUserFacade UserFacade { get; }
        private IManageFacade ManageFacade { get; }
        private IStoreFacade StoreFacade { get; }

        public FacadeBridge()
        {
            UserFacade = new UserFacade();
            ManageFacade = new ManageFacade();
            StoreFacade = new StoreFacade();
        }
        public DataProduct AddProduct(string storeNm, string productName, string description, string category, double price, int quantity)
        {
            throw new NotImplementedException();
        }

        public void AddProductToCart(string storeName, string productName, int quantity)
        {
            throw new NotImplementedException();
        }

        public void AddStoreManager(string storeNm, string username)
        {
            throw new NotImplementedException();
        }

        public void AddStoreOwner(string storeNm, string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataStore> BrowseStores()
        {
            throw new NotImplementedException();
        }

        public void EditProductCategory(string storeName, string productName, string category)
        {
            throw new NotImplementedException();
        }

        public void EditProductDescription(string storeName, string productName, string description)
        {
            throw new NotImplementedException();
        }

        public void EditProductName(string storeName, string productName, string Name)
        {
            throw new NotImplementedException();
        }

        public void EditProductPrice(string storeName, string productName, double price)
        {
            throw new NotImplementedException();
        }

        public void EditProductQuantity(string storeName, string productName, int quantity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            throw new NotImplementedException();
        }

        public void Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeNm)
        {
            throw new NotImplementedException();
        }

        public DataMessage MessageReply(DataMessage message, string storeNm, string description)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataBasket> MyCart()
        {
            throw new NotImplementedException();
        }

        public void OpenStore(string storeName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataPurchase> PurcahseHistory()
        {
            throw new NotImplementedException();
        }

        public void Purchase(DataBasket basket)
        {
            throw new NotImplementedException();
        }

        public void Register(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void RemoveProduct(string storeNm, string productName)
        {
            throw new NotImplementedException();
        }

        public void RemoveProductFromCart(string storeName, string productName, int quantity)
        {
            throw new NotImplementedException();
        }

        public void RemoveStoreManager(string storeNm, string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataProduct> SearchProductsByCategory(ref string input)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataProduct> SearchProductsByKeywords(ref string input)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataProduct> SearchProductsByName(ref string input)
        {
            throw new NotImplementedException();
        }

        public void SetPermissionsOfManager(string storeNm, string username, string authorization)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataPurchase> StorePurchaseHistory(string storeNm)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataPurchase> UserPurchaseHistory(string userNm)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataMessage> ViewMessage(string storeNm)
        {
            throw new NotImplementedException();
        }

        public void WriteMessage(string storeName, string description)
        {
            throw new NotImplementedException();
        }

        public void WriteReview(string storeName, string productName, string description)
        {
            throw new NotImplementedException();
        }
    }
}
