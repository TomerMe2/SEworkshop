using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Facades
{
    interface IUserFacade
    {
        public LoggedInUser Register(string username, byte[] password); //throws exception
        public LoggedInUser Login(string username, byte[] password); //throws exception
        public void Logout(); //throws exception        
        public IEnumerable<Basket> MyCart(User user);
        public void AddProductToCart(User user, Product product, int quantity); //throws exception
        public void RemoveProductFromCart(User user, Product product, int quantity); //throws exception
        public void Purchase(User user, Basket basket); //throws exception
        public IEnumerable<Purchase> PurcahseHistory(User user);
        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser requesting, string userToView);
        public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store);
        public LoggedInUser GetUser(string username);
    }
}