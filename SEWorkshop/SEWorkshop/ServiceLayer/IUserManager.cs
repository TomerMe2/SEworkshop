using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.ServiceLayer
{
    interface IUserManager
    {
        public void AddProductToCart(Product product);
        public IEnumerable<Store> BrowseStores();
        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred);
        public void Login(string username, string password);
        public void Logout();
        public IEnumerable<Basket> MyCart();
        public void OpenStore(LoggedInUser owner, string storeName);
        public void Purchase(Basket basket);
        public void Register(string username, string password);
        public void RemoveProductFromCart(Product product);
        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred);
        public IEnumerable<Purchase> PurcahseHistory();
        public void WriteReview(Product product, string description);
        public void WriteMessage(Store store, string description);
        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser user);
        public IEnumerable<Purchase> StorePurchaseHistory(Store store);
        public void AddProduct(Store store, string name, string description, string category, double price);
        public void RemoveProduct(Store store, string name);
        public void AddStoreOwner(Store store, string username);
        public void AddStoreManager(Store store, string username);
        public void SetPermissionsOfManager(Store store, string username, string authorization);
        public void RemoveStoreManager(Store store, string username);
        public IEnumerable<Message> ViewMessage(Store store);
        public void MessageReply(Message message, Store store, string description);
    }
}
