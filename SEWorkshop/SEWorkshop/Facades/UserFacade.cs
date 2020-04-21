using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public class UserFacade : IUserFacade
    {
        private IDictionary<string, string> Users {get; set;}
        private static UserFacade Instance = null;

        public static UserFacade GetInstance()
        {
            if (Instance == null)
                Instance = new UserFacade();
            return Instance;
        }

        public void Register(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public ICollection<Basket> MyCart(LoggedInUser user)
        {
            throw new NotImplementedException();
        }

        public void AddProductToCart(LoggedInUser user, Product product)
        {
            throw new NotImplementedException();
        }

        public void RemoveProductFromCart(LoggedInUser user, Product product)
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

        public ICollection<Purchase> PurcahseHistory()
        {
            throw new NotImplementedException();
        }

        private UserFacade()
        {
            Users = new Dictionary<string, string>();
        }

    }
}