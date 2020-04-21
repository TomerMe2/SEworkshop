using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Facades;

namespace SEWorkshop.ServiceLayer
{
    class GuestManager : IUserManager
    {
        private GuestUserFacade UserFacade = GuestUserFacade.getInstance();
        public static void Main()
        {
        }

        public void Register(string username, string password)
        {
            Result result = UserFacade.Register(username, password);
        }

        public void Login(string username, string password)
        {
            UserFacade.Login(username, password);
        }
    }
}
