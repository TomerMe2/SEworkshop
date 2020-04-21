using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    interface IUserFacade
    {
        public void Register(string username, string password); //throws exception
        public void Login(string username, string password); //throws exception
        public ICollection<Basket> MyCart(LoggedInUser user);
        public void AddProductToCart(LoggedInUser user, Product product); //throws exception
        public void RemoveProductFromCart(LoggedInUser user, Product product); //throws exception
        public void Purchase(Product product); //throws exception
        public void Logout(); //throws exception
        public ICollection<Purchase> PurcahseHistory(); //throws exception
    }
}