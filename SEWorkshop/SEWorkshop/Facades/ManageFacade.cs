using SEWorkshop.Models;
using SEWorkshop.Exceptions;
using NLog;
using System.Collections.Generic;
using System.Linq;
using SEWorkshop.Enums;
using System;
using SEWorkshop.DAL;

namespace SEWorkshop.Facades
{
    public class ManageFacade : IManageFacade
    {
        public ManageFacade()
        {
        }

        public bool UserHasPermission(LoggedInUser loggedInUser, Store store, Authorizations authorization)
        {
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
            var management = loggedInUser.Manage.FirstOrDefault(man => man.Store.Equals(store));

            return (IsUserStoreOwner(loggedInUser, store)
                    || (IsUserStoreManager(loggedInUser, store)
                        && management.HasAuthorization(authorization)));
        }

        public Product AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price, int quantity)
        {
            var prod = loggedInUser.AddProduct(store, name, description, category, price, quantity);
            DatabaseProxy.Instance.SaveChanges();
            return prod;
        }

        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product productToRemove)
        {
            loggedInUser.RemoveProduct(store, productToRemove);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void EditProductDescription(LoggedInUser loggedInUser, Store store, Product product, string description)
        {
            loggedInUser.EditProductDescription(store, product, description);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void EditProductCategory(LoggedInUser loggedInUser, Store store, Product product, string category)
        {
            loggedInUser.EditProductCategory(store, product, category);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void EditProductName(LoggedInUser loggedInUser, Store store, Product product, string name)
        {
            loggedInUser.EditProductName(store, product, name);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void EditProductPrice(LoggedInUser loggedInUser, Store store, Product product, double price)
        {
            loggedInUser.EditProductPrice(store, product, price);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void EditProductQuantity(LoggedInUser loggedInUser, Store store, Product product, int quantity)
        {
           loggedInUser.EditProductQuantity(store, product, quantity);
           DatabaseProxy.Instance.SaveChanges();
        }

        public OwnershipRequest? AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner)
        {
            loggedInUser.AddStoreOwner(store, newOwner);
            var request = newOwner.OwnershipRequests.FirstOrDefault(request => request.Store == store);
            DatabaseProxy.Instance.SaveChanges();
            return request;
        }

        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager)
        {
            loggedInUser.AddStoreManager(store, newManager);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization)
        {
            loggedInUser.SetPermissionsOfManager(store, manager,authorization);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void RemoveStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser managerToRemove)
        {
           loggedInUser.RemoveStoreManager(store, managerToRemove);
           DatabaseProxy.Instance.SaveChanges();
        }

        public void RemoveStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser ownerToRemove)
        {
            loggedInUser.RemoveStoreOwner(store, ownerToRemove);
            DatabaseProxy.Instance.SaveChanges();
        }

        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store)
        {
           return loggedInUser.GetMessage(store);
        }

        public Message MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description)
        {
            var msg = loggedInUser.MessageReply(message, store, description);
            DatabaseProxy.Instance.SaveChanges();
            return msg;
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Watching))
            { 
                return store.Purchases;
            }
            throw new UserHasNoPermissionException();
        }

        public bool IsUserStoreOwner(LoggedInUser user, Store store) => ((from owner in user.Owns
                                                                          where owner.StoreName.Equals(store.Name)
                                                                          select owner).ToList().Count() > 0);

        public bool IsUserStoreManager(LoggedInUser user, Store store) => ((from manager in user.Manage
                                                                            where manager.StoreName.Equals(store.Name)
                                                                            select manager).ToList().Count() > 0);

        public bool StoreContainsProduct(Store store, Product product) => ((from pr in store.Products
                                                                            where pr.Name == product.Name
                                                                            select product).ToList().Count() > 0);

        public void RemovePermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization)
        {
            loggedInUser.RemovePermissionsOfManager(store, manager,authorization);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AnswerOwnershipRequest(LoggedInUser loggedInUser, Store store,LoggedInUser newOwner, RequestState answer)
        {
            loggedInUser.AnswerOwnershipRequest(store, newOwner,answer);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddBuySomeGetSomeDiscount(LoggedInUser user, Store store, Product prod1, Product prod2, int buySome, int getSome, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft)
        {
            user.AddBuySomeGetSomeFreeDiscount(store, prod1, prod2, deadline, percentage, buySome, getSome, op, indexInChain, disId, toLeft);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddBuyOverDiscount(LoggedInUser user, Store store, Product product, double minSum, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft)
        {
            user.AddBuyOverDiscountDiscount(store, product, deadline, percentage, minSum, op, indexInChain, disId, toLeft);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddProductCategoryDiscount(LoggedInUser user, Store store, string categoryName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            user.AddProductCategoryDiscount(store, categoryName, deadline, percentage, op, indexInChain, disId, toLeft);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddSpecificProductDiscount(LoggedInUser user, Store store, Product product, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            user.AddSpecificProductDiscount(store, product, deadline, percentage, op, indexInChain, disId, toLeft);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void RemoveDiscount(LoggedInUser user, Store store, int indexInChain)
        {
            user.RemoveDiscount(store, indexInChain);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddAlwaysTruePolicy(LoggedInUser user, Store storeName, Operator op)
        {
            user.AddAlwaysTruePolicy(storeName, op);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddSingleProductQuantityPolicy(LoggedInUser user, Store storeName, Operator op, Product productName, int minQuantity, int maxQuantity)
        {
            user.AddSingleProductQuantityPolicy(storeName, op,
                    productName, minQuantity, maxQuantity);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddSystemDayPolicy(LoggedInUser user, Store storeName, Operator op, Weekday cantBuyIn)
        {
            user.AddSystemDayPolicy(storeName, op, cantBuyIn);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddUserCityPolicy(LoggedInUser user, Store storeName, Operator op, string requiredCity)
        {
            user.AddUserCityPolicy(storeName, op, requiredCity);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddUserCountryPolicy(LoggedInUser user, Store storeName, Operator op, string requiredCountry)
        {
            user.AddUserCountryPolicy(storeName, op, requiredCountry);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AddWholeStoreQuantityPolicy(LoggedInUser user, Store storeName, Operator op, int minQuantity, int maxQuantity)
        {
            user.AddWholeStoreQuantityPolicy(storeName, op, minQuantity, maxQuantity);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void RemovePolicy(LoggedInUser user, Store storeName, int indexInChain)
        {
            user.RemovePolicy(storeName, indexInChain);
            DatabaseProxy.Instance.SaveChanges();
        }
    }
}