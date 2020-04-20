using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public class LoggedInUserFacade : IUserFacade
    {
        private LoggedInUser loggedInUser = null;
        private static LoggedInUserFacade Instance = null;

        public static LoggedInUserFacade getInstance()
        {
            if (Instance == null)
                Instance = new LoggedInUserFacade();
            return Instance;
        }

        private LoggedInUserFacade()
        {
        }

        public LoggedInUser Login(String username, String password)
        {
            loggedInUser = GuestUserFacade.getInstance().Login(username, password);
            return loggedInUser;
        }

        public bool HasAuthorizaton()
        {
            return loggedInUser != null;
        }

        public void Logout()
        {
            loggedInUser = null;
            GuestUserFacade.getInstance().Logout();
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
    }
}