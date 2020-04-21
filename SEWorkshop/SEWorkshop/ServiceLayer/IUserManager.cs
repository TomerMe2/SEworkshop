using System;
using System.Collections.Generic;
using System.Text;

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
        public IEnumerable<Purchase> PurcahseHistory(); //throws exception
    }
}
