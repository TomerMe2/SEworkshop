using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public class GuestUserFacade : IUserFacade
    {
        private IDictionary<string, string> Users {get; set;}
        private static GuestUserFacade Instance = null;

        public static GuestUserFacade getInstance()
        {
            if (Instance == null)
                Instance = new GuestUserFacade();
            return Instance;
        }

        private GuestUserFacade()
        {
            Users = new Dictionary<string, string>();
        }

        public Result Register(string username, string password)
        {
            foreach(var User in Users)
            {
                if(username.Equals(User.Key))
                {
                    return Result.Error("User with this username already exists");
                }
            }
            Users.Add(username, password);
            return Result.Success("Welcome!");
        }

        public Result Login(string username, string password)
        {
            foreach(var User in Users)
            {
                if(username.Equals(User.Key))
                {
                    if(password.Equals(User.Value))
                    {
                        return LoggedInUserFacade.getInstance().Login(username);
                    }
                }
            }
            return Result.Error("Wrong Username or Password");
        }

        public void CreateBasket(Cart cart, Store store)
        {
            //TODO
        }
        
        public void AddProductToBasket(Product proudct)
        {
            //TODO
        }

        public void StoreInfo(Store store)
        {
            //TODO
        }

        public void SearchProducts()
        {
            //TODO
        }

        public void CartInfo(Cart cart)
        {
            //TODO
        }

        public void Purchase(Basket basket)
        {
            //TODO
        }

        void IUserFacade.Register(string username, string password)
        {
            throw new NotImplementedException();
        }

        void IUserFacade.Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Store> BrowseStores()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> FilterProducts(IEnumerable<Product> products, Func<Product, bool> pred)
        {
            throw new NotImplementedException();
        }

        public void SaveProductToBasket(Product product)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Basket> MyCart()
        {
            throw new NotImplementedException();
        }

        public void AddProductToCart(Product product)
        {
            throw new NotImplementedException();
        }

        public void RemoveProductFromCart(Product product)
        {
            throw new NotImplementedException();
        }

        public void Purchase(Product product)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public void OpenStore(Store store)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Purchase> WatchPurcahseHistory()
        {
            throw new NotImplementedException();
        }
    }
}