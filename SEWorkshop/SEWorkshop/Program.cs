using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.ServiceLayer;

namespace SEWorkshop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IUserManager um = new UserManager();
            um.Register("1", "wello", "1234");
            um.Login("1", "wello", "1234");
            um.OpenStore("1", "nini");
            var deb = true;
        }
    }
}
