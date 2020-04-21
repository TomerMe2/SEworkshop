using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Facades;

namespace SEWorkshop.ServiceLayer
{
    class UserManager : IUserManager
    {
        IUserFacade UserFacadeInstance = UserFacade.GetInstance();
        readonly IStoreFacade StoreFacadeInstance = StoreFacade.GetInstance();
        LoggedInUser LoggedInUser = null;

        public UserManager()
        {
        }

        public void AddProductToCart(Product product)
        {
            if (!StoreFacadeInstance.IsProductExists(product))
                throw new Exception("Add Product To Cart: This product does not exist in the trading system!");
            UserFacadeInstance.AddProductToCart(LoggedInUser, product);
        }

        public IEnumerable<Store> BrowseStores()
        {
            return StoreFacadeInstance.BrowseStores();
        }

        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            throw new NotImplementedException();
        }

        public void Login(string username, string password)
        {
            LoggedInUser = new LoggedInUser(username, password);
        }

        public void Logout()
        {
            if (LoggedInUser == null)
                throw new Exception("Logout: There is no logged in user!");
            LoggedInUser = null;
            UserFacadeInstance = UserFacade.GetInstance();
        }

        public IEnumerable<Basket> MyCart()
        {
            return UserFacadeInstance.MyCart(LoggedInUser);
        }

        public void OpenStore(LoggedInUser owner, string storeName)
        {
            Func<Store, bool> StoresWithThisNamePredicate = store => store.StoreName.Equals(storeName);
            ICollection<Store> StoresWithTheSameName = StoreFacadeInstance.SearchStore(StoresWithThisNamePredicate);
            if (StoresWithTheSameName.Count > 0)
                throw new Exception("Open Store: Store with this name is already exists");
            StoreFacadeInstance.CreateStore(owner, storeName);
        }

        public void Purchase(Product product)
        {
            UserFacadeInstance.Purchase(product);
        }

        public void Register(string username, string password)
        {
            UserFacadeInstance.Register(username, password);
        }

        public void RemoveProductFromCart(Product product)
        {
            UserFacadeInstance.RemoveProductFromCart(LoggedInUser, product);
        }

        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Purchase> PurcahseHistory()
        {
            return UserFacadeInstance.PurcahseHistory();
        }
    }
}
