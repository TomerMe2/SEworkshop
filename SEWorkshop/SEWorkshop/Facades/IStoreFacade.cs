using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Facades
{
    interface IStoreFacade
    {
        public Store CreateStore(LoggedInUser owner, string storeName);
        public ICollection<Store> BrowseStores();
        public ICollection<Store> SearchStore(Func<Store, bool> pred);
        public bool IsProductExists(Product product);
        public ICollection<Product> SearchProducts(Func<Product, bool> pred);
        public ICollection<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred);
        public void AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price);
        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product product);
        public void AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner);
        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager);
        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization);
        public void RemoveStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser managerToRemove);
        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store);
        public void MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description);
        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store);
    }
}
