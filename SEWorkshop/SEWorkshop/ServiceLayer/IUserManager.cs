using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.ServiceLayer
{
    class IUserManager
    {
        public static void Home()
        {
            Console.WriteLine("Welcome to the trading system!");
            Console.WriteLine("1. Browse Stores\n 2. Search Product\n 3. My Cart\n");
        }

        protected static void ApplyChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    BrowseStores();
                    break;
                case 2:
                    SearchProduct();
                    break;
                case 3:
                    MyCart();
                    break;
            }
        }

        private static void MyCart()
        {
            throw new NotImplementedException();
        }

        private static void SearchProduct()
        {
            throw new NotImplementedException();
        }

        private static void BrowseStores()
        {
            throw new NotImplementedException();
        }
    }
}
