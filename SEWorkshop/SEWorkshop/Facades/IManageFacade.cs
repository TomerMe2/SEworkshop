using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Facades
{
    interface IManageFacade
    {
        public void AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price, int quantity); //throws exception
        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product product); //throws exception
        public void AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner); //throws exception
        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager); //throws exception
        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization); //throws exception
        public void RemoveStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser managerToRemove); //throws exception
        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store); //throws exception
        public void MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description); //throws exception
        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store); //throws exception
    }
}