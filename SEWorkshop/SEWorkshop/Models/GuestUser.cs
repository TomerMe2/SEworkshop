using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class GuestUser : User
    {
        public GuestUser() : base() { }
        
        Result Register(string username, string password) {
            LoggedInUser user = new LoggedInUser(username, password);
            if(Facades.LoggedInUserFacade.getInstance().HasAuthorization(username))
            {
                Console.WriteLine("Please logout before registration");
                return null;
            }
            return Facades.GuestUserFacade.getInstance().Register(username, password);
        }

        Result Login(string username, string password) {
            if(Facades.LoggedInUserFacade.getInstance().HasAuthorization(username))
            {
                Console.WriteLine("You are already logged in");
                return null;
            }
            return Facades.GuestUserFacade.getInstance().Login(username, password);
        }
    }
}
