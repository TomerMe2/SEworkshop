using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    public class GuestUserFacade : IUserFacade
    {
        private ICollection<LoggedInUser> Users;
        private GuestUser guestUser = null;
        private static GuestUserFacade Instance = null;

        public static GuestUserFacade getInstance()
        {
            if (Instance == null)
                Instance = new GuestUserFacade();
            return Instance;
        }

        private GuestUserFacade()
        {
            Users = new List<LoggedInUser>();
            guestUser = new GuestUser();
        }

        public Result Register(LoggedInUser user)
        {
            foreach(var User in Users)
            {
                if(user.Username.Equals(User.Username))
                {
                    return Result.Error("User with this username already exists");
                }
            }
            Users.Add(user);
            return Result.Success();
        }

        public LoggedInUser Login(string username, string password)
        {
            foreach(var User in Users)
            {
                if(username.Equals(User.Username))
                {
                    if(password.Equals(User.Password))
                    {
                        guestUser = null;
                        return User;                        
                    }
                }
            }
            return null;
        }

        public void Logout()
        {
            guestUser = new GuestUser();
            LoggedInUserFacade.getInstance().Logout();
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