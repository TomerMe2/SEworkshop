using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.DAL;

namespace SEWorkshop.Tests.UnitTests
{
    [TestFixture]
    public class GuestUserTests
    {
        SecurityAdapter _securityAdapter = new SecurityAdapter();
        const string CREDIT_CARD_NUMBER_STUB = "1234";
        const string CITY_NAME_STUB = "Beer Sheva";
        const string STREET_NAME_STUB = "Shderot Ben Gurion";
        const string HOUSE_NUMBER_STUB = "111";
        const string COUNTRY_STUB = "Israel";
        const string ZIP_STUB = "1234";
        Address address = new Address(COUNTRY_STUB, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB, ZIP_STUB);

        [OneTimeSetUp]
        public void Init()
        {
            DatabaseProxy.MoveToTestDb();
        }

        [Test]
        public void AddProductToCartTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = new Product(store, "BestApp", "Authentic One", "App", 4.00, 10);
            store.Products.Add(product);

            User user = new GuestUser();
            user.AddProductToCart(product, 5);

            Assert.IsTrue(user.Cart.Baskets.ElementAt(0).Products.ElementAt(0).Product == product);
            Assert.IsTrue(user.Cart.Baskets.ElementAt(0).Products.ElementAt(0).Quantity == 5);
        }

        [Test]
        public void RemoveProductProductFromCartTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = new Product(store, "BestApp", "Authentic One", "App", 4.00, 10);
            store.Products.Add(product);

            User user = new GuestUser();
            user.Cart.Baskets.Add(new Basket(store, user.Cart));
            user.Cart.Baskets.ElementAt(0).Products.Add(new ProductsInBasket(user.Cart.Baskets.ElementAt(0), product, 5));
            user.RemoveProductFromCart(user, product, 3);

            Assert.IsTrue(user.Cart.Baskets.ElementAt(0).Products.ElementAt(0).Product == product);
            Assert.IsTrue(user.Cart.Baskets.ElementAt(0).Products.ElementAt(0).Quantity == 2);
            
            user.RemoveProductFromCart(user, product, 2);
            Assert.IsTrue(user.Cart.Baskets.Count() == 0);
        }

        [Test]
        public void PurchaseTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = new Product(store, "BestApp", "Authentic One", "App", 4.00, 10);
            store.Products.Add(product);

            User user = new GuestUser();
            user.Cart.Baskets.Add(new Basket(store, user.Cart));
            user.Cart.Baskets.ElementAt(0).Products.Add(new ProductsInBasket(user.Cart.Baskets.ElementAt(0), product, 5));

            Purchase purchase = user.Purchase(user.Cart.Baskets.ElementAt(0), "Mich's Credit Card", new Address("Israel", "Beersheba", "Rager Blv.", "123", "1234"));
            Assert.IsTrue(user.Cart.Baskets.Count() == 0);
            Assert.IsTrue(store.Purchases.ElementAt(0) == purchase);
            Assert.IsTrue(store.Products.ElementAt(0).Quantity == 5);
        }
    }
}
