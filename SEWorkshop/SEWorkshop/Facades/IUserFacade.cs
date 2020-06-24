using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Models;

namespace SEWorkshop.Facades
{
    public interface IUserFacade
    {
        public LoggedInUser Register(string username, byte[] password); //throws exception
        public IEnumerable<Basket> MyCart(User user);
        public void AddProductToCart(User user, Product product, int quantity); //throws exception
        public void RemoveProductFromCart(User user, Product product, int quantity); //throws exception
        public Purchase Purchase(User user, Basket basket, string creditCardNumber, DateTime expirationDate, string cvv, Address address, string username, string id); //throws exception
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
        public IDictionary<DateTime, IDictionary<KindOfUser, int>> GetUseRecord(DateTime dateFrom, DateTime dateTo);
        public IDictionary<Enums.KindOfUser, int> GetUsersByCategory(DateTime today);
    }
}