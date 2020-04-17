using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class UserFacade
    {

        private static ICollection<RegisteredUser> Users;
        private static bool isAuthorized;
        public UserFacade() : base()
        {
            Users = new List<RegisteredUser>();
            isAuthorized = false;
        }
        
        public static bool Register(RegisteredUser user)
        {
            if(isAuthorized)
            {
                return false;
            }
            foreach(var User in Users)
            {
                if(user.Username.Equals(User.Username))
                {
                    return false;
                }
            }
            Users.Add(user);
            return true;
        }

        public static RegisteredUser Login(string username, string password)
        {
            if(isAuthorized)
            {
                return null;
            }
            foreach(var User in Users)
            {
                if(username.Equals(User.Username))
                {
                    if(password.Equals(User.Password))
                    {
                        isAuthorized = true;
                        return User;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public static void Logout()
        {
            isAuthorized = false;
        }

        public static bool HasAuthorizaton()
        {
            return isAuthorized;
        }
    }
}