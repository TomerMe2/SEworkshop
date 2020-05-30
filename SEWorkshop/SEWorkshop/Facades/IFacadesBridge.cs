using SEWorkshop.DataModels;
using SEWorkshop.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public interface IFacadesBridge
    {
        public void AddProductToCart(DataUser user, string storeName, string productName, int quantity);
        public IEnumerable<DataStore> BrowseStores();
        public DataStore SearchStore(string storeName);
        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred);
        public DataLoggedInUser GetLoggedInUserAndApplyCart(string username, byte[] password, DataGuestUser userAsGuest);
        public DataGuestUser CreateGuest();
        public IEnumerable<DataBasket> MyCart(DataUser user);
        public void OpenStore(DataLoggedInUser user, string storeName);
        public DataPurchase Purchase(DataUser user, DataBasket basket, string creditCardNumber, Address address);
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

        public IEnumerable<DataPurchase> PurchaseHistory(DataLoggedInUser user);
        public void WriteReview(DataLoggedInUser user, string storeName, string productName, string description);
        public DataMessage WriteMessage(DataLoggedInUser user, string storeName, string description);
        public IEnumerable<DataPurchase> UserPurchaseHistory(DataLoggedInUser user, string userName);
        public IEnumerable<DataPurchase> StorePurchaseHistory(DataLoggedInUser user, string storeName);
        public IEnumerable<DataPurchase> ManagingPurchaseHistory(DataLoggedInUser user, string storeName);
        public DataProduct AddProduct(DataLoggedInUser user, string storeName, string productName, string description, string category, double price, int quantity);
        public void RemoveProduct(DataLoggedInUser user, string storeName, string productName);
        public void AddStoreOwner(DataLoggedInUser user, string storeName, string username);
        public void AnswerOwnershipRequest(DataLoggedInUser user, string storeName, string newOwnerUserName, Boolean answer);
        public void AddStoreManager(DataLoggedInUser user, string storeName, string username);
        public void SetPermissionsOfManager(DataLoggedInUser user, string storeName, string username, Authorizations authorization);
        public void RemoveStoreManager(DataLoggedInUser user, string storeName, string username);
        public IEnumerable<DataMessage> ViewMessage(DataLoggedInUser user, string storeNm);
        public DataMessage MessageReply(DataLoggedInUser user, DataMessage message, string storeName, string description);
        public void EditProductName(DataLoggedInUser user, string storeName, string productName, string name);
        public void EditProductCategory(DataLoggedInUser user, string storeName, string productName, string category);
        public void EditProductPrice(DataLoggedInUser user, string storeName, string productName, double price);
        public void EditProductQuantity(DataLoggedInUser user, string storeName, string productName, int quantity);
        public void EditProductDescription(DataLoggedInUser user, string storeName, string productName, string description);
        public void RemovePermissionsOfManager(DataLoggedInUser user, string storeName, string username, Authorizations authorization);

        //All add policies are adding to the end
        public void AddAlwaysTruePolicy(DataLoggedInUser user, string storeName, Operator op);
        public void AddSingleProductQuantityPolicy(DataLoggedInUser user, string storeName, Operator op, string productName, int minQuantity, int maxQuantity);
        public void AddSystemDayPolicy(DataLoggedInUser user, string storeName, Operator op, DayOfWeek cantBuyIn);
        public void AddUserCityPolicy(DataLoggedInUser user, string storeName, Operator op, string requiredCity);
        public void AddUserCountryPolicy(DataLoggedInUser user, string storeName, Operator op, string requiredCountry);
        public void AddWholeStoreQuantityPolicy(DataLoggedInUser user, string storeName, Operator op, int minQuantity, int maxQuantity);
        public void RemovePolicy(DataLoggedInUser user, string storeName, int indexInChain);
        public void MarkAllDiscussionAsRead(DataLoggedInUser user, string storeName, DataMessage msg);

        public void AddProductCategoryDiscount(DataLoggedInUser user, string storeName, string categoryName, DateTime deadline, double percentage, Operator op, int indexInChain);
        public void AddSpecificProductDiscount(DataLoggedInUser user, string storeName, string productName, DateTime deadline, double percentage, Operator op, int indexInChain);
        public void AddBuySomeGetSomeDiscount(DataLoggedInUser user, string storeName, string productName, int buySome, int getSome,DateTime deadline, double percentage, Operator op, int indexInChain);
        public void AddBuyOverDiscount(DataLoggedInUser user, string storeName, string productName, double minSum ,DateTime deadline, double percentage, Operator op, int indexInChain);

        public void RemoveDiscount(DataLoggedInUser user, string storeName, int indexInChain);
        public IEnumerable<string> GetRegisteredUsers();
        public double GetIncomeInDate(DateTime date);

    }
}
