using SEWorkshop.ServiceLayer;
using System;

namespace SEWorkshop
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args) {
            UserManager userManager = new UserManager();
        }
    }
}
