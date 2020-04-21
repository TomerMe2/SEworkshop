using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.ServiceLayer
{
    class UserManager : IUserManager
    {
        public void AddProductToCart(Product product)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Store> BrowseStores()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
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

        public IEnumerable<Basket> MyCart()
        {
            throw new NotImplementedException();
        }

        public void OpenStore(Store store)
        {
            throw new NotImplementedException();
        }

        public void Purchase(Product product)
        {
            throw new NotImplementedException();
        }

        public void Register(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void RemoveProductFromCart(Product product)
        {
            throw new NotImplementedException();
        }

        public void SaveProductToBasket(Product product)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Purchase> WatchPurcahseHistory()
        {
            throw new NotImplementedException();
        }
    }
}
