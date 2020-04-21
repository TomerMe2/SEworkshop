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
        bool IsLoggedIn = false;

        public UserManager()
        {
        }

        public void AddProductToCart(Product product)
        {
            if (!StoreFacadeInstance.IsProductExists(product))
                throw new ProductNotInTradingSystemException();
            UserFacadeInstance.AddProductToCart(User, product);
        }

        public IEnumerable<Store> BrowseStores()
        {
            return StoreFacadeInstance.BrowseStores();
        }

        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            if (products.Count == 0)
                throw new NoProductsToFilterException();
            return StoreFacadeInstance.FilterProducts(products, pred);
        }

        public void Login(string username, string password)
        {
            if (IsLoggedIn)
                throw new UserAlreadyLoggedInException();
            User = new LoggedInUser(username, password);
        }

        public void Logout()
        {
            if (!IsLoggedIn)
                throw new UserIsNotLoggedInException();
            User = new GuestUser();
        }

        public IEnumerable<Basket> MyCart()
        {
            return UserFacadeInstance.MyCart(User);
        }

        public void OpenStore(LoggedInUser owner, string storeName)
        {
            Func<Store, bool> StoresWithThisNamePredicate = store => store.Name.Equals(storeName);
            ICollection<Store> StoresWithTheSameName = StoreFacadeInstance.SearchStore(StoresWithThisNamePredicate);
            if (StoresWithTheSameName.Count > 0)
                throw new StoreWithThisNameAlreadyExistsException();
            StoreFacadeInstance.CreateStore(owner, storeName);
        }

        public void Purchase(Basket basket)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            UserFacadeInstance.Purchase(User, basket);
        }

        public void Register(string username, string password)
        {
            if (IsLoggedIn)
                throw new UserAlreadyLoggedInException();
            UserFacadeInstance.Register(username, password);
        }

        public void RemoveProductFromCart(Product product)
        {
            if (!StoreFacadeInstance.IsProductExists(product))
                throw new ProductNotInTradingSystemException();
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
