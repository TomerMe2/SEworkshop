using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Facades;

namespace SEWorkshop.ServiceLayer
{
    class GuestUI : UserUI
    {
        public static void Home()
        {
            //UserFacadeInstance = UserFacade.getInstance();
            PrintMenu();
            int choice = int.Parse(Console.ReadLine());
            while (choice <= 0 || choice > 5)
            {
                Console.WriteLine("No such option!!!\n");
                PrintMenu();
                choice = int.Parse(Console.ReadLine());
            }
            ApplyChoice(choice);
        }

        public static void PrintMenu()
        {
            UserUI.Home();
            Console.WriteLine("4. Register\n 5. Login\n");
        }

        public static void ApplyChoice(int choice)
        {
            switch (choice)
            {
                case 4:
                    Register();
                    break;
                case 5:
                    Login();
                    break;
                default:
                    UserUI.ApplyChoice(choice);
                    break;
            }
        }

        private static void Register()
        {
            Console.WriteLine("-----Register-----");
            Console.Write("Username: ");
            string Username = Console.ReadLine();
            Console.Write("Password: ");
            string Password = Console.ReadLine();
            Result result = GuestUserFacade.getInstance().Register(new LoggedInUser(Username, Password));
            while (!result.isSuccessful())
            {
                Console.WriteLine(result.ErrorMessage);
                Console.Write("Username: ");
                Username = Console.ReadLine();
                Console.Write("Password: ");
                Password = Console.ReadLine();
                result = GuestUserFacade.getInstance().Register(new LoggedInUser(Username, Password));
            }
            Console.WriteLine("Registered Succefully");
            Login();
        }

        private static void Login()
        {
            Console.WriteLine("-----Login-----");
            Console.Write("Username: ");
            string Username = Console.ReadLine();
            Console.Write("Password: ");
            string Password = Console.ReadLine();
        }
    }
}
