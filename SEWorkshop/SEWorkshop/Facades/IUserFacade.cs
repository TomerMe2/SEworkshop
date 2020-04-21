using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    interface IUserFacade
    {
        public void Register(string username, string password); //throws exception
        public void Login(string username, string password); //throws exception
        public IEnumerable<Store> BrowseStores();
        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred);
        public IEnumerable<Product> FilterProducts(IEnumerable<Product> products, Func<Product, bool> pred);
        public void SaveProductToBasket(Product product); //throws exception
        public IEnumerable<Basket> MyCart();
        public void AddProductToCart(Product product); //throws exception
        public void RemoveProductFromCart(Product product); //throws exception
        public void Purchase(Product product); //throws exception
        public void Logout(); //throws exception
        public void OpenStore(Store store); //throws exception
        public IEnumerable<Purchase> WatchPurcahseHistory(); //throws exception
    }
}