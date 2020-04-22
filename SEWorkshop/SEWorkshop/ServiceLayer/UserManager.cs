using System;
using System.Collections.Generic;
using SEWorkshop.Facades;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;

namespace SEWorkshop.ServiceLayer
{
    public class UserManager : IUserManager
    {
        IUserFacade UserFacadeInstance = UserFacade.GetInstance();
        readonly IStoreFacade StoreFacadeInstance = StoreFacade.GetInstance();
        User User = new GuestUser();

        public UserManager()
        {
        }

        public void AddProductToCart(Product product)
        {
            UserFacadeInstance.AddProductToCart(User, product);
        }

        public IEnumerable<Store> BrowseStores()
        {
            return StoreFacadeInstance.BrowseStores();
        }

        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            return StoreFacadeInstance.FilterProducts(products, pred);
        }

        public void Login(string username, string password)
        {
            Cart cart = User.Cart;
            User = UserFacadeInstance.Login(username, password);
            User.Cart = cart;
        }

        public void Logout()
        {
            Cart cart = User.Cart;
            UserFacadeInstance.Logout();
            User = new GuestUser();
            User.Cart = cart;
        }

        public IEnumerable<Basket> MyCart()
        {
            return UserFacadeInstance.MyCart(User);
        }

        public void OpenStore(LoggedInUser owner, string storeName)
        {
            StoreFacadeInstance.CreateStore(owner, storeName);
        }

        public void Purchase(Basket basket)
        {
            UserFacadeInstance.Purchase(User, basket);
        }

        public void Register(string username, string password)
        {
            UserFacadeInstance.Register(username, password);
        }

        public void RemoveProductFromCart(Product product)
        {
            UserFacadeInstance.RemoveProductFromCart(User, product);
        }

        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            return StoreFacadeInstance.SearchProducts(pred);
        }

        public IEnumerable<Purchase> PurcahseHistory()
        {
            return UserFacadeInstance.PurcahseHistory(User);
        }
    }
}
