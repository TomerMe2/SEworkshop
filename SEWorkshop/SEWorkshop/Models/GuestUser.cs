using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class GuestUser : User
    {
        public GuestUser() : base() { }
        
        LoggedInUser Register(string username, string password) {
            LoggedInUser user = new LoggedInUser(username, password);
            if(Facades.LoggedInUserFacade.getInstance().HasAuthorizaton())
            {
                Console.WriteLine("Please logout before registration");
                return null;
            }
            if(Facades.GuestUserFacade.getInstance().Register(user).isSuccessful())
            {
                Console.WriteLine("An error has occured");
                    return null;
            }
            return user;
        }

        LoggedInUser Login(string username, string password) {
            if(Facades.LoggedInUserFacade.getInstance().HasAuthorizaton())
            {
                Console.WriteLine("You are already logged in");
                return null;
            }
            LoggedInUser user = Facades.LoggedInUserFacade.getInstance().Login(username, password);
            if(user == null)
            {
                Console.WriteLine("An error has occured");
            }
            return user;
        }
    }
}
