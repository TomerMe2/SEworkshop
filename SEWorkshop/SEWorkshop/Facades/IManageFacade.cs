using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Models;

namespace SEWorkshop.Facades
{
    public interface IManageFacade
    {
        public Product AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price, int quantity); //throws exception
        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product product); //throws exception
        public OwnershipRequest? AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner); //throws exception
        public void AnswerOwnershipRequest(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner, RequestState answer);
        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager); //throws exception
        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization); //throws exception
        public void RemoveStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser managerToRemove); //throws exception
        public void RemoveStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser ownerToRemove); //throws exception
        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store); //throws exception
        public Message MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description); //throws exception
        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store); //throws exception
        public void EditProductDescription(LoggedInUser loggedInUser, Store store, Product product, string description); //throws exception
        public void EditProductName(LoggedInUser loggedInUser, Store store, Product product, string name); //throws exception
        public void EditProductCategory(LoggedInUser loggedInUser, Store store, Product product, string category); //throws exception
        public void EditProductPrice(LoggedInUser loggedInUser, Store store, Product product, double price); //throws exception
        public void EditProductQuantity(LoggedInUser loggedInUser, Store store, Product product, int quantity); //throws exception
        public bool UserHasPermission(LoggedInUser loggedInUser, Store store, Authorizations authorization);
        public bool IsUserStoreOwner(LoggedInUser user, Store store);
        public bool IsUserStoreManager(LoggedInUser user, Store store);
        public bool StoreContainsProduct(Store store, Product product);

        public void RemovePermissionsOfManager(LoggedInUser currUser, Store getStore, LoggedInUser getUser, Authorizations authorization);
        public void AddProductCategoryDiscount(LoggedInUser user, Store storeName, string categoryName, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);
        public void AddSpecificProductDiscount(LoggedInUser user, Store storeName, Product product, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);
        public void AddBuySomeGetSomeDiscount(LoggedInUser user, Store storeName, Product prod1, Product prod2, int buySome, int getSome, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);
        public void AddBuyOverDiscount(LoggedInUser user, Store storeName, Product product, double minSum, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft);

        public void RemoveDiscount(LoggedInUser user, Store storeName, int indexInChain);

        public void AddAlwaysTruePolicy(LoggedInUser user, Store storeName, Operator op);
        public void AddSingleProductQuantityPolicy(LoggedInUser user, Store storeName, Operator op, Product productName, int minQuantity, int maxQuantity);
        public void AddSystemDayPolicy(LoggedInUser user, Store storeName, Operator op, Weekday cantBuyIn);
        public void AddUserCityPolicy(LoggedInUser user, Store storeName, Operator op, string requiredCity);
        public void AddUserCountryPolicy(LoggedInUser user, Store storeName, Operator op, string requiredCountry);
        public void AddWholeStoreQuantityPolicy(LoggedInUser user, Store storeName, Operator op, int minQuantity, int maxQuantity);
        public void RemovePolicy(LoggedInUser user, Store storeName, int indexInChain);
    }
}