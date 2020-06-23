using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Facades
{
    public interface IUserFacade
    {
        public LoggedInUser Register(string username, byte[] password); //throws exception
        public IEnumerable<Basket> MyCart(User user);
        public void AddProductToCart(User user, Product product, int quantity); //throws exception
        public void RemoveProductFromCart(User user, Product product, int quantity); //throws exception
        public Purchase Purchase(User user, Basket basket, string creditCardNumber, Address address); //throws exception
        public IEnumerable<Purchase> PurchaseHistory(LoggedInUser user);
        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser requesting, string userToView);
        public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store);
        public LoggedInUser GetLoggedInUser(string username);
        public LoggedInUser GetLoggedInUser(string username, byte[] password);
        public GuestUser GetGuestUser(int id);
        public GuestUser CreateGuestUser();
        public Message WriteMessage(LoggedInUser user, Store store, string description);
        public void WriteReview(LoggedInUser user, Product product, string description);
        public IEnumerable<string> GetRegisteredUsers();
        public double GetIncomeInDate(DateTime date);
        public int GetGuestEntriesInDate(DateTime date);
        public int GetLoggedEntriesDate(DateTime date);
        public int GetOwnersEntriesDate(DateTime date);
        public int GetOnlyManagersEntriesDate(DateTime date);
        public int GetAdminsEntriesDate(DateTime date);

    }
}