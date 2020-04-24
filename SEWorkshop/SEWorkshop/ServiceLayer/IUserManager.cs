using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.ServiceLayer
{
    interface IUserManager
    {
        public void Register(string username, string password); //throws exception
        public void Login(string username, string password); //throws exception
        public IEnumerable<Store> BrowseStores();
        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred);
        public IEnumerable<Basket> MyCart();
        public void AddProductToCart(Product product); //throws exception
        public void RemoveProductFromCart(Product product); //throws exception
        public void Purchase(Basket basket); //throws exception
        public void Logout(); //throws exception
        public void OpenStore(LoggedInUser owner, string storeName); //throws exception
        public void WriteReview(Product product, string description); // throws exception
        public void WriteMessage(Store store, string description); // throws exception
        public IEnumerable<Purchase> PurcahseHistory(); //throws exception
        public void AddProduct(Store store, string name, string description, string category, double price); //throws exception
        public void RemoveProduct(Store store, string name); //throws exception
        public void AddStoreOwner(Store store, string username); //throws exception
        public void AddStoreManager(Store store, string username); //throws exception
        public void SetPermissionsOfManager(Store store, string username, string authorization); //throws exception
        public void RemoveStoreManager(Store store, string username); //throws exception
        public IEnumerable<Message> ViewMessage(Store store); //throws exception
        public void MessageReply(Message message, Store store, string description); //throws exception
        public IEnumerable<Purchase> ViewPurchaseHistory(Store store); //throws exception
    }
}
