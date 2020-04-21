using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    interface IUserFacade
    {
        public LoggedInUser Register(string username, string password); //throws exception
        public LoggedInUser Login(string username, string password); //throws exception
        public void Logout(); //throws exception        
        public IEnumerable<Basket> MyCart(User user);
        public void AddProductToCart(User user, Product product); //throws exception
        public void RemoveProductFromCart(User user, Product product); //throws exception
        public void Purchase(User user, Basket basket); //throws exception
        public IEnumerable<Purchase> PurcahseHistory(User user);
    }
}