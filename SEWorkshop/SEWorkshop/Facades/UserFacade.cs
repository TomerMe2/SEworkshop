using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    class UserFacade
    {
        private ICollection<RegisteredUser> Users;
        private RegisteredUser loggedInUser = null;
        private static UserFacade Instance = null;

        public static UserFacade getInstance()
        {
            if (Instance == null)
                Instance = new UserFacade();
            return Instance;
        }

        private UserFacade()
        {
            Users = new List<RegisteredUser>();
        }

        public bool isLoggedIn()
        {
            return loggedInUser != null;
        }

        public Result Register(string Username, string Password)
        {
            RegisteredUser user = new RegisteredUser(Username, Password);
            if(isLoggedIn())
            {
                return Result.Error("User is already logged in");
            }
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

        public bool Login(string username, string password)
        {
            if(isLoggedIn())
            {
                return false;
            }
            foreach(var User in Users)
            {
                if(username.Equals(User.Username))
                {
                    if(password.Equals(User.Password))
                    {
                        loggedInUser = User;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Logout()
        {
            loggedInUser = null;
        }
    }
}