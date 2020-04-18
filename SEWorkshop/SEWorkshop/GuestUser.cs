using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class GuestUser : User
    {
        public GuestUser() : base() { }
        
        RegisteredUser Register(string username, string password) {
            RegisteredUser user = new RegisteredUser(username, password);
            if(UserFacade.HasAuthorizaton())
            {
                Console.WriteLine("Please logout before registration");
                return null;
            }
            if(!UserFacade.Register(user))
            {
                Console.WriteLine("An error has occured");
                    return null;
            }
            return user;
        }

        RegisteredUser Login(string username, string password) {
            if(UserFacade.HasAuthorizaton())
            {
                Console.WriteLine("You are already logged in");
                return null;
            }
            RegisteredUser user = UserFacade.Login(username, password);
            if(user == null)
            {
                Console.WriteLine("An error has occured");
            }
            return user;
        }
    }
}
