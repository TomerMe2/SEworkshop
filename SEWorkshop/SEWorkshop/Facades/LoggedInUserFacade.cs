using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public class LoggedInUserFacade : IUserFacade
    {
        private static LoggedInUserFacade Instance = null;
        private string loggedInUser {get; set; }

        public static LoggedInUserFacade getInstance()
        {
            if (Instance == null)
                Instance = new LoggedInUserFacade();
            return Instance;
        }

        private LoggedInUserFacade()
        {
            loggedInUser = null;
        }

        public Result Login(string username)
        {
            loggedInUser = username;
            return Result.Success("Welcome back, " + username);
        }

        public bool HasAuthorization(string username)
        {
            return loggedInUser.Equals(username);
        }

        public Result Logout(string username)
        {
            if(!HasAuthorization(username))
            {
                return Result.Error("User is not authorized!");
            }
            loggedInUser = null;
            return Result.Success("Goodbye!");
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