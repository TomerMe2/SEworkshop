using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Enums;
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
            um.AddProduct("1", "nini", "prod1", "nini", "cat1", 10, 10);
            um.AddProduct("1", "nini", "prod2", "nini", "cat1", 10, 10);
            um.AddSpecificProductDiscount("1", "nini", "prod1", DateTime.Now.AddMonths(1), 10, Operator.And, 0, 0, true);
            um.AddBuyOverDiscount(10, "1", "nini", "prod1", DateTime.Now.AddMonths(1), 10, Operator.And, 0, 1, true);
            um.AddProductCategoryDiscount("1", "nini", "cat1", DateTime.Now.AddMonths(1), 10, Operator.And, 0, 3, true);
            um.AddBuySomeGetSomeDiscount(4, 5, "1", "prod1", "prod2", "nini", DateTime.Now.AddMonths(1), 10, Operator.And, 0, 3, true);
            um.RemoveDiscount("1", "nini", 0);
            var deb = true;
        }
    }
}
