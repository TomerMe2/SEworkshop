using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class GuestUser : User
    {
        public GuestUser() : base() { }
        
        public Result Register(string username, string password) {
            LoggedInUser user = new LoggedInUser(username, password);
            if(Facades.LoggedInUserFacade.getInstance().HasAuthorization(username))
            {
                return Result.Error("Please logout before registration");
            }
            return Facades.GuestUserFacade.getInstance().Register(username, password);
        }

        public Result Login(string username, string password) {
            if(Facades.LoggedInUserFacade.getInstance().HasAuthorization(username))
            {
                return Result.Error("You are already logged in");
            }
            return Facades.GuestUserFacade.getInstance().Login(username, password);
        }
    }
}
